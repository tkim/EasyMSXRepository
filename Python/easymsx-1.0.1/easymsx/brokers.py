# brokers.py

import blpapi
from .broker import Broker

GET_BROKERS = blpapi.Name("GetBrokersWithAssetClass")
ERROR_INFO = blpapi.Name("ErrorInfo")

class Brokers:
    
    def __init__(self,easymsx):
        self.easymsx = easymsx
        self.brokers=[]
        self.load_brokers()
    
    def __iter__(self):
        return self.brokers.__iter__()
    
    def load_brokers(self):
        
        req_eqty = self.easymsx.emsx_service.createRequest(str(GET_BROKERS))
        req_eqty.set("EMSX_ASSET_CLASS","EQTY")
        self.easymsx.submit_request(req_eqty, BrokerMessageHandler(self,"EQTY").process_message)

        req_opt = self.easymsx.emsx_service.createRequest(str(GET_BROKERS))
        req_opt.set("EMSX_ASSET_CLASS","OPT")
        self.easymsx.submit_request(req_opt, BrokerMessageHandler(self,"OPT").process_message)

        req_fut = self.easymsx.emsx_service.createRequest(str(GET_BROKERS))
        req_fut.set("EMSX_ASSET_CLASS","FUT")
        self.easymsx.submit_request(req_fut, BrokerMessageHandler(self,"FUT").process_message)
        
        req_multileg = self.easymsx.emsx_service.createRequest(str(GET_BROKERS))
        req_multileg.set("EMSX_ASSET_CLASS","MULTILEG_OPT")
        self.easymsx.submit_request(req_multileg, BrokerMessageHandler(self,"MULTILEG_OPT").process_message)

class BrokerMessageHandler:
    
    def __init__(self, brokers, asset_class):
        self.brokers = brokers
        self.asset_class = asset_class
        
    def process_message(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            error_code = msg.getElementAsInteger("ERROR_CODE")
            error_message = msg.getElementAsString("ERROR_MESSAGE")
            print("GetBrokers >> ERROR CODE: %d\tERROR MESSAGE: %s" % (error_code,error_message))

        elif msg.messageType() == GET_BROKERS:
            
            bkrs = msg.getElement("EMSX_BROKERS")
            
            for b in bkrs.values():
                self.brokers.brokers.append(Broker(self.brokers,b,self.asset_class))
                
                                  
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
