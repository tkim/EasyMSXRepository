'''
Created on 28 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''

import logging

class RuleEvaluator:
    
    def evaluate(self,dataset):
        raise NotImplementedError("The evaluate function of a RuleEvaluator must be overridden")
    
    def set_condition(self, condition):
        self.condition = condition
        
    def add_dependent_datapoint_name(self,datapoint_name):
        
        try:
            logging.info("Add dependent DataPoint name: " + datapoint_name + " for RuleCondition: " + self.condition.name)
        except:
            logging.info("Add dependent DataPoint name: " + datapoint_name + " for RuleCondition: unknown")
            
        try:
            self.dependent_datapoint_names.append(datapoint_name)
        except:
            self.dependent_datapoint_names = []
            self.dependent_datapoint_names.append(datapoint_name)


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
