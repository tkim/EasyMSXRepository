using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.bloomberg.samples.rulemsx
{
    class WorkingRule : RuleEventHandler
    {
        ExecutionAgent agent;
        Rule rule;
        internal DataSet dataSet;
        internal RuleEvaluator evaluator;
        internal List<ActionExecutor> actionExecutors = new List<ActionExecutor>();
        internal List<WorkingRule> workingRules = new List<WorkingRule>();

        internal WorkingRule(ExecutionAgent agent, Rule rule, DataSet dataSet) {
            Log.LogMessage(Log.LogLevels.DETAILED, "WorkingRule constructor for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());
            this.agent = agent;
            this.rule = rule;
            this.dataSet = dataSet;

            dereference();
        }

        private void dereference()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Dereferencing WorkingRule for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());

            foreach(RuleAction a in rule.GetActions())
            {
                this.actionExecutors.Add(a.getExecutor());
            }
            this.evaluator = rule.GetEvaluator();
            
            foreach(string dependencyName in this.evaluator.dependantDataPointNames)
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Connecting WorkingRule Dependencies for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());

                // Find this dependency in the current dataSet
                DataPoint dp = this.dataSet.getDataPoint(dependencyName);
                if(dp!=null) dp.GetSource().addRuleEventHandler(this);
            }
        }

        internal Rule getRule() {
            return this.rule;
        }

        internal void addWorkingRule(WorkingRule wr) {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding child WorkingRule to Dependencies for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());
            this.workingRules.Add(wr);
        }

        public void handleRuleEvent()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding WorkingRule to OpenSet for " + rule.GetName() + " and DataSet: " + dataSet.getName());
            agent.AddToOpenSetQueue(this);
        }
    }
}
