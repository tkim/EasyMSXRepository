
namespace com.bloomberg.emsx.samples
{
    public class BrokerStrategy
    {

        public string name;

        internal BrokerStrategies parent;

        public BrokerStrategyParameters parameters = null;

        internal BrokerStrategy(BrokerStrategies parent, string name)
        {
            this.parent = parent;
            this.name = name;
            parameters = new BrokerStrategyParameters(this);
        }


    }
}
