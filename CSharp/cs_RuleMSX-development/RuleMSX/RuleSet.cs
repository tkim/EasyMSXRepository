using System;
using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class RuleSet {

        private string name;
        private ExecutionAgent executionAgent = null;
        internal List<Rule> rules = new List<Rule>();

        internal RuleSet(string name) {
            Log.LogMessage(Log.LogLevels.DETAILED, "RuleSet constructor: " + name);
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }

        public bool Stop()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Stoping ExecutionAgent for RuleSet " + this.name);
            if (this.executionAgent != null) return (this.executionAgent.Stop());
            else return true;
        }

        public Rule AddRule(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Adding Rule: " + name);
            if (name == null || name == "") throw new ArgumentException("Rule name cannot be null or empty");
            Rule newRule = new Rule(this, name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new Rule " + newRule.GetName() + " to RuleSet " + this.name);
            rules.Add(newRule);
            Log.LogMessage(Log.LogLevels.BASIC, "New Rule Added: " + newRule.GetName());
            return newRule;
        }

        public List<Rule> GetRules()
        {
            return this.rules;
        }

        public Rule GetRule(string name)
        {
            foreach (Rule r in this.rules)
            {
                if (r.GetName().Equals(name)) return r;
            }
            return null;
        }


        public void Execute(DataSet dataSet) {

            Log.LogMessage(Log.LogLevels.BASIC, "Execute RuleSet " + this.name + " with DataSet " + dataSet.GetName());

            if (this.executionAgent == null) {
                Log.LogMessage(Log.LogLevels.DETAILED, "First execution, creating ExecutionAgent");
                this.executionAgent = new ExecutionAgent(this, dataSet);
            } else {
                Log.LogMessage(Log.LogLevels.DETAILED, "RuleSet already has ExecutionAgent, adding DataSet " + dataSet.GetName());
                this.executionAgent.AddDataSet(dataSet);
            }
        }
    }
}