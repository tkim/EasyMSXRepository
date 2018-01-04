# brokerstrategyparameters.py

import blpapi
from .brokerstrategyparameter import BrokerStrategyParameter

GET_BROKER_STRATEGY_INFO = blpapi.Name("GetBrokerStrategyInfoWithAssetClass")
ERROR_INFO = blpapi.Name("ErrorInfo")

class BrokerStrategyParameters:
    
    def __init__(self,broker_strategy):
        self.broker_strategy = broker_strategy
        self.parameters=[]
        self.load_strategy_parameters()

    def __iter__(self):
        return self.parameters.__iter__()
    
    def load_strategy_parameters(self):
        
        request = self.broker_strategy.parent.broker.parent.easymsx.emsx_service.createRequest(str(GET_BROKER_STRATEGY_INFO))
        request.set("EMSX_BROKER", self.broker_strategy.parent.broker.name)
        request.set("EMSX_STRATEGY", self.broker_strategy.name)
        request.set("EMSX_ASSET_CLASS",self.broker_strategy.parent.broker.asset_class)
        #print("Sending request: " + str(request))
        
        self.broker_strategy.parent.broker.parent.easymsx.submit_request(request, self.process_message)

    def process_message(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            error_code = msg.getElementAsInteger("ERROR_CODE")
            error_message = msg.getElementAsString("ERROR_MESSAGE")
            if ":2" not in error_message: # fix for issue with Strategies having 0 parameters
                print("GetBrokerStrategyInfoWithAssetClass >> ERROR CODE: %d\tERROR MESSAGE: %s" % (error_code,error_message))

        elif msg.messageType() == GET_BROKER_STRATEGY_INFO:
            
            params = msg.getElement("EMSX_STRATEGY_INFO")
            
            for p in params.values():
                field_name = p.getElementAsString("FieldName")
                disable = p.getElementAsInteger("Disable")
                value = p.getElementAsString("StringValue")
                new_bkr_stp = BrokerStrategyParameter(self, field_name, value, disable)
                self.parameters.append(new_bkr_stp)
                                  
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
