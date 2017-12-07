'''
Created on 28 Nov 2017

@author: metz
'''

import threading
import queue
from workingrule import WorkingRule

class ExecutionAgent:
    
    class WorkingSetAgent(threading.Thread):
        
        def __init__(self,execAgent):
            self.running = True
            self.openSetQueue = []
            self.openSet = []
            self.execAgent= execAgent
            self.workingSet = []
            self.lock = threading.Lock()
            threading.Thread.__init__(self)
            
        def run(self):
            
            while(self.running):

                with self.lock:
                    while self.execAgent.dataSetQueue.qsize() >0:
                        ds = self.execAgent.dataSetQueue.get()
                        self.ingestDataSet(ds)
            
                    
                while len(self.openSetQueue) > 0:
                    
                    with self.lock:
                        self.openSet = list(self.openSetQueue)
                        self.openSetQueue = []
                    
                    for wr in self.openSet:
                        res = True
                        for e in wr.evaluators:
                            if not e.evaluate(wr.dataSet): res = False
                        
                        if res:
                            for x in e.executors:
                                x.execute(wr.dataSet)
                        
                
        def ingestDataSet(self,dataSet):
                
            for k,r in self.execAgent.ruleSet.rules.items():
                wr = WorkingRule(r,dataSet)
                self.workingSet.append(wr)
                self.openSet.append(wr)
            
    
    def __init__(self,ruleSet, dataSet=None):
        
        self.ruleSet = ruleSet
        
        self.dataSetQueue = queue.Queue()

        if not dataSet == None:
            self.addDataSet(dataSet)
        
        self.workingSetAgent = self.WorkingSetAgent(self)
        self.workingSetAgent.start()
        
    def stop(self):
        self.workingSetAgent.running=False
        try:
            self.workingSetAgent.join()
        except:
            return False

        return True
            
    def addDataSet(self,dataSet):
        
        self.dataSetQueue.put(dataSet)
        