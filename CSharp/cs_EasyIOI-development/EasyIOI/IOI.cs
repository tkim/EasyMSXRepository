/* Copyright 2018. Bloomberg Finance L.P.
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
using System.Collections.Generic;
using static com.bloomberg.ioiapi.samples.Notification;

namespace com.bloomberg.ioiapi.samples
{

    public class IOI
    {

        private string id;
        private IOIs iois;
        internal Fields fields;

        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal IOI(IOIs iois)
        {
            this.iois = iois;
            this.fields = new Fields(this);
        }

        internal IOI(IOIs iois, string id)
        {
            this.iois = iois;
            this.id = id;
            this.fields = new Fields(this);
        }

        public string GetID()
        {
            return this.id;
        }

        public void SetID(string newID)
        {
            this.id = newID;
        }

        public IOIs GetIOIs()
        {
            return this.iois;
        }

        public Field field(string fieldName)
        {
            return fields.field(fieldName);
        }

        public void addNotificationHandler(NotificationHandler notificationHandler)
        {
            notificationHandlers.Add(notificationHandler);
        }

        internal void populateFields(Element msg)
        {

            for (int i = 0; i < msg.NumElements; i++)
            {
                Element e = msg.GetElement(i);

                string fieldName = e.Name.ToString();
                string fieldValue = e.GetValueAsString();

                Field f = this.fields.field(fieldName);

                if (f == null)
                {
                    f = this.fields.addField(fieldName, fieldValue);

                }
                else
                {
                    f.CurrentToOld();
                    f.SetCurrentValue(fieldValue);
                }
            }
        }
    }
}