'''
Created on 27 Nov 2017

@author: metz
'''

from datapoint import DataPoint

class DataSet:
    
    def __init__(self,name):
        
        self.name = name
        self.dataPoints = {}
        
    def addDataPoint(self,name, dataPointSource=None):
        
        if(name is None or name == ""):
            raise ValueError("DataPoint name cannot be none or empty")
        
        dp = DataPoint(self, name)
        
        if not dataPointSource == None:
            dp.setDataPointSource(dataPointSource)
        
        self.dataPoints[name] = dp
        return dp
