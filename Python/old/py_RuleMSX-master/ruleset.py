'''
Created on 27 Nov 2017

@author: metz
'''

from rule import Rule
from executionagent import ExecutionAgent

class RuleSet:
    
    def __init__(self,name):
        
        self.name = name
        self.rules = {}
        self.executionAgent = None

    def stop(self):
        return True
    
    
    def addRule(self,name):
        
        if(name is None or name == ""):
            raise ValueError("Rule name cannot be none or empty")
        
        r = Rule(self, name)
        
        self.rules[name] = r
        return r

    def execute(self,dataSet):
        
        if self.executionAgent == None:

            self.executionAgent = ExecutionAgent(self, dataSet)
        
        else:
            
            self.executionAgent.addDataSet(dataSet)
        