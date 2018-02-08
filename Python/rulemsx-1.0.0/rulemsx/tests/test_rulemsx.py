'''
Created on 25 Nov 2017

@author: RCLEGG2@BLOOMBERG.NET
'''
import unittest
import time
from rulemsx import rulemsx
from rulemsx.datapointsource import DataPointSource
from rulemsx.ruleevaluator import RuleEvaluator
from rulemsx.rulecondition import RuleCondition
from rulemsx.action import Action

class TestRuleMSX(unittest.TestCase):

    def test_instantiate_ruleMSX_empty_const_gives_empty_ruleset_and_dataset(self):

        rmsx = rulemsx.RuleMSX()
        self.assertEqual(len(rmsx.datasets),0)
        self.assertEqual(len(rmsx.rulesets),0)
        
        
    def test_create_dataset_returns_new_dataset(self):
        
        rmsx = rulemsx.RuleMSX()
        new_dataset_name = "NewDataSet"
        ds = rmsx.create_dataset(new_dataset_name)
        self.assertEqual(ds.name, new_dataset_name)
        
        
    def test_create_ruleset_returns_new_ruleset(self):
        
        rmsx = rulemsx.RuleMSX()
        new_ruleset_name = "NewRuleSet"
        rs = rmsx.create_ruleset(new_ruleset_name)
        self.assertEqual(rs.name, new_ruleset_name)
        
    
    def test_create_dataset_with_empty_name_fails(self):
        
        rmsx = rulemsx.RuleMSX()
        new_dataset_name = ""
        self.assertRaises(ValueError, rmsx.create_dataset,new_dataset_name)
        
    
    def test_create_ruleset_with_empty_name_fails(self):
        
        rmsx = rulemsx.RuleMSX()
        new_ruleset_name = ""
        self.assertRaises(ValueError, rmsx.create_ruleset,new_ruleset_name)

    
    def test_create_dataset_with_name_as_none(self):
        
        rmsx = rulemsx.RuleMSX()
        new_dataset_name = None
        self.assertRaises(ValueError, rmsx.create_dataset,new_dataset_name)
        
    
    def test_create_ruleset_with_name_as_none(self):
        
        rmsx = rulemsx.RuleMSX()
        new_ruleset_name = None
        self.assertRaises(ValueError, rmsx.create_ruleset,new_ruleset_name)
        
    
    def test_stop_should_return_true_with_no_rulesets(self):
        
        rmsx = rulemsx.RuleMSX()
        self.assertTrue(rmsx.stop())

    
    def test_stop_should_return_true_with_active_ruleset(self):
        
        rmsx = rulemsx.RuleMSX()
        ruleset_name = "NewRuleSet"
        rs = rmsx.create_ruleset(ruleset_name)
        self.assertTrue(rs.stop())
        
        
    def test_ruleset_get_name_should_return_name(self):
        
        rmsx = rulemsx.RuleMSX()
        ruleset_name = "NewRuleSet"
        rs = rmsx.create_ruleset(ruleset_name)
        self.assertEqual(rs.name, ruleset_name)

    
    def test_create_datapoint_no_source_check_name(self):
        
        rmsx = rulemsx.RuleMSX()
        dataset_name = "NewDataSet"
        datapoint_name = "NewDataPoint"
        ds = rmsx.create_dataset(dataset_name)
        ds.add_datapoint(datapoint_name)
        self.assertEqual(ds.datapoints[datapoint_name].name, datapoint_name)


    def test_create_datapoint_no_name_fail(self):
        
        rmsx = rulemsx.RuleMSX()
        dataset_name = "NewDataSet"
        datapoint_name = None
        ds = rmsx.create_dataset(dataset_name)
        self.assertRaises(ValueError, ds.add_datapoint, datapoint_name)
        

    def test_create_datapoint_empty_name_fail(self):
        
        rmsx = rulemsx.RuleMSX()
        dataset_name = "NewDataSet"
        datapoint_name = ""
        ds = rmsx.create_dataset(dataset_name)
        self.assertRaises(ValueError, ds.add_datapoint, datapoint_name)
        
    
    def test_add_datapoint_source_invalid_type(self):

        rmsx = rulemsx.RuleMSX()
        dataset_name = "NewDataSet"
        datapoint_name = "NewDataPoint"
        ds = rmsx.create_dataset(dataset_name)
        self.assertRaises(TypeError, ds.add_datapoint, datapoint_name, 1) # pass int instead of datapointsource
        
    class GenericStringDataPointSource(DataPointSource):
            
        def __init__(self, val=None):
            self.strval = val

        def get_value(self):
            print("DataPoint>> returning: %s" % (self.strval))
            return self.strval
        
        def set_value(self, val):
            self.strval = val
            super().set_stale()

    def test_add_datapoint_source_valid(self):

        rmsx = rulemsx.RuleMSX()
        dataset_name = "NewDataSet"
        datapoint_name = "NewDataPoint"
        ds = rmsx.create_dataset(dataset_name)
        dps = self.GenericStringDataPointSource()
        ds.add_datapoint(datapoint_name, dps)
        dso = rmsx.datasets[dataset_name]
        dpo = dso.datapoints[datapoint_name]
        dpsOut = dpo.datapoint_source
        self.assertEqual(dps, dpsOut)

    class GenericRuleConditionEvaluator(RuleEvaluator):
        
        def __init__(self, target, datapoint_name):
            self.target = target
            self.datapoint_name = datapoint_name
            super().add_dependent_datapoint_name(datapoint_name)
            
        def evaluate(self,dataset):
            val = dataset.datapoints[self.datapoint_name].get_value()
            res = val==self.target
            print("Condition >> Evaluating: %s = %s \treturning: %s" % (val, self.target, res))
            return res
        
    class PrintStringAction(Action):
        
        def __init__(self,some_string, mod_datapoint_name):
            self.strval = some_string
            self.mod_datapoint_name = mod_datapoint_name
            
        def execute(self,dataset):
            dataset.datapoints[self.mod_datapoint_name].datapoint_source.set_value("XtestvalueX")
            print("Action Execute: %s" % (self.strval))
            
    def test_integration_TestRuleSet01(self):
        
        raised = False
        
        try:
            rmsx = rulemsx.RuleMSX()
            
            ds1 = rmsx.create_dataset("DataSet1")
            ds1.add_datapoint("DataPoint1",self.GenericStringDataPointSource("TestValue"))
            ds1.add_datapoint("DataPoint2",self.GenericStringDataPointSource("AnotherValue"))
            
            rs1 = rmsx.create_ruleset("RuleSet1")

            r1 = rs1.add_rule("TestRule1")
            
            c1 = RuleCondition("CheckIfTarget1MatchesDataPoint", self.GenericRuleConditionEvaluator("TestValue","DataPoint1"))
            r1.add_rule_condition(c1)
            
            c2 = RuleCondition("CheckIfTarget2MatchesDataPoint", self.GenericRuleConditionEvaluator("AnotherValue","DataPoint2"))
            r1.add_rule_condition(c2)
 
            e1 = self.PrintStringAction("Result of TestRule1", "DataPoint1")
            a1 = rmsx.create_action("TestAction1", e1)

            r1.add_action(a1)
            
            rs1.execute(ds1)
 
            #time.sleep(0)
            
            rs1.stop()
            
        except BaseException  as e:
            print("error: " +str(e))
            raised = True

        self.assertFalse(raised)
