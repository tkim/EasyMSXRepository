using com.bloomberg.samples.rulemsx;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RuleMSXNUnitIntegrationTest
{
    [TestFixture]
    public class RuleMSXNUnitIntegrationTest
    {

        [Test]
        public void RuleSetReportingSizeCheckSingleRule()
        {
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "TestRuleSet";
            string newDataSetName = "TestDataSet";
            string newRuleName = "IsBooleanTrue";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            DataSet ds = rmsx.createDataSet(newDataSetName);

            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            rs.AddRule(r);

            string report = rs.report();

            System.Console.WriteLine(report);

            Assert.That(report.Length, Is.EqualTo(126));

        }

        [Test]
        public void RuleSetReportingSizeCheckMultiTop()
        {
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "TestRuleSet";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);

            rs.AddRule(new Rule("TestRule1", new GenericBoolRule(true), rmsx.createAction("TestAction1", new GenericAction("TestAction1"))));
            Rule r2 = new Rule("TestRule2", new GenericBoolRule(false), rmsx.createAction("TestAction4", new GenericAction("TestAction4")));
            rs.AddRule(r2);
            r2.AddRule(new Rule("TestRule5", new GenericBoolRule(true), rmsx.createAction("TestAction3", new GenericAction("TestAction3"))));
            r2.AddRule(new Rule("TestRule6", new GenericBoolRule(false), rmsx.createAction("TestAction5", new GenericAction("TestAction5"))));
            rs.AddRule(new Rule("TestRule3", new GenericBoolRule(false)));
            Rule r4 = new Rule("TestRule4", new GenericBoolRule(true), rmsx.createAction("TestAction2", new GenericAction("TestAction2")));
            rs.AddRule(r4);
            r4.AddRule(new Rule("TestRule7", new GenericBoolRule(false), rmsx.createAction("TestAction6", new GenericAction("TestAction6"))));
            r4.AddRule(new Rule("TestRule8", new GenericBoolRule(true), rmsx.createAction("TestAction7", new GenericAction("TestAction7"))));

            string report = rs.report();

            System.Console.WriteLine(report);

            Assert.That(report.Length, Is.EqualTo(664));

        }


        [Test]
        public void SingleRuleITest()
        {

            Log.logLevel = Log.LogLevels.DETAILED;
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "SingleIterationRuleSet";
            string newDataSetName = "SingleIterationDataSet";
            string newRuleName = "IsBooleanTrue";
            string actionMessage = "ActionMessage";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            DataSet ds = rmsx.createDataSet(newDataSetName);

            RuleAction rai = rmsx.createAction("RuleActionIn", new GenericAction(actionMessage));

            Rule r = new Rule(newRuleName, new GenericBoolRule(true), rai);
            rs.AddRule(r);

            ds.addDataPoint("TestDataPoint", new StringDataPoint("test_data_point"));

            rs.Execute(ds);

            // We have to wait for the action to take place. Under normal circumstances there 
            // would be no interation with the data from outside the evaluators/actions.
            System.Threading.Thread.Sleep(100);

            GenericAction rao = (GenericAction)rai.getExecutor();
            Assert.That(rao.getOutgoing(), Is.EqualTo(actionMessage));
        }

        private class GenericBoolRule : RuleEvaluator
        {
            bool ret;

            public GenericBoolRule(bool returnValue)
            {
                this.ret = returnValue;
                if (returnValue)
                {
                    this.addDependantDataPointName("TestDependency1");
                    this.addDependantDataPointName("TestDependency2");
                }
            }

            public override bool Evaluate(DataSet dataSet)
            {
                return this.ret;
            }
        }

        private class GenericAction : ActionExecutor
        {
            string message;
            string outgoing="unset";

            public GenericAction(string message)
            {
                this.message = message;
            }
            public void Execute(DataSet dataSet)
            {
                this.outgoing = message;
            }

            public string getOutgoing()
            {
                return this.outgoing;
            }
        }

        private class StringDataPoint : DataPointSource
        {
            private string val;

            public StringDataPoint(string val)
            {
                this.val = val;
            }
            public override object GetValue()
            {
                return val;
            }
        }
    }
}
