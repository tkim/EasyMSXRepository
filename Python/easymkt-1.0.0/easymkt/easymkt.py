# easymkt.py

import blpapi

from .security import Security

# ADMIN
SLOW_CONSUMER_WARNING           = blpapi.Name("SlowConsumerWarning")
SLOW_CONSUMER_WARNING_CLEARED   = blpapi.Name("SlowConsumerWarningCleared")

# SESSION_STATUS
SESSION_STARTED                 = blpapi.Name("SessionStarted")
SESSION_TERMINATED              = blpapi.Name("SessionTerminated")
SESSION_STARTUP_FAILURE         = blpapi.Name("SessionStartupFailure")
SESSION_CONNECTION_UP           = blpapi.Name("SessionConnectionUp")
SESSION_CONNECTION_DOWN         = blpapi.Name("SessionConnectionDown")

# SERVICE_STATUS
SERVICE_OPENED                  = blpapi.Name("ServiceOpened")
SERVICE_OPEN_FAILURE            = blpapi.Name("ServiceOpenFailure")

# SUBSCRIPTION_STATUS + SUBSCRIPTION_DATA
SUBSCRIPTION_FAILURE            = blpapi.Name("SubscriptionFailure")
SUBSCRIPTION_STARTED            = blpapi.Name("SubscriptionStarted")
SUBSCRIPTION_TERMINATED         = blpapi.Name("SubscriptionTerminated")


class EasyMKT:
    
    next_cor_id=1
        
    def __init__(self, host="localhost",port=8194):
        self.host = host
        self.port = port
 
        self.ready = False
        self.svc_opened = False
        self.request_message_handlers={}
        self.subscription_message_handlers={}
        self.notification_handlers=[]
        self.mktdata_service_name="//blp/mktdata"
        self.refdata_service_name="//blp/refdata"
        
        self.initialize()
        
    def initialize(self):
        
        self.securities=[]
        self.subscription_fields=[]
        
        self.session_options = blpapi.SessionOptions() 

        self.session_options.setServerHost(self.host)
        self.session_options.setServerPort(self.port)
                
        self.session = blpapi.Session(options=self.session_options, eventHandler=self.process_event)
        
        if not self.session.start():
            raise ("Failed to start session.")
            return

    def add_field(self, field_name):
        self.subscription_fields.append(field_name)
        
    def add_security(self, ticker):
        new_sec = Security(self, ticker)
        self.securities.append(new_sec)
        return new_sec

    def start(self):
        
        for s in self.securities:
            self.add_subscription(s)
            
    def submit_request(self,req,message_handler):
        try:
            cID = self.session.sendRequest(request=req)
            self.request_message_handlers[cID.value()] = message_handler
#            print("Request submitted (" + str(cID)  + "): \n" + str(req))
                
        except Exception as err:
            print("EasyMSX >>  Error sending request: " + str(err))
            

    def add_subscription(self, security):
        try:
            self.next_cor_id+=1
            c_id = blpapi.CorrelationId(self.next_cor_id)
            subscriptions = blpapi.SubscriptionList()
            flds = ""
            for s in self.subscription_fields:
                flds += s + ","
            subscriptions.add(security.name, flds[:-1], "", c_id)
            self.session.subscribe(subscriptions)
            self.subscription_message_handlers[c_id.value()] = security.process_message
            #print("Request submitted (" + str(c_id.value())  + "): " + security.name + "/" +str(flds[:-1])+"\n")
            
        except Exception as err:
            print("EasyMKT >>  Error subscribing to topic: " + str(err))
            

    def process_event(self, event, session):
        
#        print("Processing Event (" + str(event.eventType()) + ")")
        
        if event.eventType() == blpapi.Event.ADMIN:
            self.process_admin_event(event, session)

        elif event.eventType() == blpapi.Event.SESSION_STATUS:
            self.process_session_status_event(event,session)
       
        elif event.eventType() == blpapi.Event.SERVICE_STATUS:
            self.process_service_status_event(event,session)

        elif event.eventType() == blpapi.Event.SUBSCRIPTION_DATA:
            self.process_subscription_data_event(event,session)

        elif event.eventType() == blpapi.Event.SUBSCRIPTION_STATUS:
            self.process_subscription_status_event(event,session)

        elif event.eventType() == blpapi.Event.RESPONSE:
            self.process_response_event(event, session)
        
        else:
            self.process_misc_events(event,session)
           
        return False

    def process_admin_event(self,event,session):
        
#        print("Processing ADMIN event...")

        for msg in event:
            if msg.messageType() == SLOW_CONSUMER_WARNING:
                print("Slow Consumer Warning")
            elif msg.messageType() == SLOW_CONSUMER_WARNING_CLEARED:
                print("Slow Consumer Warning cleared")
        

    def process_session_status_event(self,event,session):

#        print("Processing SESSION_STATUS event...")

        for msg in event:
            if msg.messageType() == SESSION_STARTED:
                session.openServiceAsync(self.mktdata_service_name)
                session.openServiceAsync(self.refdata_service_name)
            elif msg.messageType() == SESSION_STARTUP_FAILURE:
                print("Session Startup Failure")
            elif msg.messageType() == SESSION_TERMINATED:
#                print("Session Terminated")
                pass
            elif msg.messageType() == SESSION_CONNECTION_UP:
#                print("Session Connection Up")
                pass
            elif msg.messageType() == SESSION_CONNECTION_DOWN:
#                print("Session Connection Down")
                pass
        

    def process_service_status_event(self,event,session):
        
#       print("Processing SERVICE_STATUS event...")

        for msg in event:
            if msg.messageType() == SERVICE_OPENED:
                svc = msg.getElementAsString("serviceName")
                if svc== self.mktdata_service_name:
                    self.mktdata = session.getService(self.mktdata_service_name)
                elif svc == self.refdata_service_name:
                    self.refdata = session.getService(self.refdata_service_name)
                if self.svc_opened:
                    self.ready=True
                else:
                    self.svc_opened = True
                    
            elif msg.messageType() == SERVICE_OPEN_FAILURE:
                print("Service Open Failure")

        
    def process_subscription_status_event(self,event,session):
        
#        print("Processing SUBSCRIPTION_STATUS event...")

        for msg in event:
            if msg.messageType() == SUBSCRIPTION_STARTED:
                pass #Log event
            elif msg.messageType() == SUBSCRIPTION_FAILURE:
                print("Error: subscription failure" + msg.correlationIds()[0].value())
            elif msg.messageType() == SUBSCRIPTION_TERMINATED:
                pass #log event
                
    def process_subscription_data_event(self,event,session):
        
#        print("Processing SUBSCRIPTION_DATA event...")

        for msg in event:
            c_id = msg.correlationIds()[0].value()
            if c_id in self.subscription_message_handlers:
                self.subscription_message_handlers[c_id](msg)
            else:
                print("Unrecognised correlation ID in subscription data event. No event handler can be found for cID: " + str(c_id))
        
        
    def process_response_event(self,event,session):
        
#        print("Processing RESPONSE event...")
        
        for msg in event:
            c_id = msg.correlationIds()[0].value()
            if c_id in self.request_message_handlers:
                handler = self.request_message_handlers[c_id]
                handler(msg)
                del self.request_message_handlers[c_id]
            else:
                print("Unrecognised correlation ID in response event. No event handler can be found for cID: " + str(c_id)) 
        

    def process_misc_events(self,event,session):
        
#        print("Processing unknown event...")

        for msg in event:
            print("Misc Event: " + msg)

    def add_notification_handler(self,handler):
        self.notification_handlers.append(handler)
        
    def notify(self, notification):
        for h in self.notification_handlers:
            if not notification.consumed: 
                h(notification)

    def stop(self):
        self.session.stop()

__copyright__ = """
Copyright 2018. Bloomberg Finance L.P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  The above
copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
"""
