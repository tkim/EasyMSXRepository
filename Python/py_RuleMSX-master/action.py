'''
Created on 27 Nov 2017

@author: metz
'''
class Action:
    
    def __init__(self, name, executor=None):
        
        self.name = name
        self.actionExecutor = executor
        
    def execute(self, dataSet):
        if not self.actionExecutor == None:
            self.actionExecutor.execute(dataSet)
            

        
    