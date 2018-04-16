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
            Assert.That(rmsx.GetRuleSets().Count, Is.EqualTo(0));
            Assert.That(rmsx.GetDataSets().Count, Is.EqualTo(0));
        }

        [Test]
        public void GetDataSetsReturnsEmptyDataSetList()
        {
            // GetDataSets() should return an empty DataSet list on newly created RuleMSX object
            RuleMSX rmsx = new RuleMSX();
            List<DataSet> dataSets = rmsx.GetDataSets();
            Assert.That(dataSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetRuleSetsReturnsEmptyRuleSetList()
        {
            // GetRuleSets() should return an empty RuleSet list on newly created RuleMSX object
            RuleMSX rmsx = new RuleMSX();
            List<RuleSet> ruleSets = rmsx.GetRuleSets();
            Assert.That(ruleSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateDataSetReturnsNewDataSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            DataSet dataSet = rmsx.CreateDataSet(newDataSetName);
            Assert.That(dataSet.GetName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void CreateRuleSetReturnsNewRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            RuleSet ruleSet = rmsx.CreateRuleSet(newRuleSetName);
            Assert.That(ruleSet.GetName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void CreateDataSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.CreateDataSet(newDataSetName));
        }

        [Test]
        public void CreateDataSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = null;
            Assert.Throws<ArgumentException>(() => rmsx.CreateDataSet(newDataSetName));
        }

        [Test]
        public void CreateRuleSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.CreateRuleSet(newRuleSetName));
        }

        [Test]
        public void CreateRuleSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.CreateRuleSet(newRuleSetName));
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
            RuleSet rs = rmsx.CreateRuleSet(newRuleSetName);
            rs.AddRule(newRuleName);
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
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
            RuleSet rs = rmsx.CreateRuleSet(newRuleSetName);
            rs.AddRule(newRuleName);
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            // We are deliberately not executing the ruleset...
            //rs.Execute(ds);
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        [Test]
        public void RuleSetGetNameShouldReturnName()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            RuleSet rs = rmsx.CreateRuleSet(newRuleSetName);
            Assert.That(rs.GetName(), Is.EqualTo(newRuleSetName));                   
        }

        [Test]
        public void DataSetGetNameShouldReturnName()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            Assert.That(ds.GetName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void RuleSetAddRuleGetRuleShouldReturnNewRule()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            string newRuleName = "Rule1";
            RuleSet rs = rmsx.CreateRuleSet(newRuleSetName);
            rs.AddRule(newRuleName);
            Assert.That(rs.GetRule(newRuleName).GetName(), Is.EqualTo(newRuleName));
        }

        [Test]
        public void DataSetAddDataPointReturnsNewDataPoint()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "DataPoint1";
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPoint dp1 = ds.AddDataPoint(newDataPointName);
            Assert.That(dp1.GetName(), Is.EqualTo(newDataPointName));
        }

        [Test]
        public void GetDataSetByNameReturnsCorrectDataSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.CreateDataSet(newDataSetName);
            DataSet ds = rmsx.GetDataSet(newDataSetName);
            Assert.That(ds.GetName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void GetRuleSetByNameReturnsCorrectRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.CreateRuleSet(newRuleSetName);
            RuleSet rs = rmsx.GetRuleSet(newRuleSetName);
            Assert.That(rs.GetName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void GetDataSetWithIncorrectNameReturnsNull()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.CreateDataSet(newDataSetName);
            DataSet ds = rmsx.GetDataSet("SomeOtherName");
            Assert.IsNull(ds);
        }

        [Test]
        public void GetRuleSetWithIncorrectNameReturnsNull()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.CreateRuleSet(newRuleSetName);
            RuleSet rs = rmsx.GetRuleSet("SomeOtherName");
            Assert.IsNull(rs);
        }

        [Test]
        public void GetDataSetsReturnsDataSetCollection()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            rmsx.CreateDataSet(newDataSetName);
            List<DataSet> dsl = rmsx.GetDataSets();
            DataSet ds = dsl[0];
            Assert.That(ds.GetName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void GetRuleSetsReturnsRuleSetCollection()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            rmsx.CreateRuleSet(newRuleSetName);
            List<RuleSet> rsl = rmsx.GetRuleSets();
            RuleSet rs = rsl[0];
            Assert.That(rs.GetName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void GetDataPointsReturnsCorrectDictOfPoints()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPoint dpi = ds.AddDataPoint(newDataPointName);
            Dictionary<string, DataPoint> dpd = ds.GetDataPoints();
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
            RuleSet rs = rmsx.CreateRuleSet(newRuleSetName);
            Rule ri = rs.AddRule(newRuleName);
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
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPoint dpo = ds.AddDataPoint(newDataPointName);
            Assert.That(dpo.GetName(), Is.EqualTo(newDataPointName));
        }

        [Test]
        public void SetDataPointSourceGetSourceReturnCorrect()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            string newDataPointName = "NewDataPointName";
            string testDataPointValue = "TestDataPointValue";
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPoint dpo = ds.AddDataPoint(newDataPointName);
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
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPointSource srci = new TestDataPointSource(testDataPointValue);
            DataPoint dpo = ds.AddDataPoint(newDataPointName, srci);
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
            DataSet ds = rmsx.CreateDataSet(newDataSetName);
            DataPoint dpo = ds.AddDataPoint(newDataPointName);
            DataPointSource srci = new TestDataPointSource(testDataPointValue);
            dpo.SetDataPointSource(srci);
            Assert.DoesNotThrow(() => srci.SetStale());
        }
    }
}

