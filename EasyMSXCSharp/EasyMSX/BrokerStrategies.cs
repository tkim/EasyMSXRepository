using System.Collections.Generic;
using Name = Bloomberglp.Blpapi.Name;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using Request = Bloomberglp.Blpapi.Request;
using Message = Bloomberglp.Blpapi.Message;
using Element = Bloomberglp.Blpapi.Element;
using System.Collections;

namespace com.bloomberg.emsx.samples {

    public class BrokerStrategies: IEnumerable<BrokerStrategy> {
	
        private static readonly Name 	GET_BROKER_STRATEGIES = new Name("GetBrokerStrategiesWithAssetClass");
	    private static readonly Name 	ERROR_INFO = new Name("ErrorInfo");
	
	    private List<BrokerStrategy> strategies = new List<BrokerStrategy>();
	    internal Broker broker;
	
	    internal BrokerStrategies(Broker broker) {
		    this.broker = broker;
		    loadStrategies();
	    }

	    private void loadStrategies() {
		    Log.LogMessage(LogLevels.DETAILED, "Broker [" + broker.name + "]: Loading Strategies");
		    Request request = broker.parent.emsxapi.emsxService.CreateRequest(GET_BROKER_STRATEGIES.ToString());
    	    request.Set("EMSX_BROKER", broker.name);
    	    request.Set("EMSX_ASSET_CLASS", broker.assetClass.ToString());
		    broker.parent.emsxapi.submitRequest(request,new BrokerStrategiesHandler(this));		
	    }
	
	    class BrokerStrategiesHandler : MessageHandler {
		
		    BrokerStrategies brokerStrategies;
		
		    internal BrokerStrategiesHandler(BrokerStrategies brokerStrategies) {
			    this.brokerStrategies = brokerStrategies;
		    }

		    public void processMessage(Message message) {
			
			    Log.LogMessage(LogLevels.DETAILED, "Broker Strategies ["+ brokerStrategies.broker.name + "]: processing message");
			
	    	    if(message.MessageType.Equals(ERROR_INFO)) {
	        	    Log.LogMessage(LogLevels.BASIC, "Broker Strategies ["+ brokerStrategies.broker.name + "]: processing RESPONSE error");
	    		    int errorCode = message.GetElementAsInt32("ERROR_CODE");
	    		    string errorMessage = message.GetElementAsString("ERROR_MESSAGE");
	    		    Log.LogMessage(LogLevels.BASIC, "Broker Strategies ["+ brokerStrategies.broker.name + "]: [" + errorCode + "] " + errorMessage);
	    	    } else if(message.MessageType.Equals(GET_BROKER_STRATEGIES)) {
	        	    Log.LogMessage(LogLevels.DETAILED, "Broker Strategies ["+ brokerStrategies.broker.name + "]: processing succesful RESPONSE");
	    		
	    		    Element strategies = message.GetElement("EMSX_STRATEGIES");
	    		
				    int numValues = strategies.NumValues;
	    		
	    		    for(int i = 0; i < numValues; i++) {
	    			
	    			    string strategy = strategies.GetValueAsString(i);
	    			    if(strategy.Length>0) {
	    				    BrokerStrategy newBrokerStrategy = new BrokerStrategy(brokerStrategies, strategy);
	    				    brokerStrategies.add(newBrokerStrategy);
	    	        	    Log.LogMessage(LogLevels.DETAILED, "Broker Strategies ["+ brokerStrategies.broker.name + "]: added new strategy " + newBrokerStrategy.name);
	    			    }
	    		    }
	    	    }		
	        }
	    }

        public IEnumerator<BrokerStrategy> GetEnumerator()
        {
            return strategies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

	    public BrokerStrategy get(int index) {
		    return strategies[index];
	    }
	
	    public BrokerStrategy get(string name) {
		    foreach(BrokerStrategy s in strategies){
			    if(s.name.Equals(name))
			     return s;
		    }
		    return null;
	    }
	
	    public void add(BrokerStrategy newBrokerStrategy) {
		    strategies.Add(newBrokerStrategy);
	    }
    }
}