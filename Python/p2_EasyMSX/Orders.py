# Orders.py

import blpapi
from Order import Order
from Notification import *

SUBSCRIPTION_STARTED            = blpapi.Name("SubscriptionStarted")

class Orders:

    def __init__(self,easyMSX):
        self.easyMSX = easyMSX
        self.orders=[]
        self.fieldSource = self.easyMSX.orderFields
        self.initialized=False
        self.notificationHandlers = []
        
    def __iter__(self):
        return self.orders.__iter__()

    def subscribe(self):
        
        orderTopic = self.easyMSX.emsxServiceName + "/order"
        if self.easyMSX.team != None: 
            orderTopic += ";team=" + self.easyMSX.team.name
        orderTopic += "?fields="   
        for f in self.fieldSource:
            if f.name=="EMSX_ORDER_REF_ID":
                orderTopic += "EMSX_ORD_REF_ID,"
            else:
                orderTopic += f.name + ","
            
        orderTopic = orderTopic[:-1] # truncate the trailing comma character
        
        self.easyMSX.subscribe(orderTopic, self.processMessage)
        
        while not self.initialized:
            pass
        
    def createOrder(self, seqNo):
        o = Order(self)
        o.sequence = seqNo
        self.orders.append(o)
        return o
    
    def getBySequenceNo(self, seqNo):
        for o in self.orders:
            if o.sequence == seqNo:
                return o
        return None
    
    def processMessage(self, msg):

        if msg.messageType() == SUBSCRIPTION_STARTED:
            #print("Order Subscription Started...")
            return

        eventStatus = msg.getElementAsInteger("EVENT_STATUS")
        
        if eventStatus==1:      # Heartbeat
            #print("Order >> Heartbeat")
            pass
        
        elif eventStatus==4:    # Initial paint
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            #print("Order >> Event(4) >> SeqNo: " + str(seqNo))
            o = self.getBySequenceNo(seqNo)
        
            if o is None:
                o = self.createOrder(seqNo)
        
            o.fields.populateFields(msg, False)
        
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.INITIALPAINT, o, o.fields.getFieldChanges()))                     
        
        elif eventStatus==6:    # New order
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            #print("Order >> Event(6) >> SeqNo: " + str(seqNo))
            o = self.getBySequenceNo(seqNo)
        
            if o is None:
                o = self.createOrder(seqNo)
        
            o.fields.populateFields(msg, False)

            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.NEW, o, o.fields.getFieldChanges()))                     
        
        elif eventStatus==7:    # Update order
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            #print("Order >> Event(7) >> SeqNo: " + str(seqNo))
            o = self.getBySequenceNo(seqNo)
        
            if o is None:
                #print("WARNING >> update received for unknown order")
                o = self.createOrder(seqNo)
        
            o.fields.populateFields(msg, True)
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.UPDATE, o, o.fields.getFieldChanges()))                     

        elif eventStatus==8:    # Delete/Expired order
            
            seqNo = msg.getElementAsInteger("EMSX_SEQUENCE")
            #print("Order >> Event(8) >> SeqNo: " + str(seqNo))
            o = self.getBySequenceNo(seqNo)
            if o is None:
                o = self.createOrder(seqNo)
                o.fields.populateFields(msg, False)

            o.fields.field("EMSX_STATUS").setValue("DELETED")
 
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.DELETE, o, o.fields.getFieldChanges()))                     
            
        elif eventStatus==11:    # End of init paint
            #print("End of ORDER INIT_PAINT")
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
