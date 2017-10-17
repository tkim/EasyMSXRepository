using EasyMSX = com.bloomberg.emsx.samples.EasyMSX;
using NotificationHandler = com.bloomberg.emsx.samples.NotificationHandler;
using Log = com.bloomberg.emsx.samples.Log;
using Environment = com.bloomberg.emsx.samples.EasyMSX.Environment;
using Team = com.bloomberg.emsx.samples.Team;
using Broker = com.bloomberg.emsx.samples.Broker;
using BrokerStrategy = com.bloomberg.emsx.samples.BrokerStrategy;
using BrokerStrategyParameter = com.bloomberg.emsx.samples.BrokerStrategyParameter;
using Order = com.bloomberg.emsx.samples.Order;
using Route = com.bloomberg.emsx.samples.Route;
using Notification = com.bloomberg.emsx.samples.Notification;
using NotificationCategory = com.bloomberg.emsx.samples.Notification.NotificationCategory;
using NotificationType = com.bloomberg.emsx.samples.Notification.NotificationType;
using FieldChange = com.bloomberg.emsx.samples.FieldChange;

namespace com.bloomberg.test {

    public class EasyMSXAPISample : NotificationHandler {

	    EasyMSX emsx;
	
        static void Main(string[] args)
        {
            System.Console.WriteLine("Bloomberg - EasyMSX Example - EasyMSXAPISample");

            EasyMSXAPISample example = new EasyMSXAPISample();
            
            example.run(args);
        
            System.Console.ReadKey();    

        }

	    private void run(string[] args) {
	
		    Log.logLevel = Log.LogLevels.DETAILED;

		    System.Console.WriteLine("Initializing EMSXAPI ");

		    //emsx = new EasyMSX(Environment.BETA,"RCAM_API");
		    emsx = new EasyMSX(Environment.BETA);
		    //emsx = new EasyMSX(Environment.BETA, "bpipe-ny-beta.bdns.bloomberg.com", 8194,"CORP\\rclegg2","172.16.21.92");

		
		    System.Console.WriteLine("EMSXAPI initialized OK");
		
		    emsx.orders.addNotificationHandler(this);
		    emsx.routes.addNotificationHandler(this);
		
		    foreach(Team t in emsx.teams) {
			    System.Console.WriteLine("Team: " + t.name);
		    }
	
		    foreach(Broker b in emsx.brokers) {
			    System.Console.WriteLine("Broker: " + b.name);
			
			    System.Console.WriteLine("\tAsset Class: " + b.assetClass.ToString());
			
			    foreach(BrokerStrategy s in b.strategies) {
				    System.Console.WriteLine("\tStrategy: " + s.name);
				
				    foreach(BrokerStrategyParameter p in s.parameters) {
					    System.Console.WriteLine("\t\tParameter: " + p.name);
				    }
			    }
		    }
		
		    System.Console.WriteLine("Existing Orders:");
		
		    foreach(Order o in emsx.orders) {
			    System.Console.WriteLine("\tSequence: " + o.field("EMSX_SEQUENCE").value() + "\tStatus: " + o.field("EMSX_STATUS").value() + "\t Ticker: " + o.field("EMSX_TICKER").value() + "\t Amount: " + o.field("EMSX_AMOUNT").value());
		    }

		    System.Console.WriteLine("Existing Routes:");
		
		    foreach(Route r in emsx.routes) {
			    System.Console.WriteLine("\tSequence: " + r.field("EMSX_SEQUENCE").value() + "\tID: " + r.field("EMSX_ROUTE_ID").value() + "\tStatus: " + r.field("EMSX_STATUS").value()  + "\t Amount: " + r.field("EMSX_AMOUNT").value());
		    }
	    }

	    public void processNotification(Notification notification) {

		    if(notification.category==NotificationCategory.ORDER) {
			    if(notification.type.Equals(NotificationType.ERROR)) {
				    System.Console.WriteLine("Order Notification [" + notification.category.ToString() + "|" + notification.type.ToString() + "] "  + " Error Code:" + notification.errorCode() + "\t" + notification.errorMessage());
			    } else {
				    System.Console.WriteLine("Order Notification [" + notification.category.ToString() + "|" + notification.type.ToString() + "] "  + " Order: " + notification.getOrder().field("EMSX_SEQUENCE").value() + " : No. of affected fields: " + notification.getFieldChanges().Count);
				    notification.consume = true;
				    foreach(FieldChange fc in notification.getFieldChanges()) {
					    //System.Console.WriteLine("\t\tChange: " + fc.field.name() + "\tOld Value: " + fc.oldValue + "\tNew Value: " + fc.newValue);
				    }
			    }
		    } 
		    else if(notification.category==NotificationCategory.ROUTE) {
			    if(notification.type.Equals(NotificationType.ERROR)) {
				    System.Console.WriteLine("Route Notification [" + notification.category.ToString() + "|" + notification.type.ToString() + "] "  + " Error Code:" + notification.errorCode() + "\t" + notification.errorMessage());
			    } else {
				    System.Console.WriteLine("Route Notification [" + notification.category.ToString() + "|" + notification.type.ToString() + "] "  + " Route: " + notification.getRoute().field("EMSX_SEQUENCE").value() + "." + notification.getRoute().field("EMSX_ROUTE_ID").value() + " : No. of affected fields: " + notification.getFieldChanges().Count);
				    notification.consume = true;
				    foreach(FieldChange fc in notification.getFieldChanges()) {
					    System.Console.WriteLine("\t\tChange: " + fc.field.name() + "\tOld Value: " + fc.oldValue + "\tNew Value: " + fc.newValue);
				    }
			    }
		    }
	    }
    }
}