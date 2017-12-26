# easymsx.py

import blpapi
from .schemafielddefinition import SchemaFieldDefinition
from .teams import Teams
from .brokers import Brokers
from .orders import Orders
from .routes import Routes

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



class EasyMSX:
    
    class Environment:
        PRODUCTION=0
        BETA=1
        
    class NotificationCategory:
        ORDER=0
        ROUTE=1
        ADMIN=2
        
        __list = ["ORDER", "ROUTE", "ADMIN"]
        @classmethod
        def as_text(self, v):
            return (self.__list[v])
        
    class NotificationType:
        NEW=0
        INITIALPAINT=1
        UPDATE=2
        DELETE=3
        CANCEL=4
        ERROR=5
 
        __list = ["NEW","INITIALPAINT","UPDATE","DELETE","CANCEL","ERROR"]       
        @classmethod
        def as_text(self,v):
            return (self.__list[v])

    next_cor_id=1
        
    def __init__(self, env=Environment.BETA, host="localhost",port=8194):
        self.env = env
        self.host = host
        self.port = port
 
        self.notification_handlers=[]
        self.request_message_handlers={}
        self.subscription_message_handlers={}
        self.order_fields=[]
        self.route_fields=[]
        self.emsx_service_name=""
        self.teams = None

        self.team=None
        
        self.initialize()

        self.orders = Orders(self)
        self.routes = Routes(self)
        
    def initialize(self):
        
        self.initialize_session()
        self.initialize_service()
        self.initialize_field_data()
        self.initialize_teams()
        self.initialize_broker_data()
        
        while len(self.request_message_handlers) > 0:
            pass

    def initialize_session(self):
        if self.env == self.Environment.BETA:  
            self.emsx_service_name = "//blp/emapisvc_beta"
        elif self.env == self.Environment.PRODUCTION:
            self.emsx_service_name = "//blp/emapisvc"
            
        self.session_options = blpapi.SessionOptions() 

        self.session_options.setServerHost(self.host)
        self.session_options.setServerPort(self.port)
                
        self.session = blpapi.Session(options=self.session_options, eventHandler=self.process_event)
        
        if not self.session.start():
            raise ("Failed to start session.")
            return
    
    def initialize_service(self):
        if not self.session.openService(self.emsx_service_name):
            self.session.stop()
            raise ("Unable to open EMSX service")

        self.emsx_service = self.session.getService(self.emsx_service_name)
        
    def initialize_field_data(self):
        
#        print("Initializing field data...")

        self.order_route_fields = self.emsx_service.getEventDefinition("OrderRouteFields");
        type_def = self.order_route_fields.typeDefinition()
        
#        print("Total number of fields: %d" % (type_def.numElementDefinitions()))
        
        for i in range(0, type_def.numElementDefinitions()):
            
            e = type_def.getElementDefinition(i)
            
            name=str(e.name())
            
            # Workaround for schema field naming
            if name=="EMSX_ORD_REF_ID": name = "EMSX_ORDER_REF_ID"
            # End of Workaround
            
            f = SchemaFieldDefinition(name)
            
            f.status = e.status()
            f.type = e.typeDefinition().description()
            f.min = e.minValues()
            f.max = e.maxValues()
            f.description = e.description()
            
            if f.is_order_field(): 
                self.order_fields.append(f)
#                print("Added order field: " + f.name)
            if f.is_route_field(): 
                self.route_fields.append(f)
#                print("Added route field: " + f.name)

            
#            print("Adding field: " + f.name + "\tStatus: " + str(f.status) + "\tType: " + f.type)
            
    def initialize_teams(self):
        self.teams = Teams(self)
        
    def initialize_broker_data(self):
        self.brokers = Brokers(self)
        
    def start(self):
        self.initialize_orders()
        self.initialize_routes()
        
    def initialize_orders(self):
        self.orders.subscribe()
        
    def initialize_routes(self):
        self.routes.subscribe()
        
    def set_team(self, selected_team):
        self.team = selected_team
        
    def submit_request(self,req,message_handler):
        try:
            cID = self.session.sendRequest(request=req)
            self.request_message_handlers[cID.value()] = message_handler
#            print("Request submitted (" + str(cID)  + "): \n" + str(req))
                
        except Exception as err:
            print("EasyMSX >>  Error sending request: " + str(err))
            

    def subscribe(self, topic, message_handler):
        try:
            self.next_cor_id+=1
            c_id = blpapi.CorrelationId(self.next_cor_id)
            subscriptions = blpapi.SubscriptionList()
            subscriptions.add(topic=topic, correlationId=c_id)
            self.session.subscribe(subscriptions)
            self.subscription_message_handlers[c_id.value()] = message_handler
#            print("Request submitted (" + str(cID)  + "): \n" + str(topic))
            
        except Exception as err:
            print("EasyMSX >>  Error subscribing to topic: " + str(err))
            

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
#                print("Session Started")
                pass
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
#                print("Service Opened")
                pass
            elif msg.messageType() == SERVICE_OPEN_FAILURE:
                print("Service Open Failure")

        
    def process_subscription_data_event(self,event,session):
        
#        print("Processing SUBSCRIPTION_DATA event...")

        for msg in event:
            c_id = msg.correlationIds()[0].value()
            if c_id in self.subscription_message_handlers:
                self.subscription_message_handlers[c_id](msg)
            else:
                print("Unrecognised correlation ID in subscription data event. No event handler can be found for cID: " + str(c_id))
        
        
    def process_subscription_status_event(self,event,session):
        
#        print("Processing SUBSCRIPTION_STATUS event...")

        for msg in event:
            c_id = msg.correlationIds()[0].value()
            if c_id in self.subscription_message_handlers:
                self.subscription_message_handlers[c_id](msg)
            else:
                print("Unrecognised correlation ID in subscription status event. No event handler can be found for cID: " + str(c_id)) 
        

    def process_response_event(self,event,session):
        
#        print("Processing RESPONSE event...")
        
        for msg in event:
            c_id = msg.correlationIds()[0].value()
#            print("Received cID: " + str(c_id))
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

__copyright__ = """
Copyright 2017. Bloomberg Finance L.P.

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
