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
        List<Action> actions;

        public RuleMSX()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Instantiating RuleMSX");

            dataSets = new List<DataSet>();
            ruleSets = new List<RuleSet>();
            actions = new List<Action>();

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

        public Action createAction(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating Action: " + name);
            if (name == null || name == "") throw new ArgumentException("Action name cannot be null or empty");
            Action newAction = new Action(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new Action " + newAction.getName() + " to Actions list.");
            actions.Add(newAction);
            Log.LogMessage(Log.LogLevels.BASIC, "New Action created: " + newAction.getName());
            return newAction;
        }

        public Action createAction(string name, ActionExecutor executor)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating Action: " + name + " with executor");
            if (name == null || name == "") throw new ArgumentException("Action name cannot be null or empty");
            if (executor == null) throw new ArgumentException("ActionExecutor cannot be null");
            Action newAction = new Action(name, executor);
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

        public List<Action> getActions()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get Actions");
            return this.actions;
        }

        public Action getAction(string name)
        {
            foreach(Action a in actions) {
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
