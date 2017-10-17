using System;
using System.Collections.Generic;
using System.Threading;

namespace com.bloomberg.samples.rulemsx
{
    class ExecutionAgent {

        static readonly object dataSetLock = new object();
        static readonly object openSetLock = new object();
        List<WorkingRule> workingSet = new List<WorkingRule>();
        List<WorkingRule> openSetQueue = new List<WorkingRule>();
        List<WorkingRule> openSet = new List<WorkingRule>();
        Queue<DataSet> dataSetQueue = new Queue<DataSet>();
        Thread workingSetAgent;
        RuleSet ruleSet;
        volatile bool stop = false;

        internal ExecutionAgent(RuleSet ruleSet, DataSet dataSet) {

            Log.LogMessage(Log.LogLevels.DETAILED, "ExecutionEngine constructor for RuleSet: " + ruleSet.getName());

            this.ruleSet = ruleSet;

            addDataSet(dataSet);

            Log.LogMessage(Log.LogLevels.DETAILED, "Creating thread for WorkingSetAgent for RuleSet: " + ruleSet.getName());

            ThreadStart agent = new ThreadStart(WorkingSetAgent);
            workingSetAgent = new Thread(agent);
            Log.LogMessage(Log.LogLevels.DETAILED, "Starting thread for WorkingSetAgent for RuleSet: " + ruleSet.getName());
            workingSetAgent.Start();

        }

        internal void addDataSet (DataSet dataSet)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding DataSet to DataSet queue for RuleSet: " + this.ruleSet.getName() + " and DataSet: " + dataSet.getName());

            lock (dataSetLock)
            {
                dataSetQueue.Enqueue(dataSet);
            }
        }

        internal bool Stop() {
            Log.LogMessage(Log.LogLevels.DETAILED, "Stoping thread for WorkingSetAgent for RuleSet: " + ruleSet.getName());
            this.stop = true;
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

            Log.LogMessage(Log.LogLevels.DETAILED, "Running WorkingSetAgent for " + ruleSet.getName());

            while (!stop)
            {

                // Ingest any new DataSets
                while (dataSetQueue.Count > 0)
                {
                    DataSet ds;
                    lock (dataSetLock) {
                        ds = dataSetQueue.Dequeue();
                        ingestDataSet(this.ruleSet, ds, null);
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

                        Log.LogMessage(Log.LogLevels.DETAILED, "Evaluating WorkingRule for Rule: " + wr.getRule().GetName() + " with DataSet: " + wr.dataSet.getName());
                        if (wr.evaluator.Evaluate(wr.dataSet))
                        {
                            Log.LogMessage(Log.LogLevels.DETAILED, "Evaluator returned True");
                            foreach (WorkingRule nwr in wr.workingRules) {
                                Log.LogMessage(Log.LogLevels.DETAILED, "Add WorkingRule for Rule: " + nwr.getRule().GetName() + " to OpenSetQueue");
                                AddToOpenSetQueue(nwr);
                            }
                            foreach (ActionExecutor a in wr.actionExecutors) {
                                Log.LogMessage(Log.LogLevels.DETAILED, "Executing Action for Rule: " + wr.getRule().GetName());
                                a.Execute(wr.dataSet);
                            }
                        }
                        else Log.LogMessage(Log.LogLevels.DETAILED, "Evaluator returned False");

                    }
                }
            }
        }

        private void ingestDataSet(RuleContainer rc, DataSet dataSet, WorkingRule parent)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Ingesting dataSet " + dataSet.getName() + " for " + ruleSet.getName() + " agent");

            // Create WorkingRule object for each DataPoint of each DataSet for each Rule in RuleSet.
            foreach (Rule r in rc.GetRules())
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Creating WorkingRule for Rule: " + r.GetName());
                WorkingRule wr = new WorkingRule(this, r, dataSet);
                workingSet.Add(wr);
                if (parent != null) {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Adding WorkingRule to parent");
                    parent.addWorkingRule(wr);
                }
                ingestDataSet(r, dataSet, wr);
                if (rc is RuleSet)
                {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Adding WorkingRule to OpenSetQueue");
                    AddToOpenSetQueue(wr); // Add alpha nodes to openSet queue
                }
            }
        }

        internal void AddToOpenSetQueue(WorkingRule wr)
        {
            lock(openSetLock) {
                if (!openSet.Contains(wr)) {
                    Log.LogMessage(Log.LogLevels.DETAILED, "...done.");
                    openSetQueue.Add(wr); //Only add working rule if it's not already in the queue
                }
                else Log.LogMessage(Log.LogLevels.DETAILED, "...ignored (already in queue)");
            }
        }

    }
}
