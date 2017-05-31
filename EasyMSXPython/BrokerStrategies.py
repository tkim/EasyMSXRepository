# BrokerStrategies.py

import blpapi
from BrokerStrategy import BrokerStrategy

GET_BROKER_STRATEGIES = blpapi.Name("GetBrokerStrategiesWithAssetClass")
ERROR_INFO = blpapi.Name("ErrorInfo")

class BrokerStrategies:
    
    def __init__(self,broker):
        self.broker = broker
        self.strategies=[]
        self.loadStrategies()
        
    def __iter__(self):
        return self.strategies.__iter__()

    def loadStrategies(self):
        request = self.broker.parent.easyMSX.emsxService.createRequest(str(GET_BROKER_STRATEGIES))
        request.set("EMSX_BROKER", self.broker.name)
        request.set("EMSX_ASSET_CLASS", self.broker.assetClass)
        self.broker.parent.easyMSX.submitRequest(request, self.processMessage)
        
    def processMessage(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            errorCode = msg.getElementAsInteger("ERROR_CODE")
            errorMessage = msg.getElementAsString("ERROR_MESSAGE")
            print "GetTeams >> ERROR CODE: %d\tERROR MESSAGE: %s" % (errorCode,errorMessage)

        elif msg.messageType() == GET_BROKER_STRATEGIES:
            
            strats = msg.getElement("EMSX_STRATEGIES")
            
            for s in strats.values():
                if len(s) > 0:
                    self.strategies.append(BrokerStrategy(self,s))
                                  
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
