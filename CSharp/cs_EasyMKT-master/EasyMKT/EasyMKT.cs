/* Copyright 2017. Bloomberg Finance L.P.
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
using LogLevels = com.bloomberg.mktdata.samples.Log.LogLevels;
using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Subscription = Bloomberglp.Blpapi.Subscription;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using static Bloomberglp.Blpapi.Event;

namespace com.bloomberg.mktdata.samples
{
    public class EasyMKT
    {

        // ADMIN
        private static readonly Name   SLOW_CONSUMER_WARNING	        = new Name("SlowConsumerWarning");
        private static readonly Name   SLOW_CONSUMER_WARNING_CLEARED	= new Name("SlowConsumerWarningCleared");

        // SESSION_STATUS
        private static readonly Name   SESSION_STARTED 		    = new Name("SessionStarted");
        private static readonly Name   SESSION_TERMINATED 		= new Name("SessionTerminated");
        private static readonly Name   SESSION_STARTUP_FAILURE  = new Name("SessionStartupFailure");
        private static readonly Name   SESSION_CONNECTION_UP 	= new Name("SessionConnectionUp");
        private static readonly Name   SESSION_CONNECTION_DOWN	= new Name("SessionConnectionDown");

        // SERVICE_STATUS
        private static readonly Name   SERVICE_OPENED 			= new Name("ServiceOpened");
        private static readonly Name   SERVICE_OPEN_FAILURE 	= new Name("ServiceOpenFailure");

        // SUBSCRIPTION_STATUS + SUBSCRIPTION_DATA
        private static readonly Name   SUBSCRIPTION_FAILURE 	= new Name("SubscriptionFailure");
        private static readonly Name   SUBSCRIPTION_STARTED	    = new Name("SubscriptionStarted");
        private static readonly Name   SUBSCRIPTION_TERMINATED	= new Name("SubscriptionTerminated");

        public Securities securities;
        public SubscriptionFields fields;

        private String host;
        private int port;

        Session session;
        Service service;

        Dictionary<CorrelationID, MessageHandler> messageHandlers = new Dictionary<CorrelationID, MessageHandler>();

        private volatile bool ready = false;

        private static readonly String MKTDATA_SERVICE = "//blp/mktdata";
	
	    public EasyMKT()
        {
            this.host = "localhost";
            this.port = 8194;
            initialise();
        }

        public EasyMKT(String host, int port)
        {
            this.host = host;
            this.port = port;
            initialise();
        }

        private void initialise()
        {

            fields = new SubscriptionFields(this);
            securities = new Securities(this);

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

        public SubscriptionField AddField(String fieldName)
        {

            return fields.CreateSubscriptionField(fieldName);
        }

        public Security AddSecurity(String ticker)
        {

            return securities.createSecurity(ticker);
        }

        public void start()
        {
            foreach (Security s in securities)
            {
                this.AddSubscription(s);
            }

        }

        public void processEvent(Event evt, Session session) {

            switch (evt.Type) {
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
                default:
                    processMiscEvents(evt, session);
                    break;
            }
        }


        private void processAdminEvent(Event evt, Session session) {
            foreach (Message msg in evt) {
                if (msg.MessageType.Equals(SLOW_CONSUMER_WARNING)) {
                    Log.LogMessage(LogLevels.BASIC, "Slow Consumer Warning");
                }
                else if (msg.MessageType.Equals(SLOW_CONSUMER_WARNING_CLEARED)) {
                    Log.LogMessage(LogLevels.BASIC, "Slow Consumer Warning cleared");
                }
            }
        }

        private void processSessionEvent(Event evt, Session session) {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt) {

                if (msg.MessageType.Equals(SESSION_STARTED)) {
                    Log.LogMessage(LogLevels.BASIC, "Session started...");
                    session.OpenServiceAsync(MKTDATA_SERVICE);
                }
                else if (msg.MessageType.Equals(SESSION_STARTUP_FAILURE)) {
                    Log.LogMessage(LogLevels.BASIC, "Error: Session startup failed");
                }
                else if (msg.MessageType.Equals(SESSION_TERMINATED)) {
                    Log.LogMessage(LogLevels.BASIC, "Session has been terminated");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_UP)) {
                    Log.LogMessage(LogLevels.BASIC, "Session connection is up");
                }
                else if (msg.MessageType.Equals(SESSION_CONNECTION_DOWN)) {
                    Log.LogMessage(LogLevels.BASIC, "Session connection is down");
                }
            }
        }

        private void processServiceEvent(Event evt, Session session) {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt) {

                if (msg.MessageType.Equals(SERVICE_OPENED)) {

                    Log.LogMessage(LogLevels.BASIC, "Service opened...");

                    this.service = session.GetService(MKTDATA_SERVICE);

                    Log.LogMessage(LogLevels.BASIC, "Got service...ready...");

                    this.ready = true;
                }
                else if (msg.MessageType.Equals(SERVICE_OPEN_FAILURE)) {
                    Log.LogMessage(LogLevels.BASIC, "Error: Service failed to open");
                }
            }
        }

        private void processSubscriptionStatus(Event evt, Session session) {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt) {
                if (msg.MessageType.Equals(SUBSCRIPTION_STARTED)) {
                    Log.LogMessage(LogLevels.BASIC, "Subscription started successfully: " + msg.CorrelationID.ToString());
                }
                else if (msg.MessageType.Equals(SUBSCRIPTION_FAILURE)) {
                    Log.LogMessage(LogLevels.BASIC, "Error: Subscription failed: " + msg.CorrelationID.ToString());
                }
                else if (msg.MessageType.Equals(SUBSCRIPTION_TERMINATED)) {
                    Log.LogMessage(LogLevels.BASIC, "Subscription terminated : " + msg.CorrelationID.ToString());
                }
            }
        }

        private void processSubscriptionDataEvent(Event evt, Session session) {

            Log.LogMessage(LogLevels.DETAILED, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt) {
                // process the incoming market data event
                messageHandlers[msg.CorrelationID].handleMessage(msg);
            }
        }

        private void processMiscEvents(Event evt, Session session) {

            Log.LogMessage(LogLevels.BASIC, "Processing " + evt.Type.ToString());

            foreach (Message msg in evt) {
                Log.LogMessage(LogLevels.BASIC, "MESSAGE: " + msg);
            }
        }

	    public void AddSubscription(Security security) {

            Log.LogMessage(LogLevels.DETAILED, "Adding subscription for security: " + security.GetName());

            CorrelationID cID = new CorrelationID(security.GetName());

            Subscription newSubscription = new Subscription(security.GetName(), fields.GetFieldList(), "", cID);

            Log.LogMessage(LogLevels.DETAILED, "Topic string: " + newSubscription.SubscriptionString);

            List<Subscription> newSubList = new List<Subscription>();

            newSubList.Add(newSubscription);

            messageHandlers.Add(cID, security);

            try {
                Log.LogMessage(LogLevels.DETAILED, "Subscribing...");
                this.session.Subscribe(newSubList);
                Log.LogMessage(LogLevels.DETAILED, "Subscription request sent...");
            }
            catch (Exception ex) {
                Log.LogMessage(LogLevels.BASIC, "Failed to subscribe: " + newSubList.ToString());
                Console.WriteLine(ex.ToString());
            }
        }	
    }
}
