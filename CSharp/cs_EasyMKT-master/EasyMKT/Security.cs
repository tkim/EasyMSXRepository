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
using System.Collections.Generic;
using static com.bloomberg.mktdata.samples.Notification;

namespace com.bloomberg.mktdata.samples
{

    public class Security : MessageHandler
    {

        private string name;
        private Securities securities;
        private Fields fields;

        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal Security(Securities securities)
        {
            this.securities = securities;
            this.fields = new Fields(this);
        }

        internal Security(Securities securities, string name)
        {
            this.securities = securities;
            this.name = name;
            this.fields = new Fields(this);
        }

        public string GetName()
        {
            return this.name;
        }

        public Securities GetSecurities()
        {
            return this.securities;
        }

        public Field field(string fieldName)
        {
            return fields.field(fieldName);
        }

        public void handleMessage(Message message)
        {
            this.fields.PopulateFields(message);
            if (fields.GetFieldChanges().Count > 0) this.sendNotifications(fields.GetFieldChanges());
        }

        public void addNotificationHandler(NotificationHandler notificationHandler)
        {
            notificationHandlers.Add(notificationHandler);
        }

        internal void sendNotifications(List<FieldChange> fcl)
        {
            if (this.notificationHandlers.Count > 0)
            {
                Notification n = new Notification(NotificationCategory.MKTDATA, NotificationType.UPDATE, this.fields.security, fcl);
                foreach (NotificationHandler nh in notificationHandlers)
                {
                    if (!n.consume) nh.ProcessNotification(n);
                }
                if (!n.consume) securities.ProcessNotification(n);
            }
        }
    }
}