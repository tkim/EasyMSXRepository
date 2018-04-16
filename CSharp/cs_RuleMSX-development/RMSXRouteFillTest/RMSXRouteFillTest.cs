using System;
using com.bloomberg.samples.rulemsx;
using com.bloomberg.emsx.samples;
using Action = com.bloomberg.samples.rulemsx.Action;
using System.Globalization;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;

namespace RMSXRouteFillTest
{
    class RMSXRouteFillTest: NotificationHandler
    {

        RuleMSX rmsx;
        EasyMSX emsx;

        static void Main(string[] args)
        {
            RMSXRouteFillTest Test = new RMSXRouteFillTest();
            Test.Run();

            System.Console.WriteLine("Press enter to terminate...");
            System.Console.ReadLine();

            Test.Stop();

            System.Console.WriteLine("Terminating.");
        }

        private void log(String msg)
        {
            System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfffzzz") + "(RMSXRouteFillTest): \t" + msg);
        }

        private void Run()
        {

            log("Initializing RuleMSX...");
            this.rmsx = new RuleMSX();
            LogRmsx.logLevel = LogRmsx.LogLevels.DETAILED;
            LogRmsx.logPrefix = "(RuleMSX..........)";

            log("RuleMSX initialized.");

            log("Initializing EasyMSX...");
            this.emsx = new EasyMSX();
            log("EasyMSX initialized.");

            log("Create ruleset...");
            BuildRules();
            log("Ruleset ready.");

            this.emsx.routes.addNotificationHandler(this);

            log("Starting EasyMSX");
            this.emsx.start();
            log("EasyMSX started");
        }

        private void Stop()
        {
            log("Stopping RuleMSX");
            this.rmsx.Stop();
            log("RuleMSX stopped");
        }

        private void BuildRules()
        {
            log("Building rules...");

            log("Creating RuleCondition condRouteFillOccurred");
            RuleCondition condRouteFillOccurred = new RuleCondition("RouteFillOccurred", new RouteFillOccurred(this));
            log("RuleCondition condRouteFillOccurred created.");

            log("Creating Action actShowRouteFill");
            Action actShowRouteFill = this.rmsx.CreateAction("ShowRouteFill", new ShowRouteFill(this));
            log("Action actShowRouteFill created");

            log("Creating RuleSet demoRouteRuleSet");
            RuleSet demoRouteRuleSet = this.rmsx.CreateRuleSet("DemoRouteRuleSet");
            log("RuleSet demoRouteRuleSet created");

            log("Creating Rule ruleRouteFilled");
            Rule ruleRouteFilled = demoRouteRuleSet.AddRule("RouteFilled");
            log("Rule ruleRouteFilled created");

            log("Assign RuleCondition condRouteFillOccurred to Rule ruleRouteFilled");
            ruleRouteFilled.AddRuleCondition(condRouteFillOccurred);

            log("Assign Action actShowRouteFill to Rule ruleRouteFilled");
            ruleRouteFilled.AddAction(actShowRouteFill);

            log("Rules built.");

        }

        public void processNotification(Notification notification)
        {

            log("EasyMSX notification recieved...");
            if (notification.category == Notification.NotificationCategory.ROUTE)
            {
                log("Notification is at ROUTE level");
                if ((notification.type == Notification.NotificationType.NEW) || (notification.type == Notification.NotificationType.INITIALPAINT))
                {
                    if ((notification.getRoute().field("EMSX_STATUS").value() == "FILLED") || (notification.getRoute().field("EMSX_STATUS").value() == "CANCEL"))
                    {
                        log("Route " + notification.getRoute().field("EMSX_SEQUENCE").value() + "." + notification.getRoute().field("EMSX_ROUTE_ID").value() + " is " + notification.getRoute().field("EMSX_STATUS").value() + " - Ignoring");
                    } else { 
                        log("Notification is NEW or INITIAL_PAINT");
                        log("EasyMSX Notification Route -> NEW/INIT_PAINT: " + notification.getRoute().field("EMSX_SEQUENCE").value() + "." + notification.getRoute().field("EMSX_ROUTE_ID").value());
                        this.parseRoute(notification.getRoute());
                    }
                }
            }
        }

        public void parseRoute(Route r)
        {
            log("Parse Route: " + r.field("EMSX_SEQUENCE").value() + "." + r.field("EMSX_ROUTE_ID").value());

            log("Creating newDataSet");
            DataSet newDataSet = this.rmsx.CreateDataSet("DS_RT_" + r.field("EMSX_SEQUENCE").value() + "." + r.field("EMSX_ROUTE_ID").value());

            log("Creating RouteStatus DataPoint");
            newDataSet.AddDataPoint("RouteStatus", new EMSXFieldDataPointSource(this, r.field("EMSX_STATUS")));

            log("Creating RouteOrderNumber DataPoint");
            newDataSet.AddDataPoint("RouteOrderNumber", new EMSXFieldDataPointSource(this, r.field("EMSX_SEQUENCE")));

            log("Creating RouteID DataPoint");
            newDataSet.AddDataPoint("RouteID", new EMSXFieldDataPointSource(this, r.field("EMSX_ROUTE_ID")));

            log("Creating RouteFilled DataPoint");
            newDataSet.AddDataPoint("RouteFilled", new EMSXFieldDataPointSource(this, r.field("EMSX_FILLED")));

            log("Creating RouteAmount DataPoint");
            newDataSet.AddDataPoint("RouteAmount", new EMSXFieldDataPointSource(this, r.field("EMSX_AMOUNT")));

            log("Creating RouteLastShares DataPoint");
            newDataSet.AddDataPoint("RouteLastShares", new EMSXFieldDataPointSource(this, r.field("EMSX_LAST_SHARES")));

            log("Creating LastFillShown DataPoint");
            newDataSet.AddDataPoint("LastFillShown", new GenericIntegerDataPointSource(this, 0));

            log("Executing RuleSet DemoRouteRuleSet with DataSet " + newDataSet.GetName());
            this.rmsx.GetRuleSet("DemoRouteRuleSet").Execute(newDataSet);

        }

        class GenericIntegerDataPointSource: DataPointSource
        {
            int value;
            RMSXRouteFillTest parent;

            internal GenericIntegerDataPointSource(RMSXRouteFillTest parent, int initialValue)
            {
                parent.log("Creating new GenericIntegerDataPointSource with initial value: " + initialValue);

                this.value = initialValue;
                this.parent = parent;
            }

            public override object GetValue()
            {
                this.parent.log("Returning value for GenericIntegerDataPointSource - Value: " + this.value);
                return this.value;
            }

            public void setValue(int newValue)
            {
                this.value = newValue;
                this.SetStale();
            }
        }

        class EMSXFieldDataPointSource : DataPointSource, NotificationHandler
        {
            Field field;
            String value;
            String previousValue;
            RMSXRouteFillTest parent;

            internal EMSXFieldDataPointSource(RMSXRouteFillTest parent, Field field)
            {
                this.parent = parent;
                this.field = field;
                this.value = field.value();
                this.previousValue = null;

                parent.log("Creating new EMSXFieldDataPointSource for Field: " + field.name() + "\tValue: " + this.value + "\tPrevious Value:" + this.previousValue);

                parent.log("Adding EasyMSX field level notification handler for Field: " + field.name());
                this.field.addNotificationHandler(this);
                parent.log("New EMSXFieldDataPointSource created");

            }

            public override object GetValue()
            {
                //this.parent.log("Returning value for field " + this.field.name() + "\tValue: " + this.value);
                return this.value;
            }

            public object GetPreviousValue()
            {
                this.parent.log("Returning previous value for field " + this.field.name() + "\tPrevious Value: " + this.previousValue);
                return this.previousValue;
            }

            public void processNotification(Notification notification)
            {
                this.parent.log("Handling EasyMSX field notifiction for field " + this.field.name());

                this.previousValue = this.value;
                this.value = notification.getFieldChanges()[0].newValue;

                this.parent.log("-- Value: " + this.value + "\tPrevious: " + this.previousValue);
                if(this.previousValue != this.value)
                {
                    this.parent.log("-- Values differ - calling SetStale");
                    this.SetStale();
                    this.parent.log("-- Returned from SetStale");
                }
            }
        }

        class RouteFillOccurred : RuleEvaluator
        {
            RMSXRouteFillTest parent;

            public RouteFillOccurred(RMSXRouteFillTest parent)
            {
                this.parent = parent;
                this.parent.log("Creating new RouteFillOccurred");

                this.parent.log("Adding dependency for RouteStatus");
                this.AddDependantDataPointName("RouteStatus");
                this.parent.log("Adding dependency for RouteFilled");
                this.AddDependantDataPointName("RouteFilled");
                this.parent.log("Adding dependency for RouteLastShares");
                this.AddDependantDataPointName("RouteLastShares");
                this.parent.log("Done Adding dependencies.");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                this.parent.log("Evaluating RouteFillOccurred...");

                EMSXFieldDataPointSource routeFilledSource = (EMSXFieldDataPointSource) dataSet.GetDataPoint("RouteFilled").GetSource();
                EMSXFieldDataPointSource routeLastSharesSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteLastShares").GetSource();
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource) dataSet.GetDataPoint("RouteStatus").GetSource();
                GenericIntegerDataPointSource lastfill = (GenericIntegerDataPointSource)dataSet.GetDataPoint("LastFillShown").GetSource();

                int currentFilled = Convert.ToInt32(routeFilledSource.GetValue());
                int previousFilled = Convert.ToInt32(routeFilledSource.GetPreviousValue());

                int currentLastShares = Convert.ToInt32(routeLastSharesSource.GetValue());
                int previousLastShares = Convert.ToInt32(routeLastSharesSource.GetPreviousValue());

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int lastFillShown = Convert.ToInt32(dataSet.GetDataPoint("LastFillShown").GetValue());

                this.parent.log("RouteFillOccurred DataSet values : currentFilled=" + currentFilled + "|previousFilled=" + previousFilled + "|currentLastShares=" + currentLastShares + "|previousLastShares=" + previousLastShares + "|currentStatus=" + currentStatus + "|previousStatus=" + previousStatus);

                bool res = ((currentFilled != previousFilled) && previousStatus != null && currentFilled != lastFillShown);

                this.parent.log("RouteFillOccurred returning value: " + res);

                return res;
            }
        }

        class ShowRouteFill : ActionExecutor
        {
            RMSXRouteFillTest parent;

            public ShowRouteFill(RMSXRouteFillTest parent)
            {
                parent.log("Creating new ShowRouteFill action executor.");
                this.parent = parent;
            }

            public void Execute(DataSet dataSet)
            {
                this.parent.log(">>> ShowRouteFill Action Executor: ");
                this.parent.log(">>> RouteStatus: " + dataSet.GetDataPoint("RouteStatus").GetValue());
                this.parent.log(">>> RouteOrderNumber: " + dataSet.GetDataPoint("RouteOrderNumber").GetValue());
                this.parent.log(">>> RouteID: " + dataSet.GetDataPoint("RouteID").GetValue());
                this.parent.log(">>> RouteFilled: " + dataSet.GetDataPoint("RouteFilled").GetValue());
                this.parent.log(">>> RouteAmount: " + dataSet.GetDataPoint("RouteAmount").GetValue());
                this.parent.log(">>> RouteLastShares: " + dataSet.GetDataPoint("RouteLastShares").GetValue());

                GenericIntegerDataPointSource lastfill = (GenericIntegerDataPointSource)dataSet.GetDataPoint("LastFillShown").GetSource();

                lastfill.setValue(Convert.ToInt32(dataSet.GetDataPoint("RouteFilled").GetValue()));
            }
        }
    }
}
