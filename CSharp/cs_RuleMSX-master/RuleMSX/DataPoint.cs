using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{
    public class DataPoint
    {

        private string name;
        private DataPointSource source;
        private DataSet dataSet;

        internal DataPoint(DataSet dataSet, string name)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "DataPoint constructor: " + name);
            this.name = name;
            this.dataSet = dataSet;
        }

        internal DataPoint(DataSet dataSet, string name, DataPointSource source)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "DataPoint constructor: " + name);
            this.name = name;
            this.dataSet = dataSet;
            this.SetDataPointSource(source);
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetDataPointSource(DataPointSource source)
        {
            source.setDataPoint(this);
            this.source = source;
        }

        public DataPointSource GetSource()
        {
            return this.source;
        }

        public DataSet GetDataSet()
        {
            return this.dataSet;
        }
    }
}