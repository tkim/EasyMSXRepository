# Teams.py

import blpapi
from Team import Team

GET_TEAMS = blpapi.Name("GetTeams")
ERROR_INFO = blpapi.Name("ErrorInfo")

class Teams:
    
    def __init__(self,easyMSX):
        self.easyMSX = easyMSX
        self.teams=[]
        self.loadTeams()

    def __iter__(self):
        return self.teams.__iter__()

    def loadTeams(self):
        
        request = self.easyMSX.emsxService.createRequest(str(GET_TEAMS))
        self.easyMSX.submitRequest(request, self.processMessage)
        
    def processMessage(self, msg):
        
        if msg.messageType() == ERROR_INFO:
            errorCode = msg.getElementAsInteger("ERROR_CODE")
            errorMessage = msg.getElementAsString("ERROR_MESSAGE")
            print("GetTeams >> ERROR CODE: %d\tERROR MESSAGE: %s" % (errorCode,errorMessage))

        elif msg.messageType() == GET_TEAMS:
            
            tms = msg.getElement("TEAMS")
            
            for t in tms.values():
                self.teams.append(Team(self,t))
                
    def get(self,teamname):
        for t in self.teams:
            if t.name == teamname:
                return t
        
        return None
                                  
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
