'''
Created on 28 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''

import logging

class RuleCondition:
    
    def __init__(self, name, evaluator):
        
        logging.info("Initializing RuleCondition: " + name)
                
        if name == "" or name is None:
            raise ValueError("RuleCondition name cannot be empty or None")
 
        if evaluator == None:
            raise ValueError("RuleCondition evaluator cannot be None")

        self.name = name
        self.evaluator = evaluator
        self.evaluator.set_condition(self)

        logging.info("Initialized RuleCondition: " + name)
        
        
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


    
        
        