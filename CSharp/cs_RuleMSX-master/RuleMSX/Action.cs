namespace com.bloomberg.samples.rulemsx
{
    public class Action
    {
        string name;
        ActionExecutor executor;

        internal Action(string name)
        {
            this.name = name;
        }
        internal Action(string name, ActionExecutor executor)
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
