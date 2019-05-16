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

using System;
using System.Collections;
using System.Collections.Generic;
using Bloomberglp.Blpapi;
using static com.bloomberg.ioiapi.samples.Log;

namespace com.bloomberg.ioiapi.samples
{

    public class IOIs : IEnumerable<IOI>, MessageHandler
    {

        internal EasyIOI easyIOI;

        private List<IOI> iois = new List<IOI>();

        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal IOIs(EasyIOI easyIOI)
        {
            this.easyIOI = easyIOI;
        }

        public IEnumerator<IOI> GetEnumerator()
        {
            return iois.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return iois.GetEnumerator();
        }


        public IOI Get(int index)
        {
            return iois[index];
        }

        public IOI Get(string id)
        {
            foreach (IOI ioi in iois)
            {
                if (ioi.GetID().Equals(id)) return ioi;
            }
            return null;
        }

        public void addNotificationHandler(NotificationHandler notificationHandler)
        {
            notificationHandlers.Add(notificationHandler);
        }

        public void handleMessage(Message message)
        {

            Element msg = message.AsElement;

            // Extract originalId_value from IOI message. For NEW, original and id_value will
            // be the same. For REPLACE or CANCEL, these values differ.

            String original = msg.GetElementAsString("originalId_value");
            String change = msg.GetElementAsString("change");

            IOI ioi = null;

            ioi = this.Get(original);

            if (ioi == null)
            {
                // unknown incoming IOI. Create the ioi object and set the id from id_value
                ioi = new IOI(this, original);
                iois.Add(ioi);
            }

            ioi.populateFields(msg);

            Notification.NotificationType nt = Notification.NotificationType.NEW;
            if(change=="REPLACE") nt = Notification.NotificationType.REPLACE;
            else if (change == "CANCEL") nt = Notification.NotificationType.CANCEL;

            this.notify(ioi, nt);
        }

        internal void notify(IOI ioi, Notification.NotificationType nt)
        {
            foreach(NotificationHandler nh in this.notificationHandlers)
            {
                nh.ProcessNotification(new Notification(Notification.NotificationCategory.IOIDATA, nt, ioi));
            }
        }
    }
}
