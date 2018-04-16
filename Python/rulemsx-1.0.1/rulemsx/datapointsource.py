'''
Created on 27 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import logging

class DataPointSource:
    
    def init(self):

        logging.info("Initializing DataPointSource")

        self.datapoint  = None
        self.associated_working_rules = []
        self.is_stale = True
    
        logging.info("Initialized DataPointSource")
    
    def set_datapoint(self,datapoint):
        
        logging.info("Setting DataPoint for DataPointSource: " + datapoint.name)

        try:
            self.datapoint
        except:
            self.init()

        self.datapoint = datapoint
        
    
    def get_datapoint(self):

        logging.info("Get DataPointSource DataPoint for : " + self.datapoint.name)

        try:
            self.datapoint
        except:
            self.init()

        return self.datapoint
    
    def get_value(self):
        
        logging.info("Get Value for DataPointSource of DataPoint: " + self.datapoint.name)
        
        raise NotImplementedError()
        
    
    def set_stale(self):
        
        logging.info("Set Stale called for DataPoint: " + self.datapoint.name)
        
        try:
            self.datapoint
        except:
            self.init()
        
        self.is_stale = True

        logging.info("Enqueuing all associated working rules for DataPoint: " + self.datapoint.name)

        for ar in self.associated_working_rules:
            ar.enqueue_working_rule()

        logging.info("Set Stale complete for DataPoint: " + self.datapoint.name)


    def associate_working_rule(self,working_rule):

        logging.info("Associate WorkingRule for Rule: " + working_rule.rule.name + " of RuleSet: " + working_rule.rule.ruleset.name + " and DataSet: " + working_rule.dataset.name)

        try:
            self.datapoint
        except:
            self.init()

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
