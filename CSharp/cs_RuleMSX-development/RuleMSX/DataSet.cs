/* Copyright 2017. Bloomberg Finance L.P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  The above
copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{

    public class DataSet
    {

        private string name;
        private Dictionary<string, DataPoint> dataPoints;

        internal DataSet(string name)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "DataSet constructor: " + name);
            this.name = name;
            this.dataPoints = new Dictionary<string, DataPoint>();
        }

        public DataPoint AddDataPoint(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Adding DataPoint: " + name + " to DataSet: " + this.name);
            if (name == null || name == "") throw new ArgumentException("DataPoint name cannot be null or empty");
            DataPoint newDataPoint = new DataPoint(this, name);
            dataPoints.Add(name, newDataPoint);
            return newDataPoint;
        }

        public DataPoint AddDataPoint(string name, DataPointSource source)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Adding DataPoint: " + name + " to DataSet: " + this.name);
            if (name == null || name == "") throw new ArgumentException("DataPoint name cannot be null or empty");
            DataPoint newDataPoint = new DataPoint(this, name, source);
            dataPoints.Add(name, newDataPoint);
            return newDataPoint;
        }

        public string GetName()
        {
            return this.name;
        }

        public DataPoint GetDataPoint(string name)
        {
            try {
                return dataPoints[name];
            } catch (Exception)
            {
                return null;
            }
        }

        public Dictionary<string, DataPoint> GetDataPoints()
        {
            return this.dataPoints;
        }
    }
}