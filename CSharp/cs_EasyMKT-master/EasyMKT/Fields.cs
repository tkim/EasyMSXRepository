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

using Bloomberglp.Blpapi;
using System;
using System.Collections;
using System.Collections.Generic;
using static com.bloomberg.mktdata.samples.Log;

namespace com.bloomberg.mktdata.samples {

    public class Fields : IEnumerable<Field> {

        private List<Field> fields = new List<Field>();

        internal Security security;
        List<FieldChange> fieldChanges;

        internal Fields(Security security) {
            this.security = security;
            LoadFields();
        }

        private void LoadFields() {

            foreach (SubscriptionField sf in security.GetSecurities().easyMKT.fields) {
                Field newField = new Field(this, sf.GetName(), "");
                this.fields.Add(newField);
            }
        }

        internal void PopulateFields(Message message) {

            Log.LogMessage(LogLevels.DETAILED, "Populate fields");
            Log.LogMessage(LogLevels.DETAILED, "Source: " + message.ToString());

            CurrentToOldValues();

            int fieldCount = message.NumElements;

            Element e = message.AsElement;

            for (int i = 0; i < fieldCount; i++) {

                Element f = e.GetElement(i);

                String fieldName = f.Name.ToString();

                Field fd = field(fieldName);

                if (fd == null) fd = new Field(this);

                fd.SetName(fieldName);

                if (!f.IsNull) fd.SetCurrentValue(f.GetValueAsString());
                else fd.SetCurrentValue("");

                Log.LogMessage(LogLevels.DETAILED, "Setting field: " + fd.Name() + "\tvalue: " + fd.Value());
            }

            fieldChanges = new List<FieldChange>();

            foreach (Field f in fields) {

                FieldChange fc = f.GetFieldChanged();

                if (fc != null) {
                    fieldChanges.Add(fc);
                    f.sendNotifications(new List<FieldChange>{ fc });
                }
            }
        }

        internal List<FieldChange> GetFieldChanges() {
            return this.fieldChanges;
        }

        internal void CurrentToOldValues() {

            foreach (Field f in fields) {
                f.CurrentToOld();
            }
        }

        public Field field(String name) {
            foreach (Field f in fields) {
                if (f.Name().Equals(name)) return f;
            }
            return null;
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }
    }
}