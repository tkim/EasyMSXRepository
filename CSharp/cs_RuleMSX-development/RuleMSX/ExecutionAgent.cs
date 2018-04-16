/* Copyright 2017. Bloomberg Finance L.P.

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
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace com.bloomberg.samples.rulemsx
{
    class ExecutionAgent {

        RuleSet ruleSet;
        static readonly object dataSetLock = new object();
        Thread workingSetAgent;
        volatile bool running = true;
        Queue<DataSet> dataSetQueue = new Queue<DataSet>();
        List<WorkingRule> openSetQueue = new List<WorkingRule>();
        List<WorkingRule> openSet = new List<WorkingRule>();
        static readonly object openSetLock = new object();
        List<WorkingRule> workingSet = new List<WorkingRule>();


        internal ExecutionAgent(RuleSet ruleSet, DataSet dataSet) {

            Log.LogMessage(Log.LogLevels.DETAILED, "ExecutionEngine constructor for RuleSet: " + ruleSet.GetName());

            this.ruleSet = ruleSet;

            AddDataSet(dataSet);

            Log.LogMessage(Log.LogLevels.DETAILED, "Creating thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());

            ThreadStart agent = new ThreadStart(WorkingSetAgent);
            workingSetAgent = new Thread(agent);
            Log.LogMessage(Log.LogLevels.DETAILED, "Starting thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());
            workingSetAgent.Start();
        }

        internal void AddDataSet (DataSet dataSet)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding DataSet to DataSet queue for RuleSet: " + this.ruleSet.GetName() + " and DataSet: " + dataSet.GetName());

            lock (dataSetLock)
            {
                dataSetQueue.Enqueue(dataSet);
            }
        }

        internal bool Stop() {
            Log.LogMessage(Log.LogLevels.DETAILED, "Stoping thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());
            this.running = false;
            try
            {
                workingSetAgent.Join();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal void WorkingSetAgent() {

            Log.LogMessage(Log.LogLevels.DETAILED, "Running WorkingSetAgent for " + ruleSet.GetName());

            while (running)
            {

                lock (dataSetLock)
                {
                    // Ingest any new DataSets
                    while (dataSetQueue.Count > 0) { 
                        DataSet ds = dataSetQueue.Dequeue();
                        ingestDataSet(ds);
                    }
                }

                while (openSetQueue.Count > 0)
                {

                    Log.LogMessage(Log.LogLevels.DETAILED, "OpenSetQueue not empty...");

                    lock (openSetLock)
                    {
                        Log.LogMessage(Log.LogLevels.DETAILED, "Move OpenSetQueue to OpenSet, reset OpenSetQueue");
                        openSet = openSetQueue;
                        openSetQueue = new List<WorkingRule>();
                    }

                    foreach (WorkingRule wr in openSet)
                    {

                        Log.LogMessage(Log.LogLevels.DETAILED, "Evaluating WorkingRule for Rule: " + wr.getRule().GetName() + " with DataSet: " + wr.dataSet.GetName());

                        bool res = true;

                        foreach(RuleEvaluator e in wr.evaluators)
                        {

                            Log.LogMessage(Log.LogLevels.DETAILED, "Calling evaluator for RuleCondition: " + e.ruleCondition.GetName());

                            if (!e.Evaluate(wr.dataSet))
                            {
                                res = false;
                                break;
                            }
                        }

                        Log.LogMessage(Log.LogLevels.DETAILED, "Checking results of rule evaluations...");

                        if (res)
                        {
                            Log.LogMessage(Log.LogLevels.DETAILED, "All RuleConditions returned true - Executing Actions");
                            foreach (ActionExecutor ex in wr.executors)
                            {
                                ex.Execute(wr.dataSet);
                            }
                        } else
                        {
                            Log.LogMessage(Log.LogLevels.DETAILED, "Evaluator returned false");
                        }
                    }

                    Log.LogMessage(Log.LogLevels.DETAILED, "Execution cycle complete, OpenSet empty");

                }
            }
        }

        private void ingestDataSet(DataSet dataSet)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Ingesting dataSet " + dataSet.GetName() + " for " + ruleSet.GetName() + " agent");

            foreach (Rule r in this.ruleSet.rules)
            {
                WorkingRule wr = new WorkingRule(r, dataSet, this);
                this.workingSet.Add(wr);
                this.EnqueueWorkingRule(wr);
            }

            Log.LogMessage(Log.LogLevels.DETAILED, "Finished Ingesting dataSet " + dataSet.GetName());

        }

        internal void EnqueueWorkingRule(WorkingRule wr)
        {

            lock (openSetLock)
            {
                if (!openSetQueue.Contains(wr))
                {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Enqueueing WorkingRule for: " + wr.getRule().GetName());
                    openSetQueue.Add(wr);
                } else
                {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Not Enqueueing WorkingRule for: " + wr.getRule().GetName() + " - already in queue.");
                }
            }
        }
    }
}
