'''
Created on 27 Nov 2017

@author: metz
'''

class DataPointSource:
    
    def __init__(self):
        self.dataPoint  = None
        self.ruleEventHandlers = []
        self.isStale = True
    
    
    def setDataPoint(self,dataPoint):
        self.dataPoint = dataPoint
        
    
    def getDataPoint(self):
        return self.dataPoint
    
    
    def getValue(self):
        raise NotImplementedError()
        
    
    def setStale(self):
        self.isStale = True
        for handler in self.ruleEventHandlers:
            handler.handleRuleEvent()
    
    
    def addRuleEventHandler(self,handler):
        self.ruleEventHandlers.append(handler)
        