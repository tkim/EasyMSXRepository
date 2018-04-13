'''
Created on 27 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import logging

class DataPointSource:
    
    def __init__(self):
        
        logging.info("Initializing DataPointSource")

        self.datapoint  = None
        self.associated_working_rules = []
        self.is_stale = True
    
        logging.info("Initialized DataPointSource")
    
    def set_datapoint(self,datapoint):
        
        logging.info("Setting DataPoint for DataPointSource: " + datapoint.name)

        self.datapoint = datapoint
        
    
    def get_datapoint(self):

        logging.info("Get DataPointSource DataPoint for : " + self.datapoint.name)

        try:
            return self.datapoint
        except:
            self.datapoint=None
            return self.datapoint
    
    def get_value(self):
        
        logging.info("Get Value for DataPointSource of DataPoint: " + self.datapoint.name)
        
        raise NotImplementedError()
        
    
    def set_stale(self):
        
        logging.info("Set DataPointSource stale for DataPoint: " + self.datapoint.name)
        
        self.is_stale = True
        try:
            logging.info("Checking DataPointSource associated working rule for DataPoint: " + self.datapoint.name)

            for ar in self.associated_working_rules:
                logging.info("Call to enqueue WorkingRule for RuleSet: " + ar.ruleset.name + " and DataSet: " + ar.dataset.name)
                ar.enqueue_working_rule()
        except:
            #ignore
            self.associated_working_rules = []

        logging.info("Set DataPointSource stale for DataPoint: " + self.datapoint.name + " complete")
    
    def associate_working_rule(self,working_rule):

        logging.info("Associate WorkingRule for Rule: " + working_rule.rule.name + " of RuleSet: " + working_rule.rule.ruleset.name + " and DataSet: " + working_rule.dataset.name)

        try:
            self.associated_working_rules.append(working_rule)
        except:
            self.associated_working_rules = []
            self.associated_working_rules.append(working_rule)


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
