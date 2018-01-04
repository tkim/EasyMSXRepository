# orders.py

import blpapi
from .order import Order
from .notification import Notification

SUBSCRIPTION_STARTED            = blpapi.Name("SubscriptionStarted")

class Orders:

    def __init__(self,easymsx):
        self.easymsx = easymsx
        self.orders=[]
        self.field_source = self.easymsx.order_fields
        self.initialized=False
        self.notification_handlers = []
        
    def __iter__(self):
        return self.orders.__iter__()

    def subscribe(self):
        
        order_topic = self.easymsx.emsx_service_name + "/order"
        if self.easymsx.team != None: 
            order_topic += ";team=" + self.easymsx.team.name
        order_topic += "?fields="   
        for f in self.field_source:
            if f.name=="EMSX_ORDER_REF_ID":
                order_topic += "EMSX_ORD_REF_ID,"
            else:
                order_topic += f.name + ","
            
        order_topic = order_topic[:-1] # truncate the trailing comma character
        
        self.easymsx.subscribe(order_topic, self.process_message)
        
        while not self.initialized:
            pass
        
    def create_order(self, seq_no):
        o = Order(self)
        o.sequence = seq_no
        self.orders.append(o)
        return o
    
    def get_by_sequence_no(self, seq_no):
        for o in self.orders:
            if o.sequence == seq_no:
                return o
        return None
    
    def process_message(self, msg):

        if msg.messageType() == SUBSCRIPTION_STARTED:
#            print("Order Subscription Started...")
            return

        event_status = msg.getElementAsInteger("EVENT_STATUS")
        
        if event_status==1:      # Heartbeat
#            print("Order >> Heartbeat")
            pass
        
        elif event_status==4:    # Initial paint
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
#            print("Order >> Event(4) >> SeqNo: " + str(seq_no))
            o = self.get_by_sequence_no(seq_no)
        
            if o is None:
                o = self.create_order(seq_no)
        
            o.fields.populate_fields(msg, False)
        
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.INITIALPAINT, o, o.fields.get_field_changes()))                     
        
        elif event_status==6:    # New order
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
#            print("Order >> Event(6) >> SeqNo: " + str(seq_no))
            o = self.get_by_sequence_no(seq_no)
        
            if o is None:
                o = self.create_order(seq_no)
        
            o.fields.populate_fields(msg, False)

            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.NEW, o, o.fields.get_field_changes()))                     
        
        elif event_status==7:    # Update order
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
#            print("Order >> Event(7) >> SeqNo: " + str(seq_no))
            o = self.get_by_sequence_no(seq_no)
        
            if o is None:
#                print("WARNING >> update received for unknown order")
                o = self.create_order(seq_no)
        
            o.fields.populate_fields(msg, True)
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.UPDATE, o, o.fields.get_field_changes()))                     

        elif event_status==8:    # Delete/Expired order
            
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
#            print("Order >> Event(8) >> SeqNo: " + str(seq_no))
            o = self.get_by_sequence_no(seq_no)
            if o is None:
                o = self.create_order(seq_no)
                o.fields.populate_fields(msg, False)

            o.fields.field("EMSX_STATUS").set_value("DELETED")
 
            o.notify(Notification(Notification.NotificationCategory.ORDER, Notification.NotificationType.DELETE, o, o.fields.get_field_changes()))                     
            
        elif event_status==11:    # End of init paint
#            print("End of ORDER INIT_PAINT")
            self.initialized=True
            
    def add_notification_handler(self,handler):
        self.notification_handlers.append(handler)
        
    def notify(self, notification):
        for h in self.notification_handlers:
            if not notification.consumed: 
                h(notification)

        if not notification.consumed: self.easymsx.notify(notification)

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
