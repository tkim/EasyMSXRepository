/* Copyright 2017. Bloomberg Finance L.P.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:  The above
 * copyright notice and this permission notice shall be included in all copies
 * or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using static com.bloomberg.mktdata.samples.Log;

namespace com.bloomberg.mktdata.samples {

    public class SubscriptionFields : IEnumerable<SubscriptionField> {

        EasyMKT easyMKT;

        private List<SubscriptionField> subscriptionFields = new List<SubscriptionField>();

        internal SubscriptionFields(EasyMKT easyMKT) {
            this.easyMKT = easyMKT;
        }

        private void updateSubscriptions() {
            // When a field is added, we cancel the existing subscriptions and re-subscribe to add the new field.
            // Ideally, there should be no subscriptions until all the fields have been added.
        }

        public IEnumerator<SubscriptionField> GetEnumerator() {
            return subscriptionFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return subscriptionFields.GetEnumerator();
        }

        public SubscriptionField Get(int index)
        {
            return subscriptionFields[index];
        }

        public SubscriptionField Get(string name)
        {
            foreach (SubscriptionField sf in subscriptionFields) {
                if (sf.GetName().Equals(name)) return sf;
            }
            return null;
        }
        
        internal SubscriptionField CreateSubscriptionField(string fieldName)
        {
            Log.LogMessage(LogLevels.DETAILED, "Adding new subscription field: " + fieldName);
            SubscriptionField newField = new SubscriptionField(this, fieldName);
            subscriptionFields.Add(newField);
            updateSubscriptions();
            Log.LogMessage(LogLevels.DETAILED, "Added new subscription field: " + newField.GetName());
            return newField;
        }

        public string GetFieldList() {

            string list = "";

            foreach (SubscriptionField f in subscriptionFields) {
                list = list + f.GetName() + ",";
            }

            if (list.Length > 0) list = list.Substring(0, list.Length - 1);
            return list;
        }
    }
}