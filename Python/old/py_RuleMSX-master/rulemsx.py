'''
Created on 25 Nov 2017

@author: Rik Clegg

'''

from dataset import DataSet
from ruleset import RuleSet
from action import Action

class RuleMSX:
    
    def __init__(self):
        self.ruleSets = {}
        self.dataSets = {}
        self.actions = {}
    

    def createDataSet(self,name):
        
        if(name is None or name == ""):
            raise ValueError("DataSet name cannot be none or empty")
        
        ds = DataSet(name)
        self.dataSets[name] = ds
        return ds
        
        
    def createRuleSet(self,name):
    
        if(name is None or name == ""):
            raise ValueError("RuleSet name cannot be none or empty")
        
        rs = RuleSet(name)
        self.ruleSets[name] = rs
        return rs

    
    def createAction(self,name):
        
        if(name is None or name == ""):
            raise ValueError("Action name cannot be none or empty")
        
        a = Action(name)
        self.actions[name] = a
        return a
    
    
    def stop(self):
        
        result = True
        
        for rs in self.ruleSets:
            if not rs.stop(): result = False
            
        return result
        