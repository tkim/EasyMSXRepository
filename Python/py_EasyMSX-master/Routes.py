# Routes.py

import blpapi
from Route import Route
from Notification import *

SUBSCRIPTION_STARTED            = blpapi.Name("SubscriptionStarted")

class Routes:

    def __init__(self,easyMSX):
        self.easyMSX = easyMSX
        self.routes=[]
        self.fieldSource = self.easyMSX.routeFields
        self.notificationHandlers=[]
        self.initialized=False
        
    def __iter__(self):
        return self.routes.__iter__()

    def subscribe(self):
        
        routeTopic = self.easyMSX.emsxServiceName + "/route"
        if self.easyMSX.team != None: 
            routeTopic += ";team=" + self.easyMSX.team.name
        routeTopic += "?fields="   
        for f in self.fieldSource:
            routeTopic += f.name + ","
            
        routeTopic = routeTopic[:-1] # truncate the trailing comma character
        
        self.easyMSX.subscribe(routeTopic, self.processMessage)
        
        while not self.initialized:
            pass
        
    def createRoute(self, seqNo, routeId):
        r = Route(self)
        r.sequence = seqNo
        r.routeId = routeId
        self.routes.append(r)
        return r
    
    def getBySequenceNoAndId(self, seqNo, routeId):
        for r in self.routes:
            if r.sequence == seqNo and r.routeId == routeId:
                return r
        return None
    
    def processMessage(self, msg):

        if msg.messageType() == SUBSCRIPTION_STARTED:
#            print("Route Subscription Started...")
            return

        eventStatus = msg.getElementAsInteger("EVENT_STATUS")
        
        if eventStatus==1:      # Heartbeat
#            print("Route >> Heartbeat")
            pass
        
        elif eventStatus==4:    # Initial paint
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            routeId = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seqNo) + "\tRouteId: " + str(routeId))
            r = self.getBySequenceNoAndId(seqNo, routeId)

            if r is None:
                r = self.createRoute(seqNo, routeId)
        
            r.fields.populateFields(msg, False)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.INITIALPAINT, r, r.fields.getFieldChanges()))                     
        
        elif eventStatus==6:    # New order
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            routeId = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seqNo) + "\tRouteId: " + str(routeId))
            r = self.getBySequenceNoAndId(seqNo, routeId)
        
            if r is None:
                r = self.createRoute(seqNo, routeId)
        
            r.fields.populateFields(msg, False)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.NEW, r, r.fields.getFieldChanges()))                     
        
        elif eventStatus==7:    # Update order
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            routeId = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seqNo) + "\tRouteId: " + str(routeId))
            r = self.getBySequenceNoAndId(seqNo, routeId)
        
            if r is None:
#                print("WARNING >> update received for unknown order")
                r = self.createRoute(seqNo, routeId)
        
            r.fields.populateFields(msg, True)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.UPDATE, r, r.fields.getFieldChanges()))                     

        elif eventStatus==8:    # Delete/Expired order
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            routeId = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seqNo) + "\tRouteId: " + str(routeId))
            r = self.getBySequenceNoAndId(seqNo, routeId)

            if r is None:
                r = self.createRoute(seqNo, routeId)
                r.fields.populateFields(msg, False)

            r.fields.field("EMSX_STATUS").setValue("DELETED")
 
            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.DELETE, r, r.fields.getFieldChanges()))                     
            
        elif eventStatus==11:    # End of init paint
#            print("End of ROUTE INIT_PAINT")
            self.initialized=True
            

    def addNotificationHandler(self,handler):
        self.notificationHandlers.append(handler)
        
    def notify(self, notification):
        for h in self.notificationHandlers:
            if not notification.consumed: 
                h(notification)

        if not notification.consumed: self.easyMSX.notify(notification)

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
