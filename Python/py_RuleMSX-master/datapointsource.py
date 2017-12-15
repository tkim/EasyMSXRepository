'''
Created on 27 Nov 2017

@author: metz
'''

class DataPointSource:
    
    def __init__(self):
        self.dataPoint  = None
        self.associatedWorkingRules = []
        self.isStale = True
    
    
    def setDataPoint(self,dataPoint):
        self.dataPoint = dataPoint
        
    
    def getDataPoint(self):
        try:
            return self.dataPoint
        except:
            self.dataPoint=None
            return self.dataPoint
    
    def getValue(self):
        raise NotImplementedError()
        
    
    def setStale(self):
        self.isStale = True
        try:
            for ar in self.associatedWorkingRules:
                ar.enqueueWorkingRule()
        except:
            #ignore
            self.associatedWorkingRules = []
    
    def associateWorkingRule(self,workingRule):
        try:
            self.associatedWorkingRules.append(workingRule)
        except:
            self.associatedWorkingRules = []
            self.associatedWorkingRules.append(workingRule)
