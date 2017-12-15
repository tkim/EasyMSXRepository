# RuleMSXDemo.py

from EasyMSX import EasyMSX
from rulemsx import RuleMSX
from rulecondition import RuleCondition
from ruleevaluator import RuleEvaluator
from action import Action
from datapointsource import DataPointSource
import logging

class RuleMSXDemo:
    
    def __init__(self):

        print("Initialising RuleMSX...")
        self.ruleMSX = RuleMSX(logging.INFO)
        print("RuleMSX initialised...")
        
        
        print("Initialising EasyMSX...")
        self.easyMSX = EasyMSX()
        print("EasyMSX initialised...")
        
        self.easyMSX.orders.addNotificationHandler(self.processNotification)
        self.easyMSX.routes.addNotificationHandler(self.processNotification)

        print("Create RuleSet...")
        self.buildRules()
        print("RuleSet ready...")

        self.easyMSX.start()
             
    class StringEqualityEvaluator(RuleEvaluator):
        
        def __init__(self, dataPointName, targetValue, additionalDep=None):
            self.dataPointName = dataPointName
            self.targetValue = targetValue
            super().addDependentDataPointName(dataPointName)
            if not additionalDep==None:
                super().addDependentDataPointName(additionalDep)
        
        def evaluate(self,dataSet):
            dpValue = dataSet.dataPoints[self.dataPointName].getValue()
            return dpValue==self.targetValue
        
    class SendMessageWithDataPointValue(Action):
        
        def __init__(self,msgStr, dataPointName1, dataPointName2=None):
            self.msgStr = msgStr
            self.dataPointName1 = dataPointName1
            self.dataPointName2 = dataPointName2
            
        def execute(self,dataSet):
            dpValue1 = dataSet.dataPoints[self.dataPointName1].getValue()
            if not self.dataPointName2 == None:
                dpValue2 = dataSet.dataPoints[self.dataPointName2].getValue()
                print (self.msgStr + dpValue1 + "/" + dpValue2)
            else:
                print (self.msgStr + dpValue1)
            
        
    class ShowFillEvent(Action):
        
        def __init__(self, easyMSX):
            self.easyMSX = easyMSX
        
        def execute(self,dataSet):
            dpOrderNo = dataSet.dataPoints["OrderNumber"].getValue()
            o = self.easyMSX.orders.getBySequenceNo(int(dpOrderNo))
            filledAmount = o.field("EMSX_FILLED").value() 
            print("Order Completed: " + dpOrderNo + "\tFilled: " + filledAmount)
            
    class ShowRouteFillEvent(Action):
        
        def __init__(self, easyMSX):
            self.easyMSX = easyMSX
        
        def execute(self,dataSet):
            dpOrderNo = dataSet.dataPoints["OrderNumber"].getValue()
            dpRouteID = dataSet.dataPoints["RouteID"].getValue()
            dpAmount = dataSet.dataPoints["Amount"].getValue()
            dpFilled = dataSet.dataPoints["Filled"].getValue()
            o = self.easyMSX.routes.getBySequenceNoAndId(int(dpOrderNo), int(dpRouteID))
            if int(o.field("EMSX_WORKING").value()) == 0:
                print("Route Completed: " + dpOrderNo + "/" + dpRouteID)
            else:
                print("Route PartFilled: " + dpOrderNo + "/" + dpRouteID + "\t" + dpFilled + " of " + dpAmount)

    class EMSXFieldDataPointSource(DataPointSource):

        def __init__(self, field):
            self.source = field
            field.addNotificationHandler(self.processNotification)
            
        def getValue(self):
            return self.source.value()
        
        def processNotification(self, notification):
            super().setStale()
            
            
    def buildRules(self):
        
        condOrderStatusNew = RuleCondition("OrderStatusIsNew", self.StringEqualityEvaluator("OrderStatus","NEW"))
        condOrderStatusWorking = RuleCondition("OrderStatusIsWorking", self.StringEqualityEvaluator("OrderStatus","WORKING"))
        condOrderStatusFilled = RuleCondition("OrderStatusIsFilled", self.StringEqualityEvaluator("OrderStatus","FILLED"))

        actionOrderSendNewMessage = self.ruleMSX.createAction("OrderSendNewMessage", self.SendMessageWithDataPointValue("New Order Created: ", "OrderNumber"))
        actionOrderSendAckMessage = self.ruleMSX.createAction("OrderSendAckMessage", self.SendMessageWithDataPointValue("Broker Acknowledged Order: ", "OrderNumber"))
        actionOrderSendFillMessage = self.ruleMSX.createAction("OrderSendFillMessage", self.ShowFillEvent(self.easyMSX))
        
        condRouteStatusNew = RuleCondition("RouteStatusIsNew", self.StringEqualityEvaluator("RouteStatus","NEW"))
        condRouteStatusWorking = RuleCondition("RouteStatusIsWorking", self.StringEqualityEvaluator("RouteStatus","WORKING"))
        condRouteStatusPartFilled = RuleCondition("RouteStatusIsPartFilled", self.StringEqualityEvaluator("RouteStatus","PARTFILL", "Filled"))
        condRouteStatusFilled = RuleCondition("RouteStatusIsFilled", self.StringEqualityEvaluator("RouteStatus","FILLED"))
        
        actionRouteSendNewMessage = self.ruleMSX.createAction("RouteSendNewMessage", self.SendMessageWithDataPointValue("New Order Created: ", "OrderNumber", "RouteID"))
        actionRouteSendAckMessage = self.ruleMSX.createAction("RouteSendAckMessage", self.SendMessageWithDataPointValue("Broker Acknowledged Route: ", "OrderNumber", "RouteID"))
        actionRouteSendPartFillMessage = self.ruleMSX.createAction("RouteSendPartFillMessage", self.ShowRouteFillEvent(self.easyMSX))
        actionRouteSendFillMessage = self.ruleMSX.createAction("RouteSendFillMessage", self.ShowRouteFillEvent(self.easyMSX))

        demoOrderRuleSet = self.ruleMSX.createRuleSet("demoOrderRuleSet")

        ruleNewOrder = demoOrderRuleSet.addRule("NewOrder")
        ruleNewOrder.addRuleCondition(condOrderStatusNew)
        ruleNewOrder.addAction(actionOrderSendNewMessage)
        
        ruleWorkingOrder = demoOrderRuleSet.addRule("WorkingOrder")
        ruleWorkingOrder.addRuleCondition(condOrderStatusWorking)
        ruleWorkingOrder.addAction(actionOrderSendAckMessage)
        
        ruleFilledOrder = demoOrderRuleSet.addRule("FilledOrder")
        ruleFilledOrder.addRuleCondition(condOrderStatusFilled)
        ruleFilledOrder.addAction(actionOrderSendFillMessage)
        
        demoRouteRuleSet = self.ruleMSX.createRuleSet("demoRouteRuleSet")

        ruleNewRoute = demoRouteRuleSet.addRule("NewRoute")
        ruleNewRoute.addRuleCondition(condRouteStatusNew)
        ruleNewRoute.addAction(actionRouteSendNewMessage)
        
        ruleWorkingRoute = demoRouteRuleSet.addRule("WorkingRoute")
        ruleWorkingRoute.addRuleCondition(condRouteStatusWorking)
        ruleWorkingRoute.addAction(actionRouteSendAckMessage)
        
        rulePartFilledRoute = demoRouteRuleSet.addRule("PartFilledRoute")
        rulePartFilledRoute.addRuleCondition(condRouteStatusPartFilled)
        rulePartFilledRoute.addAction(actionRouteSendPartFillMessage)

        ruleFilledRoute = demoRouteRuleSet.addRule("FilledRoute")
        ruleFilledRoute.addRuleCondition(condRouteStatusFilled)
        ruleFilledRoute.addAction(actionRouteSendFillMessage)

    def processNotification(self,notification):

        if notification.category == EasyMSX.NotificationCategory.ORDER:
            if notification.type == EasyMSX.NotificationType.NEW or notification.type == EasyMSX.NotificationType.INITIALPAINT: 
                self.parseOrder(notification.source)
        
        if notification.category == EasyMSX.NotificationCategory.ROUTE:
            if notification.type == EasyMSX.NotificationType.NEW or notification.type == EasyMSX.NotificationType.INITIALPAINT: 
                self.parseRoute(notification.source)
            

        
    def parseOrder(self,o):
        
        newDataSet = self.ruleMSX.createDataSet("DS_OR_" + o.field("EMSX_SEQUENCE").value())

        newDataSet.addDataPoint("OrderStatus", self.EMSXFieldDataPointSource(o.field("EMSX_STATUS")))
        newDataSet.addDataPoint("OrderNumber", self.EMSXFieldDataPointSource(o.field("EMSX_SEQUENCE")))

        self.ruleMSX.ruleSets["demoOrderRuleSet"].execute(newDataSet)


    def parseRoute(self,r):
        
        newDataSet = self.ruleMSX.createDataSet("DS_RT_" + r.field("EMSX_SEQUENCE").value() + r.field("EMSX_ROUTE_ID").value())

        newDataSet.addDataPoint("RouteStatus", self.EMSXFieldDataPointSource(r.field("EMSX_STATUS")))
        newDataSet.addDataPoint("OrderNumber", self.EMSXFieldDataPointSource(r.field("EMSX_SEQUENCE")))
        newDataSet.addDataPoint("RouteID", self.EMSXFieldDataPointSource(r.field("EMSX_ROUTE_ID")))
        newDataSet.addDataPoint("Filled", self.EMSXFieldDataPointSource(r.field("EMSX_FILLED")))
        newDataSet.addDataPoint("Amount", self.EMSXFieldDataPointSource(r.field("EMSX_AMOUNT")))

        self.ruleMSX.ruleSets["demoRouteRuleSet"].execute(newDataSet)


if __name__ == '__main__':
    
    ruleMSXDemo = RuleMSXDemo();
    
    input("Press any to terminate\n")

    print("Terminating...\n")

    quit()
    