'''
Created on 27 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import logging
from .rule import Rule
from .executionagent import ExecutionAgent

class RuleSet:
    
    def __init__(self,name):
        
        logging.info("Initializing RuleSet: " + name)
        
        self.name = name
        self.rules = {}
        self.execution_agent = None

        logging.info("Initialized RuleSet: " + name)

    def stop(self):

        logging.info("Stopping  executionAgent for RuleSet: " + self.name)

        if not self.execution_agent == None:
            self.execution_agent.stop()
            
        return True
        
    def add_rule(self,name):
        
        logging.info("Adding Rule: " + name + " to RuleSet: " + self.name)

        if(name is None or name == ""):
            raise ValueError("Rule name cannot be none or empty")
        
        r = Rule(self, name)
        
        self.rules[name] = r
        return r

    def execute(self,dataset):
        
        if self.execution_agent == None:

            logging.info("Execute RuleSet: " + self.name + " with DataSet: " + dataset.name)

            self.execution_agent = ExecutionAgent(self, dataset)
        
        else:
            
            logging.info("Add DataSet: " + dataset.name + " to ExecutionAgent for RuleSet: " + self.name)

            self.execution_agent.add_dataset(dataset)
        

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
