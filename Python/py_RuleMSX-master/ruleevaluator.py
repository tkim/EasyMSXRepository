'''
Created on 28 Nov 2017

@author: metz
'''

class RuleEvaluator:
    
    def evaluate(self,dataSet):
        raise NotImplementedError("The evaluate function of a RuleEvaluator must be overridden")
    
    def setCondition(self, condition):
        self.condition = condition
        
    def addDependentDataPointName(self,dataPointName):
        try:
            self.dependentDataPointNames.append(dataPointName)
        except:
            self.dependentDataPointNames = []
            self.dependentDataPointNames.append(dataPointName)
