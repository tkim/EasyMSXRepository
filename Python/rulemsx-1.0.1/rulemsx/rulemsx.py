"""
.. module:: rulemsx
   :platform: Unix, Windows
   :synopsis: RuleMSX rule engine

RuleMSX is designed for integration with the Bloomberg API using EasyMSX 
and EasyMSX to build applications around a set of user defined rules,
conditions and actions. It can also be used in a standalone fashion 
without connectivity to Bloomberg.

This version requires Python 3 or later

.. moduleauthor:: Richard Clegg <rclegg2@bloomberg.net>

"""

import logging
from .dataset import DataSet
from .ruleset import RuleSet
from .action import Action

class RuleMSX:
    
    def __init__(self,lvl=logging.CRITICAL):
        self.set_log_level(lvl)
        #logging.logger = logging.getLogger(__name__)

        logging.info("Initializing sets")
        
        self.rulesets = {}
        self.datasets = {}
        self.actions = {}
    
    def set_log_level(self,lvl):
        logging.basicConfig(level=lvl)

    def create_dataset(self,name):
        
        """ Create a new dataset object and add it to the dataset collection

        Args:
           name (str):  The name of the new dataset

        Returns:
           str.  The new dataset object

        Raises:
            ValueError
            
        The name cannot be None or empty
        """
        
        if(name is None or name == ""):
            raise ValueError("DataSet name cannot be none or empty")
        
        logging.info("Creating DataSet: " + name)

        ds = DataSet(name)
        self.datasets[name] = ds

        logging.info("Created DataSet: " + name)
        
        return ds

        
        
    def create_ruleset(self,name):
    
        if(name is None or name == ""):
            raise ValueError("RuleSet name cannot be none or empty")
        
        logging.info("Creating RuleSet: " + name)

        rs = RuleSet(name)
        self.rulesets[name] = rs

        logging.info("Created RuleSet: " + name)
        
        return rs

    
    def create_action(self,name, executor):
        
        logging.info("Creating Action: " + name)
        
        if(name is None or name == ""):
            raise ValueError("Action name cannot be none or empty")
        
        a = Action(name,executor)
        self.actions[name] = a

        logging.info("Created Action: " + name)

        return a
    
    
    def stop(self):
        
        logging.info("Stopping RuleMSX")
        
        result = True
        
        for rs in self.rulesets.values():
            if not rs.stop(): result = False
            
        logging.info("Stopped RuleMSX")

        return result
        
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

