'''
Created on 28 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''

import logging
import threading
import queue
from .workingrule import WorkingRule

class ExecutionAgent:
    
    class WorkingSetAgent(threading.Thread):
        
        def __init__(self,execagent):

            logging.info("Initializing WorkingSetAgent for RuleSet: " + execagent.ruleset.name)

            self.running = True
            self.openset_queue = []
            self.openset = []
            self.execagent= execagent
            self.workingset = []
            self.lock = threading.Lock()
            threading.Thread.__init__(self)
            
            logging.info("Initialized WorkingSetAgent for RuleSet: " + execagent.ruleset.name)

        def run(self):
            
            logging.info("Running WorkingSetAgent for RuleSet: " + self.execagent.ruleset.name)

            iteration_count = 0
            while(self.running):
                
                iteration_count=iteration_count+1
                
                with self.lock:
                    while self.execagent.dataset_queue.qsize() >0:
                        ds = self.execagent.dataset_queue.get()
                        self.ingest_dataset(ds)
            
                    
                while len(self.openset_queue) > 0:
                          
                    logging.info("WorkingAgent for: " + self.execagent.ruleset.name + " iteration count: " + str(iteration_count))
                    
                    with self.lock:

                        logging.info("Migrate OpenSetQueue to OpenSet in WorkingAgent for: " + self.execagent.ruleset.name)
                        
                        self.openset = list(self.openset_queue)
                        self.openset_queue = []
                    
                    for wr in self.openset:

                        logging.info("Traverse OpenSet[" + str(len(self.openset)) + "] in WorkingAgent for: " + self.execagent.ruleset.name)

                        res = True
                        for e in wr.evaluators:
                            if not e.evaluate(wr.dataset): 
                                res = False
                                break
                            
                        if res:
                            for x in wr.executors:
                                x.execute(wr.dataset)
                        
                
        def ingest_dataset(self,dataset):
                
            logging.info("Ingest DataSet: " + dataset.name + " for " + self.execagent.ruleset.name)

            for r in self.execagent.ruleset.rules.values():

                logging.info("Build WorkingRule for Rule: " + r.name)

                wr = WorkingRule(r,dataset, self)
                self.workingset.append(wr)
                self.enqueue_working_rule(wr)
            
    
        def enqueue_working_rule(self,wr):

            # only insert if not already in the queue
            if wr not in self.openset_queue:
                logging.info("Enqueue WorkingRule for Rule: " + wr.rule.name + " of RuleSet: " + wr.rule.ruleset.name + " and DataSet: " + wr.dataset.name)
                self.openset_queue.append(wr)
            else:
                logging.info("Not Enqueuing WorkingRule for Rule: " + wr.rule.name + " of RuleSet: " + wr.rule.ruleset.name + " and DataSet: " + wr.dataset.name + " - already in open set.")
                

    def __init__(self, ruleset, dataset=None):
        
        logging.info("Initializing ExecutionAgent for: " + ruleset.name)
        
        self.ruleset = ruleset
        
        self.dataset_queue = queue.Queue()
        

        if not dataset == None:
            self.add_dataset(dataset)
        
        self.workingset_agent = self.WorkingSetAgent(self)
        self.workingset_agent.start()

        logging.info("Initialized ExecutionAgent for: " + ruleset.name)
        
    def stop(self):

        logging.info("Stopping ExecutionAgent for: " + self.ruleset.name)

        self.workingset_agent.running=False
        try:
            self.workingset_agent.join()
        except:
            return False

        return True
            
    def add_dataset(self,dataset):
        
        logging.info("Adding DataSet: " + dataset.name + " to dataSetQueue in ExecutionAgent for: " + self.ruleset.name)

        self.dataset_queue.put(dataset)
        
        
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
