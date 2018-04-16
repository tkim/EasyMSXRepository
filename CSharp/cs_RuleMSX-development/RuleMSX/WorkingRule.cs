using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.bloomberg.samples.rulemsx
{
    class WorkingRule
    {
        ExecutionAgent agent;
        Rule rule;
        internal DataSet dataSet;
        internal List<RuleEvaluator> evaluators = new List<RuleEvaluator>();
        internal List<ActionExecutor> executors = new List<ActionExecutor>();

        internal WorkingRule(Rule rule, DataSet dataSet, ExecutionAgent agent) {
            Log.LogMessage(Log.LogLevels.DETAILED, "WorkingRule constructor for Rule: " + rule.GetName() + " and DataSet: " + dataSet.GetName());
            this.agent = agent;
            this.rule = rule;
            this.dataSet = dataSet;
            Dereference();
        }

        private void Dereference()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Dereferencing WorkingRule for Rule: " + rule.GetName() + " and DataSet: " + dataSet.GetName());

            foreach(Action a in rule.GetActions())
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Adding Executor for: " + a.GetName());
                this.executors.Add(a.GetExecutor());
            }

            foreach(RuleCondition c in rule.GetConditions())
            {
                RuleEvaluator e = c.GetEvaluator();
                Log.LogMessage(Log.LogLevels.DETAILED, "Adding Evaluator for: " + c.GetName());
                this.evaluators.Add(e);

                foreach(String dpn in e.dependantDataPointNames)
                {
                    DataPoint dp = this.dataSet.GetDataPoint(dpn);
                    Log.LogMessage(Log.LogLevels.DETAILED, "Adding dependent DataPointSource for: " + dp.GetName());
                    dp.GetSource().AssociateWorkingRule(this);
                }
            }
        }

        internal Rule getRule() {
            return this.rule;
        }

        internal void EnqueueWorkingRule()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Call to enqueue WorkingRule for Rule : " + this.rule.GetName());
            this.agent.EnqueueWorkingRule(this);
        }
    }
}
