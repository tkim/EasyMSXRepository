'''
Created on 28 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import logging

class Rule:
    
    def __init__(self, ruleset, name):
        
        logging.info("Initializing Rule: " + name + " for RuleSet: " + ruleset.name)
        
        self.ruleset = ruleset
        self.name = name
        self.rule_conditions = []
        self.actions = []
        
        logging.info("Initialized Rule: " + name)

    
    def add_rule_condition(self, rule_condition):
        
        logging.info("Adding RuleCondition: " + rule_condition.name + " for Rule: " + self.name)

        if rule_condition == None:
            raise ValueError("RuleCondition cannot be none")
        
        self.rule_conditions.append(rule_condition)
        
    
    def evaluate(self):
        
        logging.info("Evaluating Rule: " + self.name + " in RuleSet: " + self.ruleset.name)

        for rc in self.rule_conditions:
            
            if not rc.evaluate():
                return False
            
        return True
    
    def add_action(self,rule_action):
        
        logging.info("Add Action: " + rule_action.name + " to Rule: " + self.name + " in RuleSet: " + self.ruleset.name)

        if rule_action is None:
            raise ValueError("RuleAction cannot be None")
        
        self.actions.append(rule_action)
        

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

    