namespace com.bloomberg.emsx.samples
{
    public class BrokerStrategyParameter
    {

        public string name;
        public string value;
        public int disable;

        BrokerStrategyParameters parent = null;

        internal BrokerStrategyParameter(BrokerStrategyParameters brokerStrategyParameters, string name, string value, int disable)
        {
            this.name = name;
            this.value = value;
            this.disable = disable;
            this.parent = brokerStrategyParameters;
        }
    }
}
