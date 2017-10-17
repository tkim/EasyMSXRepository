using System;
using System.Collections;
using System.Collections.Generic;
using Subscription = Bloomberglp.Blpapi.Subscription;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using System.Threading;


namespace com.bloomberg.emsx.samples {

    public class Routes : IEnumerable<Route>, NotificationHandler {
	
	    private List<Route> routes = new List<Route>();
	    List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

	    private Subscription routeSubscription;
	
	    internal EasyMSX emsxapi;
	
	    internal Routes(EasyMSX emsxapi) {
		    this.emsxapi = emsxapi;
	    }
	
	    internal void subscribe() {
		
		    Log.LogMessage(LogLevels.BASIC, "Routes: Subscribing");
		
            String routeTopic = emsxapi.emsxServiceName + "/route";
        		
            if(emsxapi.team!=null) routeTopic = routeTopic + ";team=" + emsxapi.team.name;
        
            routeTopic = routeTopic + "?fields=";
        
            foreach (SchemaFieldDefinition f in emsxapi.routeFields) {
    		    routeTopic = routeTopic + f.name + ","; 
            }
        
            routeTopic = routeTopic.Substring(0,routeTopic.Length-1); // remove extra comma character

            Log.LogMessage(LogLevels.DETAILED, "Route Topic: " + routeTopic);

            emsxapi.subscribe(routeTopic, new RouteSubscriptionHandler(this));
    	    Log.LogMessage(LogLevels.BASIC, "Entering Route subscription lock");
            while(!emsxapi.routeBlotterInitialized){
                Thread.Sleep(1);
            }
    	    Log.LogMessage(LogLevels.BASIC, "Route subscription lock released");
	    }
	
	    internal Route createRoute(int sequence, int routeID) {
		    Route r = new Route(this);
		    r.sequence = sequence;
		    r.routeID = routeID;
		    routes.Add(r);
		    return r;
	    }

        public IEnumerator<Route> GetEnumerator()
        {
            return routes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
	
	    public Route getByRefID(String refID) {
		    foreach(Route r in routes) {
			    if(r.field("EMSX_ROUTE_REF_ID").value()==refID) return r;
		    }
		    return null;
	    }
	
	    public Route getBySequenceNoAndID(int sequence, int routeID) {
		    foreach(Route r in routes) {
			    if((r.sequence == sequence) && (r.routeID == routeID)) return r;
		    }
		    return null;
	    }

	    public void addNotificationHandler(NotificationHandler notificationHandler) {
		    notificationHandlers.Add(notificationHandler);
	    }
	
	    public void processNotification(Notification notification) {
		    foreach(NotificationHandler nh in notificationHandlers) {
			    if(!notification.consume) nh.processNotification(notification);
		    }
		    if(!notification.consume) emsxapi.processNotification(notification);
	    }

	    public int count() {
		    return routes.Count;
	    }
    }
}