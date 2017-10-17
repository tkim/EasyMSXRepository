namespace com.bloomberg.emsx.samples
{
    public class Broker
    {
        public enum AssetClass
        {
            EQTY,
            FUT,
            OPT,
            MULTILEG_OPT
        }

        public string name;
        public AssetClass assetClass;

        internal Brokers parent;

        public BrokerStrategies strategies = null;

        internal Broker(Brokers parent, string name, AssetClass assetClass)
        {
            this.parent = parent;
            this.name = name;
            this.assetClass = assetClass;
            strategies = new BrokerStrategies(this);
        }

    }
}