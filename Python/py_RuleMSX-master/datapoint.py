'''
Created on 27 Nov 2017

@author: metz
'''
from datapointsource import DataPointSource

class DataPoint:
    
    def __init__(self, dataSet, name, dataPointSource=None):
        
        self.name = name
        
        self._dataSet = dataSet
        
        if not dataPointSource == None:
            self.setDataPointSource(dataPointSource)
            
    def setDataPointSource(self, dataPointSource):

        if dataPointSource == None:
            raise ValueError("Invalid dataPointSource")

        if not isinstance(dataPointSource, DataPointSource) :
            raise TypeError("Not a valid DataPointSource")
        
        dataPointSource.setDataPoint(self)
        self.dataPointSource = dataPointSource

    
    def getValue(self):
        return self.dataPointSource.getValue()
        
    