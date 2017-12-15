# Brokers.py

import blpapi
from Broker import Broker

GET_BROKERS = blpapi.Name("GetBrokersWithAssetClass")
ERROR_INFO = blpapi.Name("ErrorInfo")

class Brokers:
    
    def __init__(self,easyMSX):
        self.easyMSX = easyMSX
        self.brokers=[]
        self.loadBrokers()
    
    def __iter__(self):
        return self.brokers.__iter__()
    
    def loadBrokers(self):
        
        reqEQTY = self.easyMSX.emsxService.createRequest(str(GET_BROKERS))
        reqEQTY.set("EMSX_ASSET_CLASS","EQTY")
        self.easyMSX.submitRequest(reqEQTY, BrokerMessageHandler(self,"EQTY").processMessage)

        reqOPT = self.easyMSX.emsxService.createRequest(str(GET_BROKERS))
        reqOPT.set("EMSX_ASSET_CLASS","OPT")
        self.easyMSX.submitRequest(reqOPT, BrokerMessageHandler(self,"OPT").processMessage)

        reqFUT = self.easyMSX.emsxService.createRequest(str(GET_BROKERS))
        reqFUT.set("EMSX_ASSET_CLASS","FUT")
        self.easyMSX.submitRequest(reqFUT, BrokerMessageHandler(self,"FUT").processMessage)
        
        reqMULTILEG = self.easyMSX.emsxService.createRequest(str(GET_BROKERS))
        reqMULTILEG.set("EMSX_ASSET_CLASS","MULTILEG_OPT")
        self.easyMSX.submitRequest(reqMULTILEG, BrokerMessageHandler(self,"MULTILEG_OPT").processMessage)

class BrokerMessageHandler:
    
    def __init__(self, brokers, assetClass):
        self.brokers = brokers
        self.assetClass = assetClass
        
    def processMessage(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            errorCode = msg.getElementAsInteger("ERROR_CODE")
            errorMessage = msg.getElementAsString("ERROR_MESSAGE")
            print("GetBrokers >> ERROR CODE: %d\tERROR MESSAGE: %s" % (errorCode,errorMessage))

        elif msg.messageType() == GET_BROKERS:
            
            bkrs = msg.getElement("EMSX_BROKERS")
            
            for b in bkrs.values():
                self.brokers.brokers.append(Broker(self.brokers,b,self.assetClass))
                
                                  
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
