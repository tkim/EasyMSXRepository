using com.bloomberg.samples.rulemsx;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RuleMSXNUnitTest
{

    [TestFixture]
    public class RuleMSXNUnitTest

    {
        [Test]
        public void InstantiateRuleMSXEmptyConstGivesEmptyRuleandDataSets()
        {
            // New instance of RuleMSX should have empty RuleSet and DataSet collections
            RuleMSX rmsx = new RuleMSX();
            Assert.That(rmsx.getRuleSets().Count, Is.EqualTo(0));
            Assert.That(rmsx.getDataSets().Count, Is.EqualTo(0));
        }

        [Test]
        public void GetDataSetsReturnsEmptyDataSetList()
        {
            // GetDataSets() should return an empty DataSet list on newly created RuleMSX object
            RuleMSX rmsx = new RuleMSX();
            List<DataSet> dataSets = rmsx.getDataSets();
            Assert.That(dataSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetRuleSetsReturnsEmptyRuleSetList()
        {
            // GetRuleSets() should return an empty RuleSet list on newly created RuleMSX object
            RuleMSX rmsx = new RuleMSX();
            List<RuleSet> ruleSets = rmsx.getRuleSets();
            Assert.That(ruleSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateDataSetReturnsNewDataSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            DataSet dataSet = rmsx.createDataSet(newDataSetName);
            Assert.That(dataSet.getName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void CreateRuleSetReturnsNewRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            RuleSet ruleSet = rmsx.createRuleSet(newRuleSetName);
            Assert.That(ruleSet.getName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void CreateDataSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createDataSet(newDataSetName));
        }

        [Test]
        public void CreateDataSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = null;
            Assert.Throws<ArgumentException>(() => rmsx.createDataSet(newDataSetName));
        }

        [Test]
        public void CreateRuleSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createRuleSet(newRuleSetName));
        }

        [Test]
        public void CreateRuleSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createRuleSet(newRuleSetName));
        }

        [Test]
        public void RuleMSXStopShouldReturnTrueWithNoRuleSets()
        {
            RuleMSX rmsx = new RuleMSX();
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        [Test]
        public void RuleMSXStopShouldReturnTrueWithActiveRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newDataSetName = "NewDataSet";
            string newRuleName = "Rule1";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            DataSet ds = rmsx.createDataSet(newDataSetName);
            rs.AddRule(r);
            rs.Execute(ds);
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        private class GenericBoolRule : RuleEvaluator
        {
            bool ret;

            public GenericBoolRule(bool returnValue)
            {
                this.ret = returnValue;
            }

            public override bool Evaluate(DataSet dataSet)
            {
                return this.ret;
            }
        }

        [Test]
        public void RuleMSXStopShouldReturnTrueWithUnexecutedRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newDataSetName = "NewDataSet";
            string newRuleName = "Rule1";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            DataSet ds = rmsx.createDataSet(newDataSetName);
            rs.AddRule(r);
            // We are deliberately not executing the ruleset...
            //rs.Execute(ds);
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        [Test]
        public void RuleSetGetNameShouldReturnName()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Assert.That(rs.getName(), Is.EqualTo(newRuleSetName));                   
        }

        [Test]
        public void DataSetGetNameShouldReturnName()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            Assert.That(ds.getName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void RuleSetAddRuleGetRuleShouldReturnNewRule()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newRuleName = "Rule1";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            rs.AddRule(r);
            Assert.That(rs.GetRule(newRuleName).GetName(), Is.EqualTo(newRuleName));
        }

        [Test]
        public void DataSetAddDataPointReturnsNewDataPoint()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "DataPoint1";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPoint dp1 = ds.addDataPoint(newDataPointName);
            Assert.That(dp1.GetName(), Is.EqualTo(newDataPointName));
        }

        [Test]
        public void GetDataSetByNameReturnsCorrectDataSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.createDataSet(newDataSetName);
            DataSet ds = rmsx.getDataSet(newDataSetName);
            Assert.That(ds.getName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void GetRuleSetByNameReturnsCorrectRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.createRuleSet(newRuleSetName);
            RuleSet rs = rmsx.getRuleSet(newRuleSetName);
            Assert.That(rs.getName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void GetDataSetWithIncorrectNameReturnsNull()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.createDataSet(newDataSetName);
            DataSet ds = rmsx.getDataSet("SomeOtherName");
            Assert.IsNull(ds);
        }

        [Test]
        public void GetRuleSetWithIncorrectNameReturnsNull()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.createRuleSet(newRuleSetName);
            RuleSet rs = rmsx.getRuleSet("SomeOtherName");
            Assert.IsNull(rs);
        }

        [Test]
        public void GetDataSetsReturnsDataSetCollection()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.createDataSet(newDataSetName);
            List<DataSet> dsl = rmsx.getDataSets();
            DataSet ds = dsl[0];
            Assert.That(ds.getName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void GetRuleSetsReturnsRuleSetCollection()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.createRuleSet(newRuleSetName);
            List<RuleSet> rsl = rmsx.getRuleSets();
            RuleSet rs = rsl[0];
            Assert.That(rs.getName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void GetDataPointsReturnsCorrectDictOfPoints()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPoint dpi = ds.addDataPoint(newDataPointName);
            Dictionary<string, DataPoint> dpd = ds.getDataPoints();
            DataPoint dpo;
            dpd.TryGetValue(newDataPointName, out dpo);
            Assert.That(dpo.GetName(), Is.EqualTo(newDataPointName));
        }

        [Test]
        public void GetRulesReturnsCorrectListOfRules()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newRuleName = "NewRuleName";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Rule ri = new Rule(newRuleName,new GenericBoolRule(true));
            rs.AddRule(ri);
            List<Rule> rl = rs.GetRules();
            Rule ro = rl[0];
            Assert.That(ro.GetName(), Is.EqualTo(newRuleName));
        }

        [Test]
        public void DataSetDataPointAddReturnsNewDataPoint()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPoint dpo = ds.addDataPoint(newDataPointName);
            Assert.That(dpo.GetName(), Is.EqualTo(newDataPointName));
        }

        [Test]
        public void SetDataPointSourceGetSourceReturnCorrect()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            string testDataPointValue = "TestDataPointValue";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPoint dpo = ds.addDataPoint(newDataPointName);
            DataPointSource srci = new TestDataPointSource(testDataPointValue);
            dpo.SetDataPointSource(srci);
            DataPointSource srco = dpo.GetSource();
            Assert.That(srco.GetValue().ToString(), Is.EqualTo(testDataPointValue));
        }

        private class TestDataPointSource : DataPointSource
        {
            string retValue;

            public TestDataPointSource(string retValue)
            {
                this.retValue = retValue;
            }
            public override object GetValue()
            {
                return retValue;
            }
        }

        [Test]
        public void SetDataPointSourceAtDataPointCreateCheckSource()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            string testDataPointValue = "TestDataPointValue";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPointSource srci = new TestDataPointSource(testDataPointValue);
            DataPoint dpo = ds.addDataPoint(newDataPointName, srci);
            DataPointSource srco = dpo.GetSource();
            Assert.That(srco.GetValue().ToString(), Is.EqualTo(testDataPointValue));
        }

        [Test]
        public void DataPointSourceSetStaleNoError()
        {
            // Call to SetStale() on DataPointSource should return no error
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            string testDataPointValue = "TestDataPointValue";
            DataSet ds = rmsx.createDataSet(newDataSetName);
            DataPoint dpo = ds.addDataPoint(newDataPointName);
            DataPointSource srci = new TestDataPointSource(testDataPointValue);
            dpo.SetDataPointSource(srci);
            Assert.DoesNotThrow(() => srci.SetStale());
        }

        [Test]
        public void RuleGetEvaluatorReturnsCorrectEvaluator()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newDataSetName = "NewDataSet";
            string newRuleName = "Rule1";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            DataSet ds = rmsx.createDataSet(newDataSetName);
            rs.AddRule(r);
            Assert.That(r.GetEvaluator().Evaluate(ds), Is.EqualTo(true));
        }

        [Test]
        public void EvaluatorAddDependencyNameNoError()
        {
            RuleEvaluator newRuleEval = new GenericBoolRule(true);
            Assert.DoesNotThrow(() => newRuleEval.addDependantDataPointName("SomeName"));
        }

        [Test]
        public void RuleAddActionGetActionReturnsCorrectAction()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newDataSetName = "NewDataSet";
            string newRuleName = "Rule1";
            string actionMessage = "ActionMessage";
            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            DataSet ds = rmsx.createDataSet(newDataSetName);
            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            rs.AddRule(r);
            RuleAction rai = rmsx.createAction("RuleActionIn", new GenericAction(actionMessage));
            r.AddAction(rai);
            ActionExecutor rae = r.GetActions()[0].getExecutor();
            rae.Execute(ds);
            GenericAction ga = (GenericAction)rae;
            Assert.That(ga.getOutgoing, Is.EqualTo(actionMessage));
        }

        private class GenericAction : ActionExecutor
        {
            string message;
            string outgoing;

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
    }
}

