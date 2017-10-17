/* Copyright 2016. Bloomberg Finance L.P.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:  The above
 * copyright notice and this permission notice shall be included in all copies
 * or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using Name = Bloomberglp.Blpapi.Name;
using Session = Bloomberglp.Blpapi.Session;
using Service = Bloomberglp.Blpapi.Service;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;
using CorrelationID = Bloomberglp.Blpapi.CorrelationID;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using SchemaElementDefinition = Bloomberglp.Blpapi.SchemaElementDefinition;
using SchemaTypeDefinition = Bloomberglp.Blpapi.SchemaTypeDefinition;
using Event = Bloomberglp.Blpapi.Event;
using EventType = Bloomberglp.Blpapi.Event.EventType;
using Message = Bloomberglp.Blpapi.Message;
using Request = Bloomberglp.Blpapi.Request;
using Subscription = Bloomberglp.Blpapi.Subscription;
namespace com.bloomberg.emsx.samples
{
    public class EasyMSX : NotificationHandler
    {
        // EVENTS
        private static readonly Name ORDER_ROUTE_FIELDS = new Name("OrderRouteFields");
        // ADMIN
        private static readonly Name SLOW_CONSUMER_WARNING = new Name("SlowConsumerWarning");
        private static readonly Name SLOW_CONSUMER_WARNING_CLEARED = new Name("SlowConsumerWarningCleared");
        // SESSION_STATUS
        private static readonly Name SESSION_STARTED = new Name("SessionStarted");
        private static readonly Name SESSION_TERMINATED = new Name("SessionTerminated");
        private static readonly Name SESSION_STARTUP_FAILURE = new Name("SessionStartupFailure");
        private static readonly Name SESSION_CONNECTION_UP = new Name("SessionConnectionUp");
        private static readonly Name SESSION_CONNECTION_DOWN = new Name("SessionConnectionDown");
        // SERVICE_STATUS
        private static readonly Name SERVICE_OPENED = new Name("ServiceOpened");
        private static readonly Name SERVICE_OPEN_FAILURE = new Name("ServiceOpenFailure");
        // SUBSCRIPTION_STATUS + SUBSCRIPTION_DATA
        private static readonly Name SUBSCRIPTION_FAILURE = new Name("SubscriptionFailure");
        private static readonly Name SUBSCRIPTION_STARTED = new Name("SubscriptionStarted");
        private static readonly Name SUBSCRIPTION_TERMINATED = new Name("SubscriptionTerminated");
        private static readonly string DEFAULT_HOST = "localhost";
        private static readonly int DEFAULT_PORT = 8194;

        public enum Environment
        {
            PRODUCTION,
            BETA
        }
        private Environment environment;
        private string host;
        private int port;

        public Orders orders;
        public Routes routes;
        public Teams teams;
        public Brokers brokers;
        internal Session session;
        internal SessionOptions sessionOptions;

        internal string emsxServiceName;
        internal string brokerSpecServiceName;
        internal Service emsxService;
        internal Service brokerSpecService;

        internal List<SchemaFieldDefinition> orderFields = new List<SchemaFieldDefinition>();
        internal List<SchemaFieldDefinition> routeFields = new List<SchemaFieldDefinition>();
        Dictionary<CorrelationID, MessageHandler> requestMessageHandlers = new Dictionary<CorrelationID, MessageHandler>();
        Dictionary<CorrelationID, MessageHandler> subscriptionMessageHandlers = new Dictionary<CorrelationID, MessageHandler>();

        internal Team team;

        NotificationHandler globalNotificationHandler = null;
        internal bool orderBlotterInitialized = false;
        internal bool routeBlotterInitialized = false;
        public EasyMSX()
        {

            this.environment = Environment.BETA;
            this.host = DEFAULT_HOST;
            this.port = DEFAULT_PORT;
            initialize();
        }

        public EasyMSX(Environment env)
        {
            this.environment = env;
            this.host = DEFAULT_HOST;
            this.port = DEFAULT_PORT;
            initialize();
        }
        public EasyMSX(Environment env, String host, int port)
        {
            this.environment = env;
            this.host = host;
            this.port = port;
            initialize();
        }


        private void initialize()
        {

            initializeSession();
            initializeService();
            initializeFieldData();
            initializeTeams();
            initializeBrokerData();
            initializeOrders();
            initializeRoutes();

            Log.LogMessage(LogLevels.BASIC, "Message Handlers count: " + requestMessageHandlers.Count);
            while (requestMessageHandlers.Count > 0)
            {
                // wait until all outstanding requests (static loads) have been completed.
                Thread.Sleep(0);
            }
            Log.LogMessage(LogLevels.BASIC, "EMSXAPI initialization complete");
        }
        private void initializeSession()
        {
            if (this.environment == Environment.BETA) emsxServiceName = "//blp/emapisvc_beta";
            else if (this.environment == Environment.PRODUCTION) emsxServiceName = "//blp/emapisvc";
            sessionOptions = new SessionOptions();

            sessionOptions.ServerHost = this.host;
            sessionOptions.ServerPort = this.port;
            Log.LogMessage(LogLevels.BASIC, "Creating Session for " + this.host + ":" + this.port + " environment:" + environment.ToString());
            this.session = new Session(sessionOptions, new EventHandler(processEvent));
            if (!this.session.Start())
            {
                throw new System.Exception("Unable to start session.");
            }
        }
        private void initializeService()
        {
            if (!session.OpenService(emsxServiceName))
            {
                session.Stop();
                throw new System.Exception("Unable to open EMSX service");
            }
            emsxService = session.GetService(emsxServiceName);
        }
        private void initializeFieldData()
        {

            SchemaElementDefinition orderRouteFields = emsxService.GetEventDefinition("OrderRouteFields");
            SchemaTypeDefinition typeDef = orderRouteFields.TypeDefinition;

            for (int i = 0; i < typeDef.NumElementDefinitions; i++)
            {
                SchemaElementDefinition e = typeDef.GetElementDefinition(i);

                String name = e.Name.ToString();

                // Workaround for schema field naming
                if (name.Equals("EMSX_ORD_REF_ID")) name = "EMSX_ORDER_REF_ID";
                // End of Workaround

                SchemaFieldDefinition f = new SchemaFieldDefinition(name);

                f.status = e.Status.ToString();
                f.type = e.TypeDefinition.Datatype.ToString();
                f.min = e.MinValues;
                f.max = e.MaxValues;
                f.description = e.Description;

                Log.LogMessage(LogLevels.DETAILED, "Adding field: " + name);

                if (f.isOrderField()) orderFields.Add(f);
                if (f.isRouteField()) routeFields.Add(f);

            }
        }
        private void initializeTeams()
        {
            teams = new Teams(this);
        }
        internal void setTeam(Team selectedTeam)
        {
            this.team = selectedTeam;
        }
        private void initializeBrokerData()
        {
            brokers = new Brokers(this);
        }

        private void initializeOrders()
        {
            orders = new Orders(this);
        }

        private void initializeRoutes()
        {
            routes = new Routes(this);
        }

        public void start()
        {
            orders.subscribe();
            routes.subscribe();
        }

        public void processEvent(Event evt, Session session)
        {
            switch (evt.Type)
            {
                case EventType.ADMIN:
                    processAdminEvent(evt, session);
                    break;
                case EventType.SESSION_STATUS:
                    processSessionEvent(evt, session);
                    break;
                case EventType.SERVICE_STATUS:
                    processServiceEvent(evt, session);
                    break;
                case EventType.SUBSCRIPTION_DATA:
                    processSubscriptionDataEvent(evt, session);
                    break;
                case EventType.SUBSCRIPTION_STATUS:
                    processSubscriptionStatus(evt, session);
                    break;
                case EventType.RESPONSE:
                    processResponse(evt, session);
                    break;
                default:
                    processMiscEvents(evt, session);
                    break;
            }
        }

        private void processAdminEvent(Event evt, Session session)
        {
            foreach (Message msg in evt)
            {
                if (msg.MessageType.Equals(SLOW_CONSUMER_WARNING))
                {
                    Log.LogMessage(LogLevels.BASIC, "Slow Consumer Warning");
                }
                else if (msg.MessageType.Equals(SLOW_CONSUMER_WARNING_CLEARED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Slow Consumer Warning cleared");
                }
            }
        }
        private void processSessionEvent(Event evt, Session session)
        {
            foreach (Message msg in evt)
            {
                if (msg.MessageType.Equals(SESSION_STARTED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session started");
                }
                else if (msg.MessageType.Equals(SESSION_STARTUP_FAILURE))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session startup failure");
                }
                else if (msg.MessageType.Equals(SESSION_TERMINATED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session terminated");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_UP))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session connection up");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_DOWN))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session connection down");
                }
            }
        }
        private void processServiceEvent(Event evt, Session session)
        {
            foreach (Message msg in evt)
            {
                if (msg.MessageType.Equals(SERVICE_OPENED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Service opened");
                }
                else if (msg.MessageType.Equals(SERVICE_OPEN_FAILURE))
                {
                    Log.LogMessage(LogLevels.BASIC, "Service open failed");
                }
            }
        }
        private void processSubscriptionStatus(Event evt, Session session)
        {
            Log.LogMessage(LogLevels.DETAILED, "Processing SUBSCRIPTION_STATUS event");

            foreach (Message msg in evt)
            {
                CorrelationID cID = msg.CorrelationID;
                if (subscriptionMessageHandlers.ContainsKey(cID))
                {
                    MessageHandler mh = subscriptionMessageHandlers[cID];
                    mh.processMessage(msg);
                }
                else
                {
                    Log.LogMessage(LogLevels.BASIC, "Unexpected SUBSCRIPTION_STATUS event recieved (CID=" + cID.ToString() + "): " + msg.ToString());
                }
            }
        }
        private void processSubscriptionDataEvent(Event evt, Session session)
        {
            Log.LogMessage(LogLevels.DETAILED, "Processing SUBSCRIPTION_DATA event");

            foreach (Message msg in evt)
            {
                CorrelationID cID = msg.CorrelationID;
                if (subscriptionMessageHandlers.ContainsKey(cID))
                {
                    MessageHandler mh = subscriptionMessageHandlers[cID];
                    mh.processMessage(msg);
                }
                else
                {
                    Log.LogMessage(LogLevels.BASIC, "Unexpected SUBSCRIPTION_DATA event recieved (CID=" + cID.ToString() + "): " + msg.ToString());
                }
            }
        }

        private void processResponse(Event evt, Session session)
        {
            Log.LogMessage(LogLevels.DETAILED, "Processing RESPONSE event");

            foreach (Message msg in evt)
            {
                CorrelationID cID = msg.CorrelationID;
                if (requestMessageHandlers.ContainsKey(cID))
                {
                    MessageHandler mh = requestMessageHandlers[cID];
                    mh.processMessage(msg);
                    requestMessageHandlers.Remove(cID);
                    Log.LogMessage(LogLevels.BASIC, "EMSXAPI: MessageHandler removed [" + cID + "]");
                }
                else
                {
                    Log.LogMessage(LogLevels.BASIC, "Unexpected RESPONSE event recieved: " + msg.ToString());
                }
            }
        }
        private void processMiscEvents(Event evt, Session session)
        {
            foreach (Message msg in evt)
            {
                Log.LogMessage(LogLevels.BASIC, "Event: processing misc event: " + msg.MessageType);
            }
        }
        internal void submitRequest(Request request, MessageHandler handler)
        {
            CorrelationID newCID = new CorrelationID();
            Log.LogMessage(LogLevels.BASIC, "EMSXAPI: Submitting request...adding MessageHandler [" + newCID + "]");
            requestMessageHandlers.Add(newCID, handler);

            try
            {
                session.SendRequest(request, newCID);
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.StackTrace);
            }
        }

        public CorrelationID sendRequest(Request request, MessageHandler handler)
        {
            CorrelationID newCID = new CorrelationID();
            Log.LogMessage(LogLevels.BASIC, "EMSXAPI: Send external request...adding MessageHandler [" + newCID + "]");
            requestMessageHandlers.Add(newCID, handler);
            try
            {
                session.SendRequest(request, newCID);
                return newCID;
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.StackTrace);
                return null;
            }
        }

        internal void subscribe(String topic, MessageHandler handler)
        {
            CorrelationID newCID = new CorrelationID();
            subscriptionMessageHandlers.Add(newCID, handler);

            Log.LogMessage(LogLevels.BASIC, "Added Subscription message handler: " + newCID.ToString());

            try
            {
                Subscription sub = new Subscription(topic, newCID);

                List<Subscription> subs = new List<Subscription>();

                subs.Add(sub);

                session.Subscribe(subs);
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.StackTrace);
            }
        }
        public Request createRequest(string requestType)
        {
            return this.emsxService.CreateRequest(requestType);
        }
        public void processNotification(Notification notification)
        {
            if (globalNotificationHandler != null && !notification.consume) globalNotificationHandler.processNotification(notification);
        }
    }
}