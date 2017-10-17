using System;
using System.Collections.Generic;
using static com.bloomberg.samples.rulemsx.RuleMSX;

namespace com.bloomberg.samples.rulemsx
{

    public abstract class DataPointSource
    {
        internal List<RuleEventHandler> ruleEventHandlers = new List<RuleEventHandler>();
        private DataPoint dataPoint;
        public abstract object GetValue();

        public void SetStale() {
            Log.LogMessage(Log.LogLevels.DETAILED, "SetState called for DataPointSource of DataPoint: " + this.dataPoint.GetName());
            foreach (RuleEventHandler h in ruleEventHandlers)
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Firing rule event handler");
                h.handleRuleEvent();
            }
        }

        internal void setDataPoint(DataPoint dataPoint) {
            this.dataPoint = dataPoint;
        }

        public DataPoint getDataPoint() {
            return this.dataPoint;
        }

        internal void addRuleEventHandler(RuleEventHandler handler) {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding rule event handler for : " + this.dataPoint.GetName());
            this.ruleEventHandlers.Add(handler);
        }
    }
}