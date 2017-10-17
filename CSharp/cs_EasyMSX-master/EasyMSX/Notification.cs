using System.Collections.Generic;
using ArrayList = System.Collections.ArrayList;

namespace com.bloomberg.emsx.samples {

    public class Notification {
	
	    public enum NotificationCategory {
		    ORDER,
		    ROUTE,
		    ADMIN
	    }
	
	    public enum NotificationType { 
		    NEW,
		    INITIALPAINT,
		    UPDATE,
		    DELETE,
		    CANCEL,
		    ERROR,
            FIELD
        }

	    public NotificationCategory category;
	    public NotificationType type;
	    public bool consume = false; 
	
	    private Order order;
	    private Route route;
	
	    private List<FieldChange> fieldChanges;
	    private int _errorCode;
	    private string _errorMessage;
	
	    internal Notification (NotificationCategory category, NotificationType type, object source, List<FieldChange> fieldChanges) {
		    this.category = category;
		    this.type = type;
		    if(category==NotificationCategory.ORDER) {
			    order = (Order)source;
		    } else if(category==NotificationCategory.ROUTE) {
			    route = (Route)source;
		    }
		    this.fieldChanges = fieldChanges;
	    }
	
	    Notification (NotificationCategory category, NotificationType type, object source, int errorCode, string errorMessage) {
		    this.category = category;
		    this.type = type;
		    if(category==NotificationCategory.ORDER) {
			    order = (Order)source;
		    } else if(category==NotificationCategory.ROUTE) {
			    route = (Route)source;
		    }
		    this._errorCode = errorCode;
		    this._errorMessage = errorMessage;
	    }

	    public Order getOrder() {
		    return this.order;
	    }
	
	    public Route getRoute() {
		    return this.route;
	    }
	
	    public List<FieldChange> getFieldChanges() {
		    return this.fieldChanges;
	    }
	
	    public int errorCode() {
		    return this._errorCode;
	    }
	
	    public string errorMessage() {
		    return this._errorMessage;
	    }
    }
}