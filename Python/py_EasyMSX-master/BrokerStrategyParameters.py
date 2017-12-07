# BrokerStrategyParameters.py

import blpapi
from BrokerStrategyParameter import BrokerStrategyParameter

GET_BROKER_STRATEGY_INFO = blpapi.Name("GetBrokerStrategyInfoWithAssetClass")
ERROR_INFO = blpapi.Name("ErrorInfo")

class BrokerStrategyParameters:
    
    def __init__(self,brokerStrategy):
        self.brokerStrategy = brokerStrategy
        self.parameters=[]
        self.loadStrategyParameters()

    def __iter__(self):
        return self.parameters.__iter__()
    
    def loadStrategyParameters(self):
        
        request = self.brokerStrategy.parent.broker.parent.easyMSX.emsxService.createRequest(str(GET_BROKER_STRATEGY_INFO))
        request.set("EMSX_BROKER", self.brokerStrategy.parent.broker.name)
        request.set("EMSX_STRATEGY", self.brokerStrategy.name)
        request.set("EMSX_ASSET_CLASS",self.brokerStrategy.parent.broker.assetClass)
        #print("Sending request: " + str(request))
        
        self.brokerStrategy.parent.broker.parent.easyMSX.submitRequest(request, self.processMessage)

    def processMessage(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            errorCode = msg.getElementAsInteger("ERROR_CODE")
            errorMessage = msg.getElementAsString("ERROR_MESSAGE")
            if ":2" not in errorMessage: # fix for issue with Strategies having 0 parameters
                print("GetBrokerStrategyInfoWithAssetClass >> ERROR CODE: %d\tERROR MESSAGE: %s" % (errorCode,errorMessage))

        elif msg.messageType() == GET_BROKER_STRATEGY_INFO:
            
            params = msg.getElement("EMSX_STRATEGY_INFO")
            
            for p in params.values():
                fieldname = p.getElementAsString("FieldName")
                disable = p.getElementAsInteger("Disable")
                value = p.getElementAsString("StringValue")
                newBkrStP = BrokerStrategyParameter(self, fieldname, value, disable)
                self.parameters.append(newBkrStP)
                                  
__copyright__ = """
Copyright 2017. Bloomberg Finance L.P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  The above
copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
"""
