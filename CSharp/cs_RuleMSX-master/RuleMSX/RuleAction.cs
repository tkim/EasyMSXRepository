namespace com.bloomberg.samples.rulemsx
{
    public class RuleAction
    {
        string name;
        ActionExecutor executor;

        internal RuleAction(string name)
        {
            this.name = name;
        }
        internal RuleAction(string name, ActionExecutor executor)
        {
            this.name = name;
            this.addExecutor(executor);
        }

        public void addExecutor(ActionExecutor executor)
        {
            this.executor = executor;
        }

        public string getName()
        {
            return this.name;
        }

        public ActionExecutor getExecutor()
        {
            return this.executor;
        }
    }
}
