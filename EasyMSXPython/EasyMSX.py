# EasyMSX.py
from __future__ import print_function
import blpapi
import sys
from SchemaFieldDefinition import SchemaFieldDefinition
from Teams import Teams
from Brokers import Brokers
from Orders import Orders
from Routes import Routes

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
        def asText(self, v):
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
        def asText(self,v):
            return (self.__list[v])

    nextCorId=1
        
    def __init__(self, env=Environment.BETA, host="localhost",port=8194):
        self.env = env
        self.host = host
        self.port = port
 
        self.notificationHandlers=[]
        self.requestMessageHandlers={}
        self.subscriptionMessageHandlers={}
        self.orderFields=[]
        self.routeFields=[]
        self.emsxServiceName=""
        self.teams = None

        self.team=None
        
        self.initialize()

        self.orders = Orders(self)
        self.routes = Routes(self)
        
    def initialize(self):
        
        self.initializeSession()
        self.initializeService()
        self.initializeFieldData()
        self.initializeTeams()
        self.initializeBrokerData()
        
        while len(self.requestMessageHandlers) > 0:
            pass

    def initializeSession(self):
        if self.env == self.Environment.BETA:  
            self.emsxServiceName = "//blp/emapisvc_beta"
        elif self.env == self.Environment.PRODUCTION:
            self.emsxServiceName = "//blp/emapisvc"
            
        self.sessionOptions = blpapi.SessionOptions() 

        self.sessionOptions.setServerHost(self.host)
        self.sessionOptions.setServerPort(self.port)
                
        self.session = blpapi.Session(options=self.sessionOptions, eventHandler=self.processEvent)
        #self.session = blpapi.Session(options=self.sessionOptions)
        
        if not self.session.start():
            raise Exception("Failed to start session.")
            return
    
    def initializeService(self):
        if not self.session.openService(self.emsxServiceName):
            self.session.stop()
            raise Exception("Unable to open EMSX service")

        self.emsxService = self.session.getService(self.emsxServiceName)
        
    def initializeFieldData(self):
        
#        print("Initializing field data...")

        self.orderRouteFields = self.emsxService.getEventDefinition("OrderRouteFields");
        typeDef = self.orderRouteFields.typeDefinition()
        
#        print("Total number of fields: %d" % (typeDef.numElementDefinitions()))
        
        for i in range(0, typeDef.numElementDefinitions()):
            
            e = typeDef.getElementDefinition(i)
            
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
            
            if f.isOrderField(): 
                self.orderFields.append(f)
                #print("Added order field: " + f.name)
            if f.isRouteField(): 
                self.routeFields.append(f)
                #print("Added route field: " + f.name)

            
            #print("Adding field: " + f.name + "\tStatus: " + str(f.status) + "\tType: " + f.type)
            
    def initializeTeams(self):
        self.teams = Teams(self)
        
    def initializeBrokerData(self):
        self.brokers = Brokers(self)
        
    def start(self):
        self.initializeOrders()
        self.initializeRoutes()
        
    def initializeOrders(self):
        self.orders.subscribe()
        
    def initializeRoutes(self):
        self.routes.subscribe()
        
    def setTeam(self, selectedTeam):
        self.team = selectedTeam
        
    def submitRequest(self,req,messageHandler):
        try:
            cID = self.session.sendRequest(request=req)
            self.requestMessageHandlers[cID.value()] = messageHandler
#            print("Request submitted (" + str(cID)  + "): \n" + str(req))
                
        except Exception as err:
            print("EasyMSX >>  Error sending request: " + str(err))
            

    def subscribe(self, topic, messageHandler):
        try:
            self.nextCorId+=1
            cID = blpapi.CorrelationId(self.nextCorId)
            subscriptions = blpapi.SubscriptionList()
            subscriptions.add(topic=topic, correlationId=cID)
            self.session.subscribe(subscriptions)
            self.subscriptionMessageHandlers[cID.value()] = messageHandler
            #print("Request submitted (" + str(cID)  + "): \n" + str(topic))
            
        except Exception as err:
            print("EasyMSX >>  Error subscribing to topic: " + str(err))
            

    def processEvent(self, event, session):
        
#        print("Processing Event (" + str(event.eventType()) + ")")
        
        if event.eventType() == blpapi.Event.ADMIN:
            self.processAdminEvent(event, session)

        elif event.eventType() == blpapi.Event.SESSION_STATUS:
            self.processSessionStatusEvent(event,session)
       
        elif event.eventType() == blpapi.Event.SERVICE_STATUS:
            self.processServiceStatusEvent(event,session)

        elif event.eventType() == blpapi.Event.SUBSCRIPTION_DATA:
            self.processSubscriptionDataEvent(event,session)

        elif event.eventType() == blpapi.Event.SUBSCRIPTION_STATUS:
            self.processSubscriptionStatusEvent(event,session)

        elif event.eventType() == blpapi.Event.RESPONSE:
            self.processResponseEvent(event, session)
        
        else:
            self.processMiscEvents(event,session)
           
        return False

    def processAdminEvent(self,event,session):
        
#        print("Processing ADMIN event...")

        for msg in event:
            if msg.messageType() == SLOW_CONSUMER_WARNING:
                print("Slow Consumer Warning")
            elif msg.messageType() == SLOW_CONSUMER_WARNING_CLEARED:
                print("Slow Consumer Warning cleared")
        

    def processSessionStatusEvent(self,event,session):

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
        

    def processServiceStatusEvent(self,event,session):
        
 #       print("Processing SERVICE_STATUS event...")

        for msg in event:
            if msg.messageType() == SERVICE_OPENED:
#                print("Service Opened")
                pass
            elif msg.messageType() == SERVICE_OPEN_FAILURE:
                print("Service Open Failure")

        
    def processSubscriptionDataEvent(self,event,session):
        
#        print("Processing SUBSCRIPTION_DATA event...")

        for msg in event:
            cID = msg.correlationIds()[0].value()
            if cID in self.subscriptionMessageHandlers:
                self.subscriptionMessageHandlers[cID](msg)
            else:
                print("Unrecognised correlation ID in subscription data event. No event handler can be found for cID: " + str(cID))
        
        
    def processSubscriptionStatusEvent(self,event,session):
        
#        print("Processing SUBSCRIPTION_STATUS event...")

        for msg in event:
            cID = msg.correlationIds()[0].value()
            if cID in self.subscriptionMessageHandlers:
                self.subscriptionMessageHandlers[cID](msg)
            else:
                print("Unrecognised correlation ID in subscription status event. No event handler can be found for cID: " + str(cID)) 
        

    def processResponseEvent(self,event,session):
        
#        print("Processing RESPONSE event...")
        
        for msg in event:
            cID = msg.correlationIds()[0].value()
#            print("Received cID: " + str(cID))
            if cID in self.requestMessageHandlers:
                handler = self.requestMessageHandlers[cID]
                handler(msg)
                del self.requestMessageHandlers[cID]
            else:
                print("Unrecognised correlation ID in response event. No event handler can be found for cID: " + str(cID)) 
        

    def processMiscEvents(self,event,session):
        
#        print("Processing unknown event...")

        for msg in event:
            print("Misc Event: " + msg)

    def addNotificationHandler(self,handler):
        self.notificationHandlers.append(handler)
        
    def notify(self, notification):
        for h in self.notificationHandlers:
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
