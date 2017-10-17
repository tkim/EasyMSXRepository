using System.Collections.Generic;

namespace com.bloomberg.emsx.samples {

    public class Order : FieldsOwner {
	
	    internal Orders parent;
	    internal int sequence;
	    private Broker broker;
	
	    List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

	    internal Order(Orders parent) {
		    this.parent = parent;
		    this.fields = new Fields(this);
		    this.sequence = 0;
	    }
	
	    public Broker getBroker(){
		    return this.broker;
	    }
	
	    public Field field(string fieldname) {
		    return this.fields.field(fieldname);
	    }

	    public void addNotificationHandler(NotificationHandler notificationHandler) {
		    notificationHandlers.Add(notificationHandler);
	    }

	    internal void notify(Notification notification) {
		
		    foreach(NotificationHandler nh in notificationHandlers) {
			    if(!notification.consume) nh.processNotification(notification);
		    }
		    if(!notification.consume) parent.processNotification(notification);

	    }
    }
}