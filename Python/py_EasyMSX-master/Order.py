# Order.py

from Fields import Fields

class Order:
    
    def __init__(self, parent):
        self.parent = parent
        self.sequence = 0
        self.notificationHandlers = []
        self.fields=Fields(self)
        
    def field(self,fieldname):
        return self.fields.field(fieldname)

    def addNotificationHandler(self,handler):
        self.notificationHandlers.append(handler)

    def notify(self, notification):
        for h in self.notificationHandlers:
            if not notification.consumed:
                h(notification)
        if not notification.consumed: 
            self.parent.notify(notification)
            