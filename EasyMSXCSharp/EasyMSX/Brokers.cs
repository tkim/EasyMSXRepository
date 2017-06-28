using System.Collections.Generic;

using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using Element = Bloomberglp.Blpapi.Element;
using Message =  Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;
using System.Collections;

namespace com.bloomberg.emsx.samples {

    public class Brokers : IEnumerable<Broker>  {
	
	    private static readonly Name 	GET_BROKERS = new Name("GetBrokersWithAssetClass");
	    private static readonly Name 	ERROR_INFO = new Name("ErrorInfo");

	    private List<Broker> brokers = new List<Broker>();
        internal EasyMSX emsxapi;
    
	    internal Brokers(EasyMSX emsxapi) {
		    this.emsxapi = emsxapi;
		    loadBrokers();
	    }
	
	    private void loadBrokers() {

		    Log.LogMessage(LogLevels.BASIC,"Brokers: Loading");
	
		    Request reqEQTY = emsxapi.emsxService.CreateRequest(GET_BROKERS.ToString());
		    reqEQTY.Set("EMSX_ASSET_CLASS", Broker.AssetClass.EQTY.ToString());
		    emsxapi.submitRequest(reqEQTY,new BrokersHandler(this, Broker.AssetClass.EQTY));

		    Request reqOPT = emsxapi.emsxService.CreateRequest(GET_BROKERS.ToString());
		    reqOPT.Set("EMSX_ASSET_CLASS", Broker.AssetClass.OPT.ToString());
		    emsxapi.submitRequest(reqOPT,new BrokersHandler(this, Broker.AssetClass.OPT));

		    Request reqFUT = emsxapi.emsxService.CreateRequest(GET_BROKERS.ToString());
		    reqFUT.Set("EMSX_ASSET_CLASS", Broker.AssetClass.FUT.ToString());
		    emsxapi.submitRequest(reqFUT,new BrokersHandler(this, Broker.AssetClass.FUT));

		    Request reqMULTILEFOPT = emsxapi.emsxService.CreateRequest(GET_BROKERS.ToString());
		    reqMULTILEFOPT.Set("EMSX_ASSET_CLASS", Broker.AssetClass.MULTILEG_OPT.ToString());
		    emsxapi.submitRequest(reqMULTILEFOPT,new BrokersHandler(this, Broker.AssetClass.MULTILEG_OPT));
	
	    }

	    class BrokersHandler : MessageHandler {
		
		    Brokers brokers;
		    Broker.AssetClass assetClass;
		
		    internal BrokersHandler(Brokers brokers, Broker.AssetClass assetClass) {
			    this.brokers = brokers;
			    this.assetClass = assetClass;
		    }

		    public void processMessage(Message message) {
			
			    Log.LogMessage(LogLevels.BASIC,"Brokers: processing message");
			
	    	    if(message.MessageType.Equals(ERROR_INFO)) {
	        	    Log.LogMessage(LogLevels.BASIC,"Brokers: processing RESPONSE error");
	    		    int errorCode = message.GetElementAsInt32("ERROR_CODE");
	    		    string errorMessage = message.GetElementAsString("ERROR_MESSAGE");
	    		    Log.LogMessage(LogLevels.BASIC,"Error getting brokers: [" + errorCode + "] " + errorMessage);
	    	    } else if(message.MessageType.Equals(GET_BROKERS)) {
	        	    Log.LogMessage(LogLevels.BASIC,"Brokers: processing successful RESPONSE");
	    		
	        	    Element brokerList = message.GetElement("EMSX_BROKERS");
	    		
				    int numValues = brokerList.NumValues;

				    for(int i = 0; i < numValues; i++) {
	    			
	    			    string brokerName = brokerList.GetValueAsString(i);
	    			    Broker newBroker = new Broker(brokers,brokerName,assetClass);
	    			    brokers.add(newBroker);
	            	    Log.LogMessage(LogLevels.DETAILED,"Brokers: added new broker " + newBroker.name);
	    		    }
	    	    }
		    }
	    }

        public IEnumerator<Broker> GetEnumerator()
        {
            return brokers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

	    public Broker get(int index) {
		    return brokers[index];
	    }
	
	    public Broker get(string name, Broker.AssetClass assetClass) {
		    foreach(Broker b in brokers){
			    if(b.name.Equals(name) && b.assetClass==assetClass)
			     return b;
		    }
		    return null;
	    }
	
	    public void add(Broker newBroker) {
		    brokers.Add(newBroker);
	    }
    }
}
