using System;
using System.Collections;
using System.Collections.Generic;
using Subscription = Bloomberglp.Blpapi.Subscription;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using SchemaFieldDefinition = Bloomberglp.Blpapi.SchemaElementDefinition;
using System.Threading;

namespace com.bloomberg.emsx.samples {

    public class Orders : IEnumerable<Order>, NotificationHandler {
	
	    private List<Order> orders = new List<Order>();
	    List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

	    private Subscription orderSubscription;
	
	    internal EasyMSX emsxapi;
	
	    internal Orders(EasyMSX emsxapi) {
		    this.emsxapi = emsxapi;
		    subscribe();
	    }
	
	    private void subscribe() {
		
		    Log.LogMessage(LogLevels.BASIC, "Orders: Subscribing");
		
            String orderTopic = emsxapi.emsxServiceName + "/order";
        		
            if(emsxapi.team!=null) orderTopic = orderTopic + ";team=" + emsxapi.team.name;
        
            orderTopic = orderTopic + "?fields=";
        
            foreach (SchemaFieldDefinition f in emsxapi.orderFields) {
        	    if(f.name.Equals("EMSX_ORDER_REF_ID")) { // Workaround for schema field naming
            	    orderTopic = orderTopic + "EMSX_ORD_REF_ID" + ",";
        	    } else {
        		    orderTopic = orderTopic + f.name + ","; 
        	    }
        	
            }
        
            orderTopic = orderTopic.Substring(0,orderTopic.Length-1); // remove extra comma character

            emsxapi.subscribe(orderTopic, new OrderSubscriptionHandler(this));
    	    Log.LogMessage(LogLevels.BASIC, "Entering Order subscription lock");
            while(!emsxapi.orderBlotterInitialized){
                Thread.Sleep(1);
            }
    	    Log.LogMessage(LogLevels.BASIC, "Order subscription lock released");
	    }
	
	    internal Order createOrder(int sequence) {
		    Order o = new Order(this);
		    o.sequence = sequence;
		    orders.Add(o);
		    return o;
	    }

        public IEnumerator<Order> GetEnumerator()
        {
            return orders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
	
	    public Order getByRefID(String refID) {
		    foreach(Order o in orders) {
			    if(o.field("EMSX_ORDER_REF_ID").value()==refID) return o;
		    }
		    return null;
	    }
	
	    public Order getBySequenceNo(int sequence) {
		    foreach(Order o in orders) {
			    if(o.sequence == sequence) return o;
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
		    return orders.Count;
	    }
    }
}