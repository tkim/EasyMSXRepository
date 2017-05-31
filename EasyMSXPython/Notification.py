# Notification.py

class Notification:

    class NotificationCategory:
        ORDER=0
        ROUTE=1
        ADMIN=2
        
    class NotificationType:
        NEW=0
        INITIALPAINT=1
        UPDATE=2
        DELETE=3
        CANCEL=4
        ERROR=5
        
    def __init__(self,notificationCategory,notificationType,notificationSource,fieldChanges=[],errorCode=0,errorMessage=""):
        self.category = notificationCategory
        self.type = notificationType
        self.source = notificationSource
        self.fieldChanges = fieldChanges
        self.errorCode = errorCode
        self.errorMessage = errorMessage
        self.consumed=False

