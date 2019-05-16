using System;
using com.bloomberg.samples.rulemsx;
using NUnit.Framework;

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

        [Test]
        public void CounterTest()
        {

            Log.logLevel = Log.LogLevels.BASIC;

            RuleMSX rmsx = new RuleMSX();

            RuleSet ruleSet = rmsx.createRuleSet("CounterTestRuleSet");
            DataSet dataSet = rmsx.createDataSet("CounterTestDataSet");

            dataSet.addDataPoint("counter", new GenericIntDataPointSource(0));

            int maxVal = 10000;

            RuleAction counterSignalAndStep10 = rmsx.createAction("CounterSignalAndStep10", new CounterSignalAndStep(10));
            RuleAction counterSignalAndStep100 = rmsx.createAction("CounterSignalAndStep100", new CounterSignalAndStep(100));
            RuleAction counterSignalAndStep1000 = rmsx.createAction("CounterSignalAndStep1000", new CounterSignalAndStep(1000));
            RuleAction counterSignalAndStepMax = rmsx.createAction("CounterSignalAndStepMax", new CounterSignalAndStep(maxVal));
            EqualSignal equalSignal = new EqualSignal();
            RuleAction equalSignalMax = rmsx.createAction("EqualSignalMax", equalSignal);

            Rule lessThan10 = new Rule("LessThan10", new IntMinMaxEvaluator(0, 9),counterSignalAndStep10);
            Rule greaterThanOrEqualTo10LessThan100 = new Rule("GreaterThanOrEqualTo10LessThan100", new IntMinMaxEvaluator(10, 99), counterSignalAndStep100);
            Rule greaterThanOrEqualTo100LessThan1000 = new Rule("GreaterThanOrEqualTo100LessThan1000", new IntMinMaxEvaluator(100, 999), counterSignalAndStep1000);
            Rule greaterThanOrEqualTo1000LessThanMax = new Rule("GreaterThanOrEqualTo1000LessThanMax", new IntMinMaxEvaluator(1000, maxVal-1), counterSignalAndStepMax);
            Rule equalMax = new Rule("EqualMax", new EqualEvaluator(maxVal), equalSignalMax);

            ruleSet.AddRule(lessThan10);
            ruleSet.AddRule(greaterThanOrEqualTo10LessThan100);
            ruleSet.AddRule(greaterThanOrEqualTo100LessThan1000);
            ruleSet.AddRule(greaterThanOrEqualTo1000LessThanMax);
            ruleSet.AddRule(equalMax);

            System.Console.WriteLine(ruleSet.report());

            ruleSet.Execute(dataSet);

            int maxMS = 400000;
            int step = 10;
            while(maxMS > 0)
            {
                if (equalSignal.fired)
                {
                    System.Console.WriteLine("Target reached");
                    break;
                }
                System.Threading.Thread.Sleep(step);
                maxMS -= step;
            }

            if(maxMS==0) System.Console.WriteLine("Timeout");

            ruleSet.Stop();

            System.Console.WriteLine("Execution Time (ms): " + ruleSet.GetLastCycleExecutionTime());

            Assert.That(equalSignal.fired, Is.EqualTo(true));
        }

        private class GenericIntDataPointSource : DataPointSource
        {
            int val;

            public GenericIntDataPointSource(int initVal)
            {
                this.val = initVal;
            }

            public override object GetValue()
            {
                return this.val;
            }

            public void SetVal(int newValue)
            {
                this.val = newValue;
                this.SetStale();
            }
        }

        private class IntMinMaxEvaluator : RuleEvaluator
        {
            int min;
            int max;

            public IntMinMaxEvaluator(int min, int max)
            {
                this.min = min;
                this.max = max;
                this.addDependantDataPointName("counter");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int counter = (int)dataSet.getDataPoint("counter").GetValue();
                if (counter >= this.min && counter <= this.max) return true;
                return false;
            }
        }

        private class EqualEvaluator : RuleEvaluator
        {
            int val;

            public EqualEvaluator(int val)
            {
                this.val = val;
                this.addDependantDataPointName("counter");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int counter = (int)dataSet.getDataPoint("counter").GetValue();
                if (counter == this.val) return true;
                return false;
            }
        }

        private class CounterSignalAndStep : ActionExecutor
        {
            int boundary;
            volatile bool crossed = false;

            public CounterSignalAndStep(int boundary)
            {
                this.boundary = boundary;
            }

            public void Execute(DataSet dataSet)
            {
                GenericIntDataPointSource counter = (GenericIntDataPointSource)dataSet.getDataPoint("counter").GetSource();

                if (!this.crossed)
                {
                    counter.SetVal((int)counter.GetValue() + 1);
                    if ((int)counter.GetValue() >= this.boundary)
                    {
                        this.crossed = true;
                        System.Console.WriteLine("Counter is >= " + boundary.ToString());
                    }
                    else
                    {
                        // System.Console.WriteLine("Counter value is now: " + counter.GetValue());

                    }
                }
            }
        }

        private class EqualSignal : ActionExecutor
        {
            public volatile bool fired = false;

            public void Execute(DataSet dataSet)
            {

                if (!fired)
                {
                    System.Console.WriteLine("Counter value equals Maximum");
                    fired = true;
                }
            }
        }

        [Test]
        public void VariableRuleCountTest()
        {

            /*  Rule: IntBetween(x,y)
             *
             *               {1,10}
             *              /      \
             *         {1,5}        {6,10}
             *        /    \        /    \
             *    {1,3}   {4,5}  {6,8}  {9,10}
             *    / | \   / \    / | \   / \
             *  {1}{2}{3}{4}{5}{6}{7}{8}{9}{10}
             */

            Log.logLevel = Log.LogLevels.NONE;

            RuleMSX rmsx = new RuleMSX();

            RuleSet ruleSet = rmsx.createRuleSet("VariableRuleSet");

            // Set min and max of target range
            int numMin = 1;
            int numMax = 10000;

            // Set value for target
            int target = 9999;

            TargetReached targetReached = new TargetReached();
            RuleAction reached = rmsx.createAction("reached",targetReached);

            Accumulator count = new Accumulator(0);

            // Setup rule tree
            createNRules(rmsx, ruleSet, reached, count, numMin, numMax);

            DataSet dataSet = rmsx.createDataSet("TargetTestDataSet");

            // Set datapoint for target
            dataSet.addDataPoint("target", new GenericIntDataPointSource(target));

            // Set datapoint for result
            dataSet.addDataPoint("result", new GenericIntDataPointSource(0));

            //System.Console.WriteLine("Report: \n\n" + ruleSet.report());

            ruleSet.Execute(dataSet);

            int maxMS = 40000;
            int step = 10;
            while (maxMS > 0)
            {
                if (targetReached.fired)
                {
                    break;
                }
                System.Threading.Thread.Sleep(step);
                maxMS -= step;
            }

            if (maxMS == 0) System.Console.WriteLine("Timeout");

            ruleSet.Stop();

            System.Console.WriteLine("Execution Time (ms): " + ruleSet.GetLastCycleExecutionTime());
            System.Console.WriteLine("Number of Evaluations: " + count.value().ToString());

            int result = (int)((GenericIntDataPointSource)dataSet.getDataPoint("result").GetSource()).GetValue();

            Assert.True(target==result);
        }

        private void createNRules(RuleMSX rmsx, RuleContainer rc, RuleAction reached, Accumulator acm, int min, int max)
        {

            if (min == max)
            {
                // leaf node, so add rule with action
                Rule newRule = new Rule("intBetween" + min.ToString() + "and" + max.ToString(), new IsIntBetween(min, max, acm), rmsx.createAction("ShowValue", new ShowValue(min)));
                newRule.AddAction(reached);
                rc.AddRule(newRule);
            }
            else
            {
                Rule newRule = new Rule("intBetween" + min.ToString() + "and" + max.ToString(), new IsIntBetween(min, max,acm));
                rc.AddRule(newRule);

                if (max - min < 2)
                {
                    createNRules(rmsx, newRule, reached, acm, min, min);
                    createNRules(rmsx, newRule, reached, acm, max, max);
                }
                else
                {
                    int mid = ((max - min) / 2) + min;
                    createNRules(rmsx, newRule, reached, acm, min, mid);
                    createNRules(rmsx, newRule, reached, acm, mid + 1, max);
                }
            }
        }

        private class IsIntBetween : RuleEvaluator
        {
            int min;
            int max;
            Accumulator acm;

            public IsIntBetween(int min, int max, Accumulator acm)
            {
                this.min = min;
                this.max = max;
                this.addDependantDataPointName("target");
                this.acm = acm;
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int target = (int)dataSet.getDataPoint("target").GetValue();
                acm.inc(1);
                if (target >= min && target <= max) return true;
                else return false;
            }
        }

        private class ShowValue : ActionExecutor
        {
            int val;

            public ShowValue(int val)
            {
                this.val = val;
            }

            public void Execute(DataSet dataSet)
            {
                GenericIntDataPointSource res =  (GenericIntDataPointSource)dataSet.getDataPoint("result").GetSource();
                res.SetVal(val);
                System.Console.WriteLine("Value is : " + this.val.ToString());
            }
        }

        private class TargetReached : ActionExecutor
        {
            public volatile bool fired = false;

            public void Execute(DataSet dataSet)
            {
                fired = true;
            }
        }

        private class Accumulator
        {
            int val;

            public Accumulator(int initVal)
            {
                this.val = initVal;
            }

            public void inc(int i)
            {
                this.val += i;
            }

            public int value()
            {
                return this.val;
            }
        }

        [Test]
        public void VariableDataSetCountTest()
        {

            Log.logLevel = Log.LogLevels.NONE;

            RuleMSX rmsx = new RuleMSX();

            RuleSet ruleSet = rmsx.createRuleSet("VariableRuleSet");

            // Set min and max of target range
            int numMin = 1;
            int numMax = 1000;

            // Set value for target
            int target = 998;

            // number of datasets to run
            int numDataSets = 500;

            MultiTargetReached targetReached = new MultiTargetReached(numDataSets);
            RuleAction reached = rmsx.createAction("reached", targetReached);

            Accumulator count = new Accumulator(0);

            // Setup rule tree
            createNRules(rmsx, ruleSet, reached, count, numMin, numMax);


            for (int i = 0; i < numDataSets; i++)
            {

                DataSet dataSet = rmsx.createDataSet("TargetTestDataSet" + i.ToString());

                // Set datapoint for target
                dataSet.addDataPoint("target", new GenericIntDataPointSource(target));

                // Set datapoint for result
                dataSet.addDataPoint("result", new GenericIntDataPointSource(0));

                ruleSet.Execute(dataSet);
            }

            int maxMS = 40000;
            int step = 10;
            while (maxMS > 0)
            {
                if (targetReached.fired)
                {
                    break;
                }
                System.Threading.Thread.Sleep(step);
                maxMS -= step;
            }

            if (maxMS == 0) System.Console.WriteLine("Timeout");
            ruleSet.Stop();

            System.Console.WriteLine("Execution Time (ms): " + ruleSet.GetLastCycleExecutionTime());
            System.Console.WriteLine("Number of Evaluations: " + count.value().ToString());

            //int result = (int)((GenericIntDataPointSource)dataSet.getDataPoint("result").GetSource()).GetValue();

            Assert.True(true);
        }

        private class MultiTargetReached : ActionExecutor
        {
            private int multiTarget;
            private int hitCount=0;
            public volatile bool fired = false;

            public MultiTargetReached(int multiTarget)
            {
                this.multiTarget = multiTarget;
            }

            public void Execute(DataSet dataSet)
            {
                hitCount++;
                if(hitCount==multiTarget) fired = true;
            }
        }

    }
}
