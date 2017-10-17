using System;
using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{

    public class RuleMSX
    {

        public enum DataPointState
        {
            STALE,
            CURRENT
        }

        List<DataSet> dataSets;
        List<RuleSet> ruleSets;
        List<RuleAction> actions;

        public RuleMSX()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Instantiating RuleMSX");

            dataSets = new List<DataSet>();
            ruleSets = new List<RuleSet>();
            actions = new List<RuleAction>();

            Log.LogMessage(Log.LogLevels.BASIC, "Instantiating RuleMSX complete.");
        }

        public DataSet createDataSet(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating DataSet: " + name);
            if (name == null || name == "") throw new ArgumentException("DataSet name cannot be null or empty");
            DataSet newDataSet = new DataSet(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new DataSet " + newDataSet.getName() + " to DataSets list.");
            dataSets.Add(newDataSet);
            Log.LogMessage(Log.LogLevels.BASIC, "New DataSet created: " + newDataSet.getName());
            return newDataSet;
        }

        public RuleSet createRuleSet(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating RuleSet: " + name);
            if (name == null || name == "") throw new ArgumentException("DataSet name cannot be null or empty");
            RuleSet newRuleSet = new RuleSet(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new RuleSet " + newRuleSet.getName() + " to RuleSets list.");
            ruleSets.Add(newRuleSet);
            Log.LogMessage(Log.LogLevels.BASIC, "New RuleSet created: " + newRuleSet.getName());
            return newRuleSet;
        }

        public RuleAction createAction(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating Action: " + name);
            if (name == null || name == "") throw new ArgumentException("Action name cannot be null or empty");
            RuleAction newAction = new RuleAction(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new Action " + newAction.getName() + " to Actions list.");
            actions.Add(newAction);
            Log.LogMessage(Log.LogLevels.BASIC, "New Action created: " + newAction.getName());
            return newAction;
        }

        public RuleAction createAction(string name, ActionExecutor executor)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating Action: " + name + " with executor");
            if (name == null || name == "") throw new ArgumentException("Action name cannot be null or empty");
            if (executor == null) throw new ArgumentException("ActionExecutor cannot be null");
            RuleAction newAction = new RuleAction(name, executor);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new Action " + newAction.getName() + " to Actions list.");
            actions.Add(newAction);
            Log.LogMessage(Log.LogLevels.BASIC, "New Action created: " + newAction.getName() + " with executor");
            return newAction;
        }

        public List<DataSet> getDataSets()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get DataSets");
            return this.dataSets;
        }

        public List<RuleSet> getRuleSets()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get RuleSets");
            return this.ruleSets;
        }

        public List<RuleAction> getActions()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get Actions");
            return this.actions;
        }

        public RuleAction getAction(string name)
        {
            foreach(RuleAction a in actions) {
                if (a.getName().Equals(name)) return a;
            }
            return null;
        }

        public RuleSet getRuleSet(string name)
        {
            foreach (RuleSet rs in ruleSets)
            {
                if (rs.getName().Equals(name)) return rs;
            }
            return null;
        }

        public DataSet getDataSet(string name)
        {
            foreach (DataSet ds in dataSets)
            {
                if (ds.getName().Equals(name)) return ds;
            }
            return null;
        }

        public bool Stop()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Stopping all RuleSet agents.");
            bool result = true;

            foreach (RuleSet rs in this.ruleSets)
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Stopping RuleSet: " + rs.getName());
                if (!rs.Stop()) result = false;
            }

            return result;
        }
    }
}
