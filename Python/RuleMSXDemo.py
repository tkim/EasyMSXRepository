# RuleMSXDemo.py

import logging
from easymsx.easymsx import EasyMSX
from rulemsx.rulemsx import RuleMSX
from rulemsx.ruleevaluator import RuleEvaluator
from rulemsx.action import Action
from rulemsx.datapointsource import DataPointSource
from rulemsx.rulecondition import RuleCondition

class RuleMSXDemo:
    
    def __init__(self):

        print("Initialising RuleMSX...")
        self.rulemsx = RuleMSX(logging.CRITICAL)
        print("RuleMSX initialised...")
        
        
        print("Initialising EasyMSX...")
        self.easymsx = EasyMSX()
        print("EasyMSX initialised...")
        
        self.easymsx.orders.add_notification_handler(self.process_notification)
        self.easymsx.routes.add_notification_handler(self.process_notification)

        print("Create RuleSet...")
        self.build_rules()
        print("RuleSet ready...")

        self.easymsx.start()
             
    class StringEqualityEvaluator(RuleEvaluator):
        
        def __init__(self, datapoint_name, target_value, additional_dep=None):
            
            #print("Initializing StringEqualityEvaluator for DataPoint: " + datapoint_name)

            self.datapoint_name = datapoint_name
            self.target_value = target_value
            super().add_dependent_datapoint_name(datapoint_name)
            if not additional_dep==None:
                super().add_dependent_datapoint_name(additional_dep)

            #print("Initialized StringEqualityEvaluator for DataPoint: " + datapoint_name)
        
        def evaluate(self,dataset):
            dp_value = dataset.datapoints[self.datapoint_name].get_value()
            #print("Evaluated StringEqualityEvaluator for DataPoint: " + self.datapoint_name + " of DataSet: " + dataset.name + " - Returning: " + str(dp_value==self.target_value))
            return dp_value==self.target_value
        
    class SendMessageWithDataPointValue(Action):
        
        def __init__(self,msg_str, datapoint_name1, datapoint_name2=None):
            
            #print("Initializing SendMessageWithDataPointValue for msg: " + msg_str)

            self.msg_str = msg_str
            self.datapoint_name1 = datapoint_name1
            self.datapoint_name2 = datapoint_name2
            
        def execute(self,dataset):
            dp_value1 = dataset.datapoints[self.datapoint_name1].get_value()
            if not self.datapoint_name2 == None:
                dp_value2 = dataset.datapoints[self.datapoint_name2].get_value()
                print (self.msg_str + dp_value1 + "/" + dp_value2)
            else:
                print (self.msg_str + dp_value1)
            
        
    class ShowFillEvent(Action):
        
        def __init__(self, easymsx):
            #print("Initializing ShowFillEvent")
            self.easymsx = easymsx
        
        def execute(self,dataset):
            dp_order_no = dataset.datapoints["OrderNumber"].get_value()
            o = self.easymsx.orders.get_by_sequence_no(int(dp_order_no))
            filled_amount = o.field("EMSX_FILLED").value() 
            print("Order Completed: " + dp_order_no + "\tFilled: " + filled_amount)
            
    class ShowRouteFillEvent(Action):
        
        def __init__(self, easymsx):
            #print("Initializing ShowRouteFillEvent")
            self.easymsx = easymsx
        
        def execute(self,dataset):
            dp_order_no = dataset.datapoints["OrderNumber"].get_value()
            dp_route_id = dataset.datapoints["RouteID"].get_value()
            dp_amount = dataset.datapoints["Amount"].get_value()
            dp_filled = dataset.datapoints["Filled"].get_value()
            o = self.easymsx.routes.get_by_sequence_no_and_id(int(dp_order_no), int(dp_route_id))
            if int(o.field("EMSX_WORKING").value()) == 0:
                print("Route Completed: " + dp_order_no + "/" + dp_route_id + "\tFilled: " + dp_filled)
            else:
                print("Route PartFilled: " + dp_order_no + "/" + dp_route_id + "\tFilled: " + dp_filled + " of " + dp_amount)

    class EMSXFieldDataPointSource(DataPointSource):

        def __init__(self, field):
            #print("Initializing EMSXFieldDataPointSource for field: " + field.name())
            self.source = field
            field.add_notification_handler(self.process_notification)
            
        def get_value(self):
            #print("GetValue of EMSXFieldDataPointSource for field: " + self.source.name())
            return self.source.value()
        
        def process_notification(self, notification):
            #print("SetValue of EMSXFieldDataPointSource for field: " + self.source.name())
            super().set_stale()
            
            
    def build_rules(self):
        
        print("Building Rules...")

        cond_order_status_new = RuleCondition("OrderStatusIsNew", self.StringEqualityEvaluator("OrderStatus","NEW"))
        cond_order_status_working = RuleCondition("OrderStatusIsWorking", self.StringEqualityEvaluator("OrderStatus","WORKING"))
        cond_order_status_filled = RuleCondition("OrderStatusIsFilled", self.StringEqualityEvaluator("OrderStatus","FILLED"))

        action_order_send_new_message = self.rulemsx.create_action("OrderSendNewMessage", self.SendMessageWithDataPointValue("New Order Created: ", "OrderNumber"))
        action_order_send_ack_message = self.rulemsx.create_action("OrderSendAckMessage", self.SendMessageWithDataPointValue("Broker Acknowledged Order: ", "OrderNumber"))
        action_order_send_fill_message = self.rulemsx.create_action("OrderSendFillMessage", self.ShowFillEvent(self.easymsx))
        
        cond_route_status_new = RuleCondition("RouteStatusIsNew", self.StringEqualityEvaluator("RouteStatus","NEW"))
        cond_route_status_working = RuleCondition("RouteStatusIsWorking", self.StringEqualityEvaluator("RouteStatus","WORKING"))
        cond_route_status_partfilled = RuleCondition("RouteStatusIsPartFilled", self.StringEqualityEvaluator("RouteStatus","PARTFILL", "Filled"))
        cond_route_status_filled = RuleCondition("RouteStatusIsFilled", self.StringEqualityEvaluator("RouteStatus","FILLED"))
        
        action_route_send_new_message = self.rulemsx.create_action("RouteSendNewMessage", self.SendMessageWithDataPointValue("New Order Created: ", "OrderNumber", "RouteID"))
        action_route_send_ack_message = self.rulemsx.create_action("RouteSendAckMessage", self.SendMessageWithDataPointValue("Broker Acknowledged Route: ", "OrderNumber", "RouteID"))
        action_route_send_partfill_message = self.rulemsx.create_action("RouteSendPartFillMessage", self.ShowRouteFillEvent(self.easymsx))
        action_route_send_fill_message = self.rulemsx.create_action("RouteSendFillMessage", self.ShowRouteFillEvent(self.easymsx))

        demo_order_ruleset = self.rulemsx.create_ruleset("demoOrderRuleSet")

        rule_new_order = demo_order_ruleset.add_rule("NewOrder")
        rule_new_order.add_rule_condition(cond_order_status_new)
        rule_new_order.add_action(action_order_send_new_message)
        
        rule_working_order = demo_order_ruleset.add_rule("WorkingOrder")
        rule_working_order.add_rule_condition(cond_order_status_working)
        rule_working_order.add_action(action_order_send_ack_message)
        
        rule_filled_order = demo_order_ruleset.add_rule("FilledOrder")
        rule_filled_order.add_rule_condition(cond_order_status_filled)
        rule_filled_order.add_action(action_order_send_fill_message)
        
        demo_route_ruleset = self.rulemsx.create_ruleset("demoRouteRuleSet")

        rule_new_route = demo_route_ruleset.add_rule("NewRoute")
        rule_new_route.add_rule_condition(cond_route_status_new)
        rule_new_route.add_action(action_route_send_new_message)
        
        rule_working_route = demo_route_ruleset.add_rule("WorkingRoute")
        rule_working_route.add_rule_condition(cond_route_status_working)
        rule_working_route.add_action(action_route_send_ack_message)
        
        rule_part_filled_route = demo_route_ruleset.add_rule("PartFilledRoute")
        rule_part_filled_route.add_rule_condition(cond_route_status_partfilled)
        rule_part_filled_route.add_action(action_route_send_partfill_message)

        rule_filled_route = demo_route_ruleset.add_rule("FilledRoute")
        rule_filled_route.add_rule_condition(cond_route_status_filled)
        rule_filled_route.add_action(action_route_send_fill_message)

        print("Rules built.")


    def process_notification(self,notification):

        if notification.category == EasyMSX.NotificationCategory.ORDER:
            if notification.type == EasyMSX.NotificationType.NEW or notification.type == EasyMSX.NotificationType.INITIALPAINT: 
                print("EasyMSX Notification ORDER -> NEW/INIT_PAINT: " + notification.source.field("EMSX_SEQUENCE").value())
                self.parse_order(notification.source)
        
        if notification.category == EasyMSX.NotificationCategory.ROUTE:
            if notification.type == EasyMSX.NotificationType.NEW or notification.type == EasyMSX.NotificationType.INITIALPAINT: 
                print("EasyMSX Notification ROUTE -> NEW/INIT_PAINT: " + notification.source.field("EMSX_SEQUENCE").value() + "/" + notification.source.field("EMSX_ROUTE_ID").value())
                self.parse_route(notification.source)
            
        
    def parse_order(self,o):
        
        print("Parse Order: " + o.field("EMSX_SEQUENCE").value())

        new_dataset = self.rulemsx.create_dataset("DS_OR_" + o.field("EMSX_SEQUENCE").value())

        new_dataset.add_datapoint("OrderStatus", self.EMSXFieldDataPointSource(o.field("EMSX_STATUS")))
        new_dataset.add_datapoint("OrderNumber", self.EMSXFieldDataPointSource(o.field("EMSX_SEQUENCE")))

        self.rulemsx.rulesets["demoOrderRuleSet"].execute(new_dataset)

        #print("Parse Order: " + o.field("EMSX_SEQUENCE").value()+ "...done.")

    def parse_route(self,r):
        
        print("Parse Route: " + r.field("EMSX_SEQUENCE").value() + "/" + r.field("EMSX_ROUTE_ID").value())

        new_dataset = self.rulemsx.create_dataset("DS_RT_" + r.field("EMSX_SEQUENCE").value() + r.field("EMSX_ROUTE_ID").value())

        new_dataset.add_datapoint("RouteStatus", self.EMSXFieldDataPointSource(r.field("EMSX_STATUS")))
        new_dataset.add_datapoint("OrderNumber", self.EMSXFieldDataPointSource(r.field("EMSX_SEQUENCE")))
        new_dataset.add_datapoint("RouteID", self.EMSXFieldDataPointSource(r.field("EMSX_ROUTE_ID")))
        new_dataset.add_datapoint("Filled", self.EMSXFieldDataPointSource(r.field("EMSX_FILLED")))
        new_dataset.add_datapoint("Amount", self.EMSXFieldDataPointSource(r.field("EMSX_AMOUNT")))

        self.rulemsx.rulesets["demoRouteRuleSet"].execute(new_dataset)

        #print("Parse Route: " + r.field("EMSX_SEQUENCE").value() + r.field("EMSX_ROUTE_ID").value() + "...done.")


if __name__ == '__main__':
    
    rulemsxdemo = RuleMSXDemo();
    
    input("Press any to terminate\n")

    print("Terminating...\n")

    rulemsxdemo.rulemsx.stop()
    
    quit()
    