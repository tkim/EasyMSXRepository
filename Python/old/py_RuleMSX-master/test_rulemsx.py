'''
Created on 25 Nov 2017

@author: Rik Clegg
'''
import unittest
import rulemsx
from datapointsource import DataPointSource
from ruleevaluator import RuleEvaluator
from rulecondition import RuleCondition
import time

class TestRuleMSX(unittest.TestCase):

    def test_InstantiateRuleMSXEmptyConstGivesEmptyRuleandDataSets(self):

        rmsx = rulemsx.RuleMSX()
        self.assertEqual(len(rmsx.dataSets),0)
        self.assertEqual(len(rmsx.ruleSets),0)
        
        
    def test_CreateDataSetReturnsNewDataSet(self):
        
        rmsx = rulemsx.RuleMSX()
        newDataSetName = "NewDataSet"
        ds = rmsx.createDataSet(newDataSetName)
        self.assertEqual(ds.name, newDataSetName)
        
        
    def test_CreateRuleSetReturnsNewRuleSet(self):
        
        rmsx = rulemsx.RuleMSX()
        newRuleSetName = "NewRuleSet"
        rs = rmsx.createRuleSet(newRuleSetName)
        self.assertEqual(rs.name, newRuleSetName)
        
    
    def test_CreateDataSetWithEmptyNameFails(self):
        
        rmsx = rulemsx.RuleMSX()
        newDataSetName = ""
        self.assertRaises(ValueError, rmsx.createDataSet,newDataSetName)
        
    
    def test_CreateRuleSetWithEmptyNameFails(self):
        
        rmsx = rulemsx.RuleMSX()
        newRuleSetName = ""
        self.assertRaises(ValueError, rmsx.createRuleSet,newRuleSetName)

    
    def test_CreateDataSetWithNameAsNone(self):
        
        rmsx = rulemsx.RuleMSX()
        newDataSetName = None
        self.assertRaises(ValueError, rmsx.createDataSet,newDataSetName)
        
    
    def test_CreateRuleSetWithNameAsNone(self):
        
        rmsx = rulemsx.RuleMSX()
        newRuleSetName = None
        self.assertRaises(ValueError, rmsx.createRuleSet,newRuleSetName)
        
    
    def test_RuleMSXStopShouldReturnTrueWithNoRuleSets(self):
        
        rmsx = rulemsx.RuleMSX()
        self.assertTrue(rmsx.stop())

    
    def test_RuleMSXStopShouldReturnTrueWithActiveRuleSet(self):
        
        rmsx = rulemsx.RuleMSX()
        ruleSetName = "NewRuleSet"
        rs = rmsx.createRuleSet(ruleSetName)
        self.assertTrue(rs.stop())
        
        
    def test_RuleSetGetNameShouldReturnName(self):
        
        rmsx = rulemsx.RuleMSX()
        ruleSetName = "NewRuleSet"
        rs = rmsx.createRuleSet(ruleSetName)
        self.assertEqual(rs.name, ruleSetName)

    
    def test_CreateDataPointNoSourceCheckName(self):
        
        rmsx = rulemsx.RuleMSX()
        dataSetName = "NewDataSet"
        dataPointName = "NewDataPoint"
        ds = rmsx.createDataSet(dataSetName)
        ds.addDataPoint(dataPointName)
        self.assertEqual(ds.dataPoints[dataPointName].name, dataPointName)


    def test_CreateDataPointNoNameFail(self):
        
        rmsx = rulemsx.RuleMSX()
        dataSetName = "NewDataSet"
        dataPointName = None
        ds = rmsx.createDataSet(dataSetName)
        self.assertRaises(ValueError, ds.addDataPoint, dataPointName)
        

    def test_CreateDataPointEmptyNameFail(self):
        
        rmsx = rulemsx.RuleMSX()
        dataSetName = "NewDataSet"
        dataPointName = ""
        ds = rmsx.createDataSet(dataSetName)
        self.assertRaises(ValueError, ds.addDataPoint, dataPointName)
        
    
    def test_AddDataPointSourceInvalidType(self):

        rmsx = rulemsx.RuleMSX()
        dataSetName = "NewDataSet"
        dataPointName = "NewDataPoint"
        ds = rmsx.createDataSet(dataSetName)
        self.assertRaises(TypeError, ds.addDataPoint, dataPointName, 1) # pass int instead of datapointsource
        
    class GenericStringDataPointSource(DataPointSource):
            
        def __init__(self, val=None):
            self.strval = val

    def test_AddDataPointSourceValid(self):

        rmsx = rulemsx.RuleMSX()
        dataSetName = "NewDataSet"
        dataPointName = "NewDataPoint"
        ds = rmsx.createDataSet(dataSetName)
        dps = self.GenericStringDataPointSource()
        ds.addDataPoint(dataPointName, dps)
        dso = rmsx.dataSets[dataSetName]
        dpo = dso.dataPoints[dataPointName]
        dpsOut = dpo.dataPointSource
        self.assertEqual(dps, dpsOut)

    class GenericRuleConditionEvaluator(RuleEvaluator):
        
        def __init__(self, target, dataPointName):
            self.target = target
            self.dataPointName = dataPointName
            
        def evaluate(self,dataSet):
            val = dataSet.dataPoints[self.dataPointName].getValue()
            return val==self.target
        
    def test_integration_TestRuleSet01(self):
        
        raised = False
        
        try:
            rmsx = rulemsx.RuleMSX()
            
            ds1 = rmsx.createDataSet("DataSet1")
            ds1.addDataPoint("DataPoint1",self.GenericStringDataPointSource("TestValue"))
            ds1.addDataPoint("DataPoint2",self.GenericStringDataPointSource("AnotherValue"))
            
            rs1 = rmsx.createRuleSet("RuleSet1")
            r1 = rs1.addRule("TestRule1")
            
            c1 = RuleCondition("CheckIfTarget1MatchesDataPoint", self.GenericRuleConditionEvaluator("TestValue","DataPoint1"))
            r1.addRuleCondition(c1)
            
            c2 = RuleCondition("CheckIfTarget2MatchesDataPoint", self.GenericRuleConditionEvaluator("AnotherValue","DataPoint2"))
            r1.addRuleCondition(c2)
 
            rs1.execute(ds1)
 
            time.sleep(5)
            
            rs1.stop()
            
        except:
            raised = True

        self.assertFalse(raised)
