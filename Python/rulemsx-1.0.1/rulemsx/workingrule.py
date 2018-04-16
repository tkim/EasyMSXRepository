'''
Created on 28 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import logging

class WorkingRule:
    
    def __init__(self, rule, dataset, execagent):
        
        logging.info("Initializing WorkingRule for Rule: " + rule.name + " in RuleSet: " + rule.ruleset.name + " with DataSet: " + dataset.name)
        
        self.rule = rule
        self.dataset = dataset
        self.execagent = execagent
        self.executors = []
        self.evaluators = []
        self.dereference()
        
    
    def dereference(self):
        
        logging.info("Dereference WorkingRule for Rule: " + self.rule.name + " in RuleSet: " + self.rule.ruleset.name + " with DataSet: " + self.dataset.name)

        if not self.rule.actions == []:         
            for action in self.rule.actions:
                self.executors.append(action.action_executor)
            
        if not self.rule.rule_conditions == []:         
            for condition in self.rule.rule_conditions:
                self.evaluators.append(condition.evaluator)
                for dpn in condition.evaluator.dependent_datapoint_names:
                    dp = self.dataset.datapoints[dpn]
                    dp.datapoint_source.associate_working_rule(self)
                    
        
            
    def enqueue_working_rule(self):
        logging.info("Call to enqueue WorkingRule for Rule: " + self.rule.name + " in RuleSet: " + self.rule.ruleset.name + " with DataSet: " + self.dataset.name)
        self.execagent.enqueue_working_rule(self)
        logging.info("Enqueue complete for Rule: " + self.rule.name + " in RuleSet: " + self.rule.ruleset.name + " with DataSet: " + self.dataset.name)
            
    

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
