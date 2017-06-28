using System.Collections.Generic;
using Name = Bloomberglp.Blpapi.Name;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using Request = Bloomberglp.Blpapi.Request;
using Message = Bloomberglp.Blpapi.Message;
using Element = Bloomberglp.Blpapi.Element;
using System.Collections;

namespace com.bloomberg.emsx.samples 
{
    public class BrokerStrategyParameters : IEnumerable<BrokerStrategyParameter>{

	    private static readonly Name 	GET_BROKER_STRATEGY_INFO = new Name("GetBrokerStrategyInfoWithAssetClass");
	    private static readonly Name 	ERROR_INFO = new Name("ErrorInfo");

	    private List<BrokerStrategyParameter> parameters = new List<BrokerStrategyParameter>();
	    internal BrokerStrategy brokerStrategy;
	
	    internal BrokerStrategyParameters(BrokerStrategy brokerStrategy) {
		    this.brokerStrategy = brokerStrategy;
		    loadStrategyParameters();
	    }
	
	    private void loadStrategyParameters() {
		    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy [" + brokerStrategy.parent.broker.name + "." + brokerStrategy.name + "]: Loading strategy parameters");
		    Request request = brokerStrategy.parent.broker.parent.emsxapi.emsxService.CreateRequest(GET_BROKER_STRATEGY_INFO.ToString());
    	    request.Set("EMSX_BROKER", brokerStrategy.parent.broker.name);
    	    request.Set("EMSX_STRATEGY", brokerStrategy.name);
    	    request.Set("EMSX_ASSET_CLASS", brokerStrategy.parent.broker.assetClass.ToString());
    	    brokerStrategy.parent.broker.parent.emsxapi.submitRequest(request,new BrokerStrategyParametersHandler(this));		
	    }
	
	    class BrokerStrategyParametersHandler : MessageHandler {
		
		    BrokerStrategyParameters brokerStrategyParameters;
		
		    internal BrokerStrategyParametersHandler(BrokerStrategyParameters brokerStrategyParameters) {
			    this.brokerStrategyParameters = brokerStrategyParameters;
		    }

		    public void processMessage(Message message) {
			
			    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy Parameters ["+ brokerStrategyParameters.brokerStrategy.parent.broker.name + "." + brokerStrategyParameters.brokerStrategy.name + "]: processing message");
			
	    	    if(message.MessageType.Equals(ERROR_INFO)) {
	        	    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy Parameters ["+ brokerStrategyParameters.brokerStrategy.parent.broker.name + "." + brokerStrategyParameters.brokerStrategy.name + "]: processing RESPONSE error");
	    		    int errorCode = message.GetElementAsInt32("ERROR_CODE");
	    		    string errorMessage = message.GetElementAsString("ERROR_MESSAGE");
	    		    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy Parameters ["+ brokerStrategyParameters.brokerStrategy.parent.broker.name + "." + brokerStrategyParameters.brokerStrategy.name + "]: [" + errorCode + "] " + errorMessage);
	    	    } else if(message.MessageType.Equals(GET_BROKER_STRATEGY_INFO)) {
	        	    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy Parameters ["+ brokerStrategyParameters.brokerStrategy.parent.broker.name + "." + brokerStrategyParameters.brokerStrategy.name + "]: processing succesful RESPONSE");
	    		
	    		    Element parameters = message.GetElement("EMSX_STRATEGY_INFO");
	    		
				    int numValues = parameters.NumValues;
	    		
	    		    for(int i = 0; i < numValues; i++) {
	    			
	    			    Element parameter = parameters.GetValueAsElement(i);

	    			    string parameterName = parameter.GetElementAsString("FieldName");
	    			    int disable = parameter.GetElementAsInt32("Disable");
	    			    string stringValue = parameter.GetElementAsString("StringValue");

	    			    BrokerStrategyParameter newParameter = new BrokerStrategyParameter(brokerStrategyParameters, parameterName,stringValue,disable);
	    			    brokerStrategyParameters.add(newParameter);
	            	    Log.LogMessage(LogLevels.DETAILED, "Broker Strategy Parameters ["+ brokerStrategyParameters.brokerStrategy.parent.broker.name + "." + brokerStrategyParameters.brokerStrategy.name + "] Added new parameter " + parameterName);
	    		    }
	    	    }		
	        }
	    }

        public IEnumerator<BrokerStrategyParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

	    public BrokerStrategyParameter get(int index) {
		    return parameters[index];
	    }
	
	    public void add(BrokerStrategyParameter newBrokerStrategyParameter) {
		    parameters.Add(newBrokerStrategyParameter);
	    }
    }
}