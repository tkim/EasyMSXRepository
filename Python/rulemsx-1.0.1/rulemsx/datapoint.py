'''
Created on 27 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''

import logging
from .datapointsource import DataPointSource

class DataPoint:
    
    def __init__(self, dataset, name, datapoint_source=None):
        
        logging.info("Initializing DataPoint: " + name)
        
        self.name = name
        
        self._dataset = dataset
        
        if not datapoint_source == None:
            self.set_datapoint_source(datapoint_source)

        logging.info("Initialized DataPoint: " + name)
            
    def set_datapoint_source(self, datapoint_source):

        logging.info("Set DataPointSource for DataPoint: " + self.name)

        if datapoint_source == None:
            raise ValueError("Invalid dataPointSource")

        if not isinstance(datapoint_source, DataPointSource) :
            raise TypeError("Not a valid DataPointSource")
        
        datapoint_source.set_datapoint(self)
        self.datapoint_source = datapoint_source

        logging.info("DataPointSource Set for DataPoint: " + self.name)
    
    def get_value(self):

        logging.info("Getting value for DataPoint: " + self.name)
        
        return self.datapoint_source.get_value()
        
    
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
