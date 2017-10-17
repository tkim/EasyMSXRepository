using com.bloomberg.emsx.samples;
using com.bloomberg.mktdata.samples;
using LogEmsx = com.bloomberg.emsx.samples.Log;
using EMSXField = com.bloomberg.emsx.samples.Field;
using EasyMSXNotification = com.bloomberg.emsx.samples.Notification;
using EasyMSXFieldChange = com.bloomberg.emsx.samples.FieldChange;
using EasyMSXNotificationHandler = com.bloomberg.emsx.samples.NotificationHandler;
using EasyMKTNotificationHandler = com.bloomberg.mktdata.samples.NotificationHandler;
using EasyMKTNotification = com.bloomberg.mktdata.samples.Notification;
using MessageHandler = com.bloomberg.emsx.samples.MessageHandler;
using System;
using System.Collections.Generic;
using Request = Bloomberglp.Blpapi.Request;
using Bloomberglp.Blpapi;
using com.bloomberg.samples.rulemsx;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;

namespace com.bloomberg.samples.rulemsx.test {

    public class RuleMSXTest : EasyMSXNotificationHandler {

        private RuleMSX rmsx;
        private EasyMSX emsx;
        private EasyMKT emkt;
        private RuleSet ruleSet;

        static void Main(string[] args) {

            System.Console.WriteLine("Bloomberg - RMSX - RuleMSXTest\n");

            RuleMSXTest example = new RuleMSXTest();

            System.Console.WriteLine("Press any key to terminate...");
            System.Console.ReadLine();
            example.Stop();
        }

        public RuleMSXTest() {

            System.Console.WriteLine("RuleMSXTest starting...");
            System.Console.WriteLine("Instantiating RuleMSX...");

            LogRmsx.logLevel = LogRmsx.LogLevels.NONE;

            this.rmsx = new RuleMSX();
            
            // Create new RuleSet
            this.ruleSet = rmsx.createRuleSet("TestRules");

            Rule ruleIsNotExpired = new Rule("IsNotExpired", new StringInequalityRule("OrderStatus", "EXPIRED"));
            Rule ruleIsExpired = new Rule("IsExpired", new StringEqualityRule("OrderStatus", "EXPIRED"), rmsx.createAction("SendIgnoringSignal",new SendAdditionalSignal("Ignoring Order - EXPIRED")));
            Rule ruleNeedsRouting = new Rule("NeedsRouting", new NeedsRoutingRule());
            Rule ruleIsLNExchange = new Rule("IsLNExchange", new StringEqualityRule("Exchange", "LN"), rmsx.createAction("RouteToBrokerBB", new RouteToBroker(this, "BB")));
            Rule ruleIsUSExchange = new Rule("IsUSExchange", new StringEqualityRule("Exchange", "US"));
            Rule ruleIsIBM = new Rule("IsIBM", new StringEqualityRule("Ticker", "IBM US Equity"), rmsx.createAction("RouteToBrokerEFIX", new RouteToBroker(this, "EFIX")));
            ruleIsIBM.AddAction(rmsx.createAction("SendIBMSignal", new SendAdditionalSignal("This is IBM!!")));
            Rule ruleIsMSFT = new Rule("IsMSFT", new StringEqualityRule("Ticker", "MSFT US Equity"), rmsx.createAction("RejectedSignal", new SendAdditionalSignal("Not Routing as would be rejected")));
            Rule ruleIsFilled500 = new Rule("IsFilled500", new IsFilled500Rule(), rmsx.createAction("Signal500filled", new SendAdditionalSignal("Order is filled to 500 or more.")));

            // Maybe add code so that rather than RouteToBroker("BB") we create a new datapoint "TargetBroker", set it's value to BB.
            // Then add a new rule that checks if there are available shares. if true, then action is route to targetbroker which depends on target broker

            //Add new rules for working/filled amount checks
            this.ruleSet.AddRule(ruleIsNotExpired);
            this.ruleSet.AddRule(ruleIsExpired);
            //this.ruleSet.AddRule(ruleIsFilled500);
            ruleIsNotExpired.AddRule(ruleNeedsRouting);
            ruleNeedsRouting.AddRule(ruleIsLNExchange);
            ruleNeedsRouting.AddRule(ruleIsUSExchange);
            ruleIsUSExchange.AddRule(ruleIsIBM);
            ruleIsUSExchange.AddRule(ruleIsMSFT);

            System.Console.WriteLine("...done.");

            System.Console.WriteLine(ruleSet.report());

            System.Console.WriteLine("Instantiating EasyMKT...");

            this.emkt = new EasyMKT();

            System.Console.WriteLine("...done.");
            
            // Adding subscription fields to EasyMKT
            emkt.AddField("BID");
            emkt.AddField("ASK");
            emkt.AddField("MID");
            emkt.AddField("LAST_PRICE");

            System.Console.WriteLine("Starting EasyMKT...");
            emkt.start();
            System.Console.WriteLine("EasyMKT started.");

            System.Console.WriteLine("Instantiating EasyMSX...");
            LogEmsx.logLevel = LogEmsx.LogLevels.NONE;
            try
            {
                this.emsx = new EasyMSX(EasyMSX.Environment.BETA);

                System.Console.WriteLine("EasyMSX instantiated. Adding notification handler.");

                this.emsx.orders.addNotificationHandler(this);

                System.Console.WriteLine("Starting EasyMSX");

                this.emsx.start();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            System.Console.WriteLine("EasyMSX started.");
            System.Console.WriteLine("RuleMSXTest running...");

        }

        public void Stop()
        {
            this.rmsx.Stop();
        }

        public void processNotification(EasyMSXNotification notification) {

            if (notification.category == EasyMSXNotification.NotificationCategory.ORDER) {
                if (notification.type == EasyMSXNotification.NotificationType.NEW || notification.type == EasyMSXNotification.NotificationType.INITIALPAINT)
                {
                    System.Console.WriteLine("EasyMSX Event (NEW/INITPAINT): " + notification.getOrder().field("EMSX_SEQUENCE").value());
                    parseOrder(notification.getOrder());
                }
            }
        }

        private void parseOrder(Order o) {

            bool show = false;

            // Create new DataSet for each order
            DataSet rmsxTest = this.rmsx.createDataSet("RMSXTest" + o.field("EMSX_SEQUENCE").value());
            if(show) System.Console.WriteLine("New DataSet created: " + rmsxTest.getName());

            // Create new data point for each required field

            DataPoint orderStatus = rmsxTest.addDataPoint("OrderStatus");
            orderStatus.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_STATUS")));
            if (show) System.Console.WriteLine("New DataPoint added : " + orderStatus.GetName());

            DataPoint orderNo = rmsxTest.addDataPoint("OrderNo");
            orderNo.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_SEQUENCE")));
            if (show) System.Console.WriteLine("New DataPoint added : " + orderNo.GetName());

            DataPoint assetClass = rmsxTest.addDataPoint("AssetClass");
            assetClass.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_ASSET_CLASS")));
            if (show) System.Console.WriteLine("New DataPoint added : " + assetClass.GetName());

            DataPoint amount = rmsxTest.addDataPoint("Amount");
            amount.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_AMOUNT")));
            if (show) System.Console.WriteLine("New DataPoint added : " + amount.GetName());

            DataPoint exchange = rmsxTest.addDataPoint("Exchange");
            exchange.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_EXCHANGE")));
            if (show) System.Console.WriteLine("New DataPoint added : " + exchange.GetName());

            DataPoint ticker = rmsxTest.addDataPoint("Ticker");
            ticker.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_TICKER")));
            if (show) System.Console.WriteLine("New DataPoint added : " + ticker.GetName());

            DataPoint working = rmsxTest.addDataPoint("Working");
            working.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_WORKING")));
            if (show) System.Console.WriteLine("New DataPoint added : " + working.GetName());

            DataPoint filled = rmsxTest.addDataPoint("Filled");
            filled.SetDataPointSource(new EMSXFieldDataPoint(o.field("EMSX_FILLED")));
            if (show) System.Console.WriteLine("New DataPoint added : " + filled.GetName());

            DataPoint isin = rmsxTest.addDataPoint("ISIN");
            isin.SetDataPointSource(new RefDataDataPoint("ID_ISIN", o.field("EMSX_TICKER").value()));
            if (show) System.Console.WriteLine("New DataPoint added : " + isin.GetName());

            DataPoint notified = rmsxTest.addDataPoint("Notified");
            notified.SetDataPointSource(new DynamicBoolDataPoint("Notified", false));
            if (show) System.Console.WriteLine("New DataPoint added : " + notified.GetName());

            if (show) System.Console.WriteLine("Adding order secuity to EasyMKT...");
            Security sec = emkt.securities.Get(o.field("EMSX_TICKER").value());

            if (sec == null)
            {
                sec = emkt.AddSecurity(o.field("EMSX_TICKER").value());
            }

            DataPoint lastPrice = rmsxTest.addDataPoint("LastPrice");
            lastPrice.SetDataPointSource(new MktDataDataPoint("LAST_PRICE", sec));
            if (show) System.Console.WriteLine("New DataPoint added : " + lastPrice.GetName());

            DataPoint margin = rmsxTest.addDataPoint("Margin");
            margin.SetDataPointSource(new CustomNumericDataPoint(2.0f));
            if (show) System.Console.WriteLine("New DataPoint added : " + margin.GetName());

            DataPoint price = rmsxTest.addDataPoint("NewPrice");
            price.SetDataPointSource(new CustomCompoundDataPoint(margin, lastPrice));
            if (show) System.Console.WriteLine("New DataPoint added : " + price.GetName());

            this.ruleSet.Execute(rmsxTest);
        }


        class EMSXFieldDataPoint : DataPointSource, EasyMSXNotificationHandler {

            private EMSXField source;

            internal EMSXFieldDataPoint(EMSXField source) {
                this.source = source;
                System.Console.WriteLine("Adding field notification handler for field: " + source.name());
                source.addNotificationHandler(this);
            }

            public override object GetValue()
            {
                return this.source.value().ToString();
            }

            public void processNotification(EasyMSXNotification notification) {

               // System.Console.WriteLine("Notification event: " + this.source.name() + " on " + getDataPoint().GetDataSet().getName());

                try {
                    System.Console.WriteLine("Category: " + notification.category.ToString() + "\tType: " + notification.type.ToString() + "\tOrder: " + notification.getOrder().field("EMSX_SEQUENCE").value());
                    foreach (EasyMSXFieldChange fc in notification.getFieldChanges())
                    {
                        System.Console.WriteLine("\tName: " + fc.field.name() + "\tOld: " + fc.oldValue + "\tNew: " + fc.newValue);
                    }
                }
                catch (Exception ex) {
                    System.Console.WriteLine("Failed!!: " + ex.ToString());
                }

                this.SetStale();
            }

        }

        class RefDataDataPoint : DataPointSource {

            private string fieldName;
            private string ticker;
            private string value;
            private bool isStale = true;

            internal RefDataDataPoint(String fieldName, String ticker) {
                this.fieldName = fieldName;
                this.ticker = ticker;
                this.value = "";
            }

            public override object GetValue() {

                // make ref data call to get the field value for supplied ticker
                this.SetStale();
                return "value";
            }

        }

        class MktDataDataPoint : DataPointSource, EasyMKTNotificationHandler {

            private string fieldName;
            private Security security;
            private string value;

            internal MktDataDataPoint(String fieldName, Security security) {
                this.fieldName = fieldName;
                this.security = security;
                this.security.field(this.fieldName).AddNotificationHandler(this);
            }

            public override object GetValue() {
                return value;
            }

            public void ProcessNotification(EasyMKTNotification notification) {

                if (notification.type == com.bloomberg.mktdata.samples.Notification.NotificationType.FIELD) {
                    this.value = notification.GetFieldChanges()[0].newValue;
                    //System.Console.WriteLine("Update for " + this.security.GetName() + ": " + this.fieldName + "=" + this.value);
                }
                this.SetStale();
            }

        }

        class CustomNumericDataPoint : DataPointSource {

            private float value;

            internal CustomNumericDataPoint(float value) {
                this.value = value;
            }

            public override object GetValue() {
                return value;
            }

        }

        class CustomCompoundDataPoint : DataPointSource {

            private DataPoint margin;
            private DataPoint lastPrice;
            private float value;
            private bool isStale = true;

            internal CustomCompoundDataPoint(DataPoint margin, DataPoint lastPrice) {
                this.margin = margin;
                this.lastPrice = lastPrice;
            }

            public override object GetValue() {

                if (this.isStale) {
                    this.value = (float)margin.GetSource().GetValue() + (float)lastPrice.GetSource().GetValue();
                    this.isStale = false;
                }
                return value;
            }

        }

        class DynamicBoolDataPoint : DataPointSource
        {

            private volatile bool value;

            internal DynamicBoolDataPoint(string name, bool initial)
            {
                this.value = initial;
            }

            public override object GetValue()
            {
                return value;
            }

            public void setValue(bool val)
            {

                Console.WriteLine("Setting notified status to true");
                this.value = val;
                Console.WriteLine("notified status :" + this.value.ToString());
                this.SetStale();
            }
        }

        class StringEqualityRule : RuleEvaluator {

            string dataPointName;
            string match;

            internal StringEqualityRule(string dataPointName, string match) {
                this.dataPointName = dataPointName;
                this.addDependantDataPointName(dataPointName);
                this.match = match;
            }

            public override bool Evaluate(DataSet dataSet) {
                DataPoint dp = dataSet.getDataPoint(this.dataPointName);
                DataPointSource dps = dp.GetSource();
                string val = dps.GetValue().ToString();
                return val.Equals(this.match);
            }

        }

        class StringInequalityRule : RuleEvaluator {

            string dataPointName;
            string match;

            internal StringInequalityRule(string dataPointName, string match) {
                this.dataPointName = dataPointName;
                this.addDependantDataPointName(dataPointName);
                this.match = match;
            }

            public override bool Evaluate(DataSet dataSet) {
                return !dataSet.getDataPoint(this.dataPointName).GetSource().GetValue().Equals(this.match);
            }

        }

        class NeedsRoutingRule: RuleEvaluator {

            internal NeedsRoutingRule() {}

            public override bool Evaluate(DataSet dataSet)
            {
                int workingAmount = Convert.ToInt32(dataSet.getDataPoint("Working").GetSource().GetValue().ToString());
                int filledAmount = Convert.ToInt32(dataSet.getDataPoint("Filled").GetSource().GetValue().ToString());
                int amount = Convert.ToInt32(dataSet.getDataPoint("Amount").GetSource().GetValue().ToString());
                string status = dataSet.getDataPoint("OrderStatus").GetSource().GetValue().ToString();
                
                if(status.Equals("NEW") || status.Equals("INIT_PAINT") || status.Equals("WORKING") || status.Equals("ASSIGN"))
                {
                    if (workingAmount + filledAmount < amount) return true;
                }
                return false;
            }
        }

        class IsFilled500Rule : RuleEvaluator
        {

            internal IsFilled500Rule() {
                addDependantDataPointName("Filled");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int filledAmount = Convert.ToInt32(dataSet.getDataPoint("Filled").GetSource().GetValue().ToString());

                if (filledAmount >= 500) {
                    DataPointSource dps = dataSet.getDataPoint("Notified").GetSource();
                    lock(dps)
                    {
                        bool v = Convert.ToBoolean(dps.GetValue());
                        System.Console.WriteLine("Testing DataSet: " + dataSet.getName() + ":Notified current value (bool): " + dps.GetValue().ToString() + "(" + v.ToString() + ")");
                        if (!v)
                        {
                            DynamicBoolDataPoint p = (DynamicBoolDataPoint)dps;
                            p.setValue(true);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        class RouteToBroker : ActionExecutor, MessageHandler {

            private string brokerCode;
            private RuleMSXTest ruleMSXTest;

            internal RouteToBroker(RuleMSXTest ruleMSXTest, string brokerCode) {
                this.ruleMSXTest = ruleMSXTest;
                this.brokerCode = brokerCode;
            }

            public void Execute(DataSet dataSet) {

                // create route instruction for the order based only on data from the dataset
                System.Console.WriteLine("Created route to broker '" + this.brokerCode + "' for DataSet: " + dataSet.getName());
                
                // Get the order
                Orders os = this.ruleMSXTest.emsx.orders;
                DataPoint dp = dataSet.getDataPoint("OrderNo");
                DataPointSource dps = dp.GetSource();
                string ordno = dps.GetValue().ToString();
                Order o = os.getBySequenceNo(Convert.ToInt32(ordno));

                if (o != null) {
                    Request req = this.ruleMSXTest.emsx.createRequest("RouteEx");
                    req.Set("EMSX_SEQUENCE", o.field("EMSX_SEQUENCE").value());
                    req.Set("EMSX_AMOUNT", o.field("EMSX_AMOUNT").value());
                    req.Set("EMSX_BROKER", brokerCode);
                    req.Set("EMSX_HAND_INSTRUCTION", "ANY");
                    req.Set("EMSX_ORDER_TYPE", o.field("EMSX_ORDER_TYPE").value());
                    req.Set("EMSX_TICKER", o.field("EMSX_TICKER").value());
                    req.Set("EMSX_TIF", o.field("EMSX_TIF").value());
                    System.Console.WriteLine("Sending request: " + req.ToString());

                    this.ruleMSXTest.emsx.sendRequest(req, this);
                }
            }

            public void processMessage(Message message) {
                System.Console.WriteLine("Route response: " + message);
            }
        }

        class SendAdditionalSignal : ActionExecutor {

            private String signal;
            internal SendAdditionalSignal(String signal) {
                this.signal = signal;
            }

            public void Execute(DataSet dataSet) {
                System.Console.WriteLine("Sending Signal for " + dataSet.getName() + ": " + this.signal);
            }
        }
    }
}
