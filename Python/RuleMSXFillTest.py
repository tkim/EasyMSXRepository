# RMSXRouteFillTest.py

import logging
from datetime import datetime

from easymsx.easymsx import EasyMSX
from easymsx.notification import Notification as EasyMSXNotification
from rulemsx.rulemsx import RuleMSX
from rulemsx.ruleevaluator import RuleEvaluator
from rulemsx.action import Action
from rulemsx.datapointsource import DataPointSource
from rulemsx.rulecondition import RuleCondition

def log(msg):

    mytime= datetime.now()
    s  = mytime.strftime("%Y%m%d%H%M%S%f")[:-3]
    print(s + "(RMSXRouteFillTest): \t" + msg)


class RMSXRouteFillTest:
    
    def __init__(self):

        self.easymsx = None
        
        log("Initialising RuleMSX...")
        self.rulemsx = RuleMSX()
        log("RuleMSX initialised.")
        
        log("Initialising EasyMSX...")
        self.easymsx = EasyMSX()
        log("EasyMSX initialised.")
        
        log("Create ruleset...")
        self.build_rules()
        log("RuleSet ready.")

        self.easymsx.routes.add_notification_handler(self.process_notification)

        log("Starting EasyMSX...")
        self.easymsx.start()
        log("EasyMSX started.")

    
    def build_rules(self):
        
        log("Building Rules...")

        log("Creating RuleCondition condRouteFillOccurred")
        cond_route_fill_occured = RuleCondition("RouteFillOccured", self.RouteFillOccurred())
        log("RuleCondition condRouteFillOccurred created.")

        log("Creating Action actShowRouteFill")
        action_show_route_fill = self.rulemsx.create_action("ShowRouteFill", self.ShowRouteFill())
        log("Action actShowRouteFill created")

        log("Creating RuleSet demoRouteRuleSet")
        demo_route_ruleset = self.rulemsx.create_ruleset("demoRouteRuleSet")
        log("RuleSet demoRouteRuleSet created")

        log("Creating Rule ruleRouteFilled")
        rule_route_filled = demo_route_ruleset.add_rule("RouteFilled")
        log("Rule ruleRouteFilled created")

        log("Assign RuleCondition condRouteFillOccurred to Rule ruleRouteFilled")
        rule_route_filled.add_rule_condition(cond_route_fill_occured)

        log("Assign Action actShowRouteFill to Rule ruleRouteFilled")
        rule_route_filled.add_action(action_show_route_fill)

        log("Rules built.")


    def process_notification(self,notification):

        log("EasyMSX notification received...")
        if notification.category == EasyMSXNotification.NotificationCategory.ROUTE:
            log("Notification is at ROUTE level")
            log("\tNotification: ")
            for fc in notification.field_changes:
                log("\t\tField: " + fc.field.name() + "\told: " + fc.old_value + "\tnew: " + fc.new_value)
            if notification.type == EasyMSXNotification.NotificationType.NEW or notification.type == EasyMSXNotification.NotificationType.INITIALPAINT: 
                log("Notification is NEW or INITIAL_PAINT")
                if notification.source.field("EMSX_STATUS").value() == "FILLED" or notification.source.field("EMSX_STATUS").value() == "CANCEL":
                    log("Route " + notification.source.field("EMSX_SEQUENCE").value() + "." + notification.source.field("EMSX_ROUTE_ID").value() + " is " + notification.source.field("EMSX_STATUS").value() + " - Ignoring")
                else: 
                    log("EasyMSX Notification Route -> NEW/INIT_PAINT: " + notification.source.field("EMSX_SEQUENCE").value() + "." + notification.source.field("EMSX_ROUTE_ID").value())
                    self.parse_route(notification.source)
            


    def parse_route(self,r):
        
        log("Parse Route: " + r.field("EMSX_SEQUENCE").value() + "." + r.field("EMSX_ROUTE_ID").value())

        log("Creating newDataSet")
        new_dataset = self.rulemsx.create_dataset("DS_RT_" + r.field("EMSX_SEQUENCE").value() + "." + r.field("EMSX_ROUTE_ID").value())

        log("Creating RouteStatus DataPoint")
        new_dataset.add_datapoint("RouteStatus", self.EMSXFieldDataPointSource(r.field("EMSX_STATUS")))
 
        log("Creating RouteOrderNumber DataPoint")
        new_dataset.add_datapoint("RouteOrderNumber", self.EMSXFieldDataPointSource(r.field("EMSX_SEQUENCE")))

        log("Creating RouteID DataPoint")
        new_dataset.add_datapoint("RouteID", self.EMSXFieldDataPointSource(r.field("EMSX_ROUTE_ID")))

        log("Creating RouteFilled DataPoint")
        new_dataset.add_datapoint("RouteFilled", self.EMSXFieldDataPointSource(r.field("EMSX_FILLED")))

        log("Creating RouteAmount DataPoint")
        new_dataset.add_datapoint("RouteAmount", self.EMSXFieldDataPointSource(r.field("EMSX_AMOUNT")))

        log("Creating RouteLastShares DataPoint")
        new_dataset.add_datapoint("RouteLastShares", self.EMSXFieldDataPointSource(r.field("EMSX_LAST_SHARES")))

        log("Creating LastFillShown DataPoint")
        new_dataset.add_datapoint("LastFillShown", self.GenericIntegerDataPointSource(0))

        log("Executing RuleSet DemoRouteRuleSet with DataSet " + new_dataset.name)
        self.rulemsx.rulesets["demoRouteRuleSet"].execute(new_dataset)



    class GenericIntegerDataPointSource(DataPointSource):

        def __init__(self, initial_value):
            
            log("Creating new GenericIntegerDataPointSource with initial value: " + str(initial_value));
            
            self.value = initial_value
        
        def get_value(self):
            log("Returning value for GenericIntegerDataPointSource - Value: " + str(self.value));
            return self.value;

        def set_value(self, new_value):
            log("Setting GenericIntegerDataPointSource value to: " + str(new_value) + "\t (old value: " + str(self.value) +")")
            self.value = new_value
            self.set_stale()



    class EMSXFieldDataPointSource(DataPointSource):

        def __init__(self, field):
            self.field = field
            self.value = field.value()
            self.previous_value = None
            
            log("Creating new EMSXFieldDataPointSource for Field: " + field.name() + "\tValue: ")

            log("Adding EasyMSX field level notification handler for Field: " + field.name())
            self.field.add_notification_handler(self.process_notification)
            log("New EMSXFieldDataPointSource created")

        def get_value(self):
            return self.value
        
        def get_previous_value(self):
            log("Returning previous value for field " + self.field.name() + "\tPrevious Value: " + (self.previous_value if not self.previous_value is None else "/none/"));
            return self.previous_value
        
        def process_notification(self, notification):
            
            log("Handling EasyMSX field notifiction for field " + self.field.name())

            self.previous_value = self.value
            self.value = notification.field_changes[0].new_value

            log("-- Value: " + self.value + "\tPrevious: " + self.previous_value)
            
            if self.previous_value != self.value:
                log("-- Values differ - calling SetStale")
                self.set_stale()
                log("-- Returned from SetStale")

     

    class RouteFillOccurred(RuleEvaluator):
        
        def __init__(self):
            
            log("Creating new RouteFillOccurred")
            
            log("Adding dependency for RouteStatus")
            self.add_dependent_datapoint_name("RouteStatus")
            log("Adding dependency for RouteFilled")
            self.add_dependent_datapoint_name("RouteFilled")
            log("Adding dependency for RouteLastShares")
            self.add_dependent_datapoint_name("RouteLastShares")
            log("Done Adding dependencies.")

                
        def evaluate(self,dataset):
            
            log("Evaluating RouteFillOccurred...")

            route_filled_source = dataset.datapoints["RouteFilled"].datapoint_source
            route_last_shares_source = dataset.datapoints["RouteLastShares"].datapoint_source
            route_status_source = dataset.datapoints["RouteStatus"].datapoint_source
            
            current_filled =  int(route_filled_source.get_value())
            previous_filled =  int(route_filled_source.get_previous_value()) if not route_filled_source.get_previous_value() is None else 0

            current_last_shares = int(route_last_shares_source.get_value())
            previous_last_shares = int(route_last_shares_source.get_previous_value()) if not route_last_shares_source.get_previous_value() is None else 0

            current_status = route_status_source.get_value()
            previous_status = route_status_source.get_previous_value() if not route_status_source.get_previous_value () is None else ""
            
            last_fill_shown = dataset.datapoints["LastFillShown"].get_value()
            
            log("RouteFillOccurred DataSet values : current_filled=" + str(current_filled) + "|previous_filled=" + str(previous_filled) + "|current_last_shares=" + str(current_last_shares) + "|previous_last_shares=" + str(previous_last_shares) + "|current_status=" + current_status + "|previous_status=" + previous_status + "|last_fill_shown=" + str(last_fill_shown))
                
            res = (current_filled != previous_filled and previous_status != "" and current_filled != last_fill_shown)
            #res = (current_filled != previous_filled and current_filled != last_fill_shown)
            
            log("RouteFillOccurred returning value: " + str(res))
            
            return res
            
    class ShowRouteFill(Action):
        
        def __init__(self):
            log("Creating new ShowRouteFill action executor.")
        
        def execute(self,dataset):
            
            log(">>> ShowRouteFill Action Executor: ")
            log(">>> RouteStatus:" + dataset.datapoints["RouteStatus"].get_value())
            log(">>> RouteOrderNumber:" + dataset.datapoints["RouteOrderNumber"].get_value())
            log(">>> RouteID:" + dataset.datapoints["RouteID"].get_value())
            log(">>> RouteFilled:" + dataset.datapoints["RouteFilled"].get_value())
            log(">>> RouteAmount:" + dataset.datapoints["RouteAmount"].get_value())
            log(">>> RouteLastShares:" + dataset.datapoints["RouteLastShares"].get_value())

            log("Getting source for LastFillShown")
            last_fill = dataset.datapoints["LastFillShown"].datapoint_source
            log("Setting value for LastFillShown")
            last_fill.set_value(dataset.datapoints["RouteFilled"].get_value())
            log("Done setting value for LastFillShown")



if __name__ == '__main__':
    
    RMSXRouteFillTest = RMSXRouteFillTest();
    
    input("Press any to terminate\n")

    print("Terminating...\n")

    RMSXRouteFillTest.rulemsx.stop()
    
    quit()
    