/* Copyright 2018. Bloomberg Finance L.P.
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
using Name = Bloomberglp.Blpapi.Name;
using Session = Bloomberglp.Blpapi.Session;
using Service = Bloomberglp.Blpapi.Service;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;
using CorrelationID = Bloomberglp.Blpapi.CorrelationID;
using LogLevels = com.bloomberg.ioiapi.samples.Log.LogLevels;
using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Subscription = Bloomberglp.Blpapi.Subscription;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using Request = Bloomberglp.Blpapi.Request;

namespace com.bloomberg.ioiapi.samples
{
    public class EasyIOI
    {

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

        public IOIs iois;

        private String host;
        private int port;

        Session session;
        Service ioiSubService;
        Service ioiReqService;

        Dictionary<CorrelationID, MessageHandler> subscriptionMessageHandlers = new Dictionary<CorrelationID, MessageHandler>();
        Dictionary<CorrelationID, MessageHandler> requestMessageHandlers = new Dictionary<CorrelationID, MessageHandler>();

        private volatile bool ready = false;
        private volatile bool svcOpened = false;

        private static readonly String IOISUBDATA_SERVICE = "//blp/ioisub-beta";
        private static readonly String IOIREQDATA_SERVICE = "//blp/ioiapi-beta-request";

        public EasyIOI()
        {
            this.host = "localhost";
            this.port = 8194;
            initialise();
        }

        public EasyIOI(String host, int port)
        {
            this.host = host;
            this.port = port;
            initialise();
        }

        private void initialise()
        {

            Log.logPrefix = "EasyIOI";

            this.iois = new IOIs(this);

            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = this.host;
            sessionOptions.ServerPort = this.port;

            this.session = new Session(sessionOptions, new EventHandler(processEvent));

            try
            {
                this.session.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            while (!this.ready) ;

        }

        public void start()
        {
            CreateIOISubscription();
        }

        public void processEvent(Event evt, Session session)
        {

            switch (evt.Type)
            {
                case Event.EventType.ADMIN:
                    processAdminEvent(evt, session);
                    break;
                case Event.EventType.SESSION_STATUS:
                    processSessionEvent(evt, session);
                    break;
                case Event.EventType.SERVICE_STATUS:
                    processServiceEvent(evt, session);
                    break;
                case Event.EventType.SUBSCRIPTION_DATA:
                    processSubscriptionDataEvent(evt, session);
                    break;
                case Event.EventType.SUBSCRIPTION_STATUS:
                    processSubscriptionStatus(evt, session);
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

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt)
            {

                if (msg.MessageType.Equals(SESSION_STARTED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session started...");
                    session.OpenServiceAsync(IOISUBDATA_SERVICE);
                    session.OpenServiceAsync(IOIREQDATA_SERVICE);
                }
                else if (msg.MessageType.Equals(SESSION_STARTUP_FAILURE))
                {
                    Log.LogMessage(LogLevels.BASIC, "Error: Session startup failed");
                }
                else if (msg.MessageType.Equals(SESSION_TERMINATED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session has been terminated");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_UP))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session connection is up");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_DOWN))
                {
                    Log.LogMessage(LogLevels.BASIC, "Session connection is down");
                }
            }
        }

        private void processServiceEvent(Event evt, Session session)
        {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt)
            {

                if (msg.MessageType.Equals(SERVICE_OPENED))
                {

                    String svc = msg.GetElementAsString("serviceName");
                    if (svc == IOISUBDATA_SERVICE)
                    {
                        Log.LogMessage(LogLevels.BASIC, "IOI Subscription Service opened");
                        this.ioiSubService = session.GetService(IOISUBDATA_SERVICE);
                        Log.LogMessage(LogLevels.BASIC, "Got IOI Subscription service...ready");
                        if (svcOpened) this.ready = true;
                        else svcOpened = true;
                    }
                    else if (svc == IOIREQDATA_SERVICE)
                    {
                        Log.LogMessage(LogLevels.BASIC, "IOI Request Service opened");
                        this.ioiReqService = session.GetService(IOIREQDATA_SERVICE);
                        Log.LogMessage(LogLevels.BASIC, "Got IOI Request service...ready");
                        if (svcOpened) this.ready = true;
                        else svcOpened = true;
                    }
                }
                else if (msg.MessageType.Equals(SERVICE_OPEN_FAILURE))
                {
                    Log.LogMessage(LogLevels.BASIC, "Error: Service failed to open");
                }
            }
        }

        private void processSubscriptionStatus(Event evt, Session session)
        {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt)
            {
                if (msg.MessageType.Equals(SUBSCRIPTION_STARTED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Subscription started successfully: " + msg.CorrelationID.ToString());
                }
                else if (msg.MessageType.Equals(SUBSCRIPTION_FAILURE))
                {
                    Log.LogMessage(LogLevels.BASIC, "Error: Subscription failed: " + msg.CorrelationID.ToString());
                }
                else if (msg.MessageType.Equals(SUBSCRIPTION_TERMINATED))
                {
                    Log.LogMessage(LogLevels.BASIC, "Subscription terminated : " + msg.CorrelationID.ToString());
                }
            }
        }

        private void processSubscriptionDataEvent(Event evt, Session session)
        {

            Log.LogMessage(LogLevels.DETAILED, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt)
            {

                Log.LogMessage(LogLevels.DETAILED, "Looking for handler : " + msg.CorrelationID.ToString());

                if (subscriptionMessageHandlers.ContainsKey(msg.CorrelationID))
                {
                    Log.LogMessage(LogLevels.DETAILED, "Message handler found: " + msg.CorrelationID.ToString());
                    // process the incoming market data event
                    subscriptionMessageHandlers[msg.CorrelationID].handleMessage(msg);
                } else
                {
                    Log.LogMessage(LogLevels.DETAILED, "Failed to find message handler for: " + msg.CorrelationID.ToString());
                }
            }
        }

        private void processMiscEvents(Event evt, Session session)
        {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt)
            {
                Log.LogMessage(LogLevels.BASIC, "MESSAGE: " + msg);
            }
        }

        public void CreateIOISubscription()
        {

            Log.LogMessage(LogLevels.DETAILED, "Create IOI Subscription");

            CorrelationID cID = new CorrelationID();

            Subscription newSubscription = new Subscription(IOISUBDATA_SERVICE + "/ioi", cID);

            Log.LogMessage(LogLevels.DETAILED, "Topic string: " + newSubscription.SubscriptionString);

            List<Subscription> newSubList = new List<Subscription>();

            newSubList.Add(newSubscription);

            subscriptionMessageHandlers.Add(cID, this.iois);

            Log.LogMessage(LogLevels.DETAILED, "Added new IOIs message handler: " + cID.ToString());


            try
            {
                Log.LogMessage(LogLevels.DETAILED, "Subscribing...");
                this.session.Subscribe(newSubList);
                Log.LogMessage(LogLevels.DETAILED, "Subscription request sent...");
            }
            catch (Exception ex)
            {
                Log.LogMessage(LogLevels.BASIC, "Failed to subscribe: " + newSubList.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        public CorrelationID sendRequest(Request request, MessageHandler handler)
        {
            CorrelationID newCID = new CorrelationID();
            Log.LogMessage(LogLevels.BASIC, "EMSXAPI: Send external refdata request...adding MessageHandler [" + newCID + "]");
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

        public Request createRequest(string requestType)
        {
            return this.ioiReqService.CreateRequest(requestType);
        }
    }
}
