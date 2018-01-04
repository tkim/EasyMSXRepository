# routes.py

import blpapi
from .route import Route
from .notification import Notification

SUBSCRIPTION_STARTED            = blpapi.Name("SubscriptionStarted")

class Routes:

    def __init__(self,easymsx):
        self.easymsx = easymsx
        self.routes=[]
        self.field_source = self.easymsx.route_fields
        self.notification_handlers=[]
        self.initialized=False
        
    def __iter__(self):
        return self.routes.__iter__()

    def subscribe(self):
        
        route_topic = self.easymsx.emsx_service_name + "/route"
        if self.easymsx.team != None: 
            route_topic += ";team=" + self.easymsx.team.name
        route_topic += "?fields="   
        for f in self.field_source:
            route_topic += f.name + ","
            
        route_topic = route_topic[:-1] # truncate the trailing comma character
        
        self.easymsx.subscribe(route_topic, self.process_message)
        
        while not self.initialized:
            pass
        
    def create_route(self, seq_no, route_id):
        r = Route(self)
        r.sequence = seq_no
        r.route_id = route_id
        self.routes.append(r)
        return r
    
    def get_by_sequence_no_and_id(self, seq_no, route_id):
        for r in self.routes:
            if r.sequence == seq_no and r.route_id == route_id:
                return r
        return None
    
    def process_message(self, msg):

        if msg.messageType() == SUBSCRIPTION_STARTED:
#            print("Route Subscription Started...")
            return

        event_status = msg.getElementAsInteger("EVENT_STATUS")
        
        if event_status==1:      # Heartbeat
#            print("Route >> Heartbeat")
            pass
        
        elif event_status==4:    # Initial paint
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
            route_id = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seq_no) + "\tRouteId: " + str(route_id))
            r = self.get_by_sequence_no_and_id(seq_no, route_id)

            if r is None:
                r = self.create_route(seq_no, route_id)
        
            r.fields.populate_fields(msg, False)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.INITIALPAINT, r, r.fields.get_field_changes()))                     
        
        elif event_status==6:    # New order
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
            route_id = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seq_no) + "\tRouteId: " + str(route_iId))
            r = self.get_by_sequence_no_and_id(seq_no, route_id)
        
            if r is None:
                r = self.create_route(seq_no, route_id)
        
            r.fields.populate_fields(msg, False)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.NEW, r, r.fields.get_field_changes()))                     
        
        elif event_status==7:    # Update order
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
            route_id = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seq_no) + "\tRouteId: " + str(route_id))
            r = self.get_by_sequence_no_and_id(seq_no, route_id)
        
            if r is None:
#                print("WARNING >> update received for unknown order")
                r = self.create_route(seq_no, route_id)
        
            r.fields.populate_fields(msg, True)

            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.UPDATE, r, r.fields.get_field_changes()))                     

        elif event_status==8:    # Delete/Expired order
            seq_no = msg.getElementAsInteger("EMSX_SEQUENCE")
            route_id = msg.getElementAsInteger("EMSX_ROUTE_ID")
#            print("Route >> Event(4) >> SeqNo: " + str(seq_no) + "\tRouteId: " + str(route_id))
            r = self.get_by_sequence_no_and_id(seq_no, route_id)

            if r is None:
                r = self.create_route(seq_no, route_id)
                r.fields.populate_fields(msg, False)

            r.fields.field("EMSX_STATUS").set_value("DELETED")
 
            r.notify(Notification(Notification.NotificationCategory.ROUTE, Notification.NotificationType.DELETE, r, r.fields.get_field_changes()))                     
            
        elif event_status==11:    # End of init paint
#            print("End of ROUTE INIT_PAINT")
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
