using Name = Bloomberglp.Blpapi.Name;
using Message = Bloomberglp.Blpapi.Message;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using NotificationCategory = com.bloomberg.emsx.samples.Notification.NotificationCategory;
using NotificationType = com.bloomberg.emsx.samples.Notification.NotificationType;


namespace com.bloomberg.emsx.samples {

    class RouteSubscriptionHandler : MessageHandler {
	
	    private static readonly Name	SUBSCRIPTION_FAILURE 	= new Name("SubscriptionFailure");
	    private static readonly Name	SUBSCRIPTION_STARTED	= new Name("SubscriptionStarted");
	    private static readonly Name	SUBSCRIPTION_TERMINATED	= new Name("SubscriptionTerminated");

	    Routes routes;
	
	    internal RouteSubscriptionHandler(Routes routes) {
		    this.routes = routes;
	    }

	    public void processMessage(Message message) {

		    Log.LogMessage(LogLevels.DETAILED, "RouteSubscriptionHandler: Processing message");
		
		    Log.LogMessage(LogLevels.DETAILED, "Message: " + message.ToString());

		    if(message.MessageType.Equals(SUBSCRIPTION_STARTED)) {
			    Log.LogMessage(LogLevels.BASIC, "Route subscription started");
			    return;
		    }
			
		    int eventStatus = message.GetElementAsInt32("EVENT_STATUS");

		    if(eventStatus==1) {
			    Log.LogMessage(LogLevels.DETAILED, "RouteSubscriptionHandler: HEARTBEAT received");
		    } else if(eventStatus==4) { //init_paint
			    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: INIT_PAINT message received");
			    Log.LogMessage(LogLevels.DETAILED, "Message: " + message.ToString());
		
			    int sequence = message.GetElementAsInt32("EMSX_SEQUENCE");
			    int routeID = message.GetElementAsInt32("EMSX_ROUTE_ID");
			
			    Route r = routes.getBySequenceNoAndID(sequence, routeID);
			
			    if(r==null) r = routes.createRoute(sequence, routeID);

			    r.fields.populateFields(message,false);
			    r.notify(new Notification(NotificationCategory.ROUTE, NotificationType.INITIALPAINT, r, r.fields.getFieldChanges()));
			
		    } else if(eventStatus==6) { //new
			    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: NEW_ORDER_ROUTE message received");
			    Log.LogMessage(LogLevels.DETAILED, "Message: " + message.ToString());

			    int sequence = message.GetElementAsInt32("EMSX_SEQUENCE");
			    int routeID = message.GetElementAsInt32("EMSX_ROUTE_ID");
			
			    Route r = routes.getBySequenceNoAndID(sequence, routeID);
			
			    if(r==null) r = routes.createRoute(sequence,routeID);

			    r.fields.populateFields(message,false);
			    r.notify(new Notification(NotificationCategory.ROUTE, NotificationType.NEW, r, r.fields.getFieldChanges()));
			
		    } else if(eventStatus==7) { // update
			    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: UPD_ORDER_ROUTE message received");
			    Log.LogMessage(LogLevels.DETAILED, "Message: " + message.ToString());
			
			    int sequence = message.GetElementAsInt32("EMSX_SEQUENCE");
			    int routeID = message.GetElementAsInt32("EMSX_ROUTE_ID");

			    Route r = routes.getBySequenceNoAndID(sequence, routeID);
			
			    if(r==null) { 
				    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: WARNING > Update received for unkown route");
				    r = routes.createRoute(sequence, routeID);
			    }
			    r.fields.populateFields(message,true);
			    r.notify(new Notification(NotificationCategory.ROUTE, NotificationType.UPDATE, r, r.fields.getFieldChanges()));
		
		    } else if(eventStatus==8) { // deleted/expired
			    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: DELETE message received");
			    Log.LogMessage(LogLevels.DETAILED, "Message: " + message.ToString());
			
			    int sequence = message.GetElementAsInt32("EMSX_SEQUENCE");
			    int routeID = message.GetElementAsInt32("EMSX_ROUTE_ID");

			    Route r = routes.getBySequenceNoAndID(sequence, routeID);

			    if(r==null) { // Order not found
				    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: WARNING > Delete received for unkown route");
				    r = routes.createRoute(sequence, routeID);
			    }
			    r.fields.populateFields(message,false);
			    r.fields.field("EMSX_STATUS").setCurrentValue("EXPIRED");
			    r.notify(new Notification(NotificationCategory.ROUTE, NotificationType.DELETE, r, r.fields.getFieldChanges()));
		    } else if(eventStatus==11) { // INIT_PAINT_END
			    // End of inital paint messages
			    Log.LogMessage(LogLevels.BASIC, "RouteSubscriptionHandler: End of Initial Paint");
			    routes.emsxapi.routeBlotterInitialized = true;
		    }
	    }
    }
}