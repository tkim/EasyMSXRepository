'''
Created on 28 Nov 2017

@author: metz
'''

class Rule:
    
    def __init__(self, ruleSet, name):
        
        self.ruleSet = ruleSet
        self.name = name
        self.ruleConditions = []
        self.actions = []
        
    
    def addRuleCondition(self, ruleCondition):
        
        if ruleCondition == None:
            raise ValueError("RuleCondition cannot be none")
        
        self.ruleConditions.append(ruleCondition)
        
    
    def evaluate(self):
        
        for rc in self.ruleConditions:
            
            if not rc.evaluate():
                return False
            
        return True
    
    def addAction(self,ruleAction):
        
        if ruleAction is None:
            raise ValueError("RuleAction cannot be None")
        
        self.actions.append(ruleAction)
        
    