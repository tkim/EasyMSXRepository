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

namespace com.bloomberg.mktdata.samples
{

    public class Securities : IEnumerable<Security>, NotificationHandler
    {

        internal EasyMKT easyMKT;

        private List<Security> securities = new List<Security>();
        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal Securities(EasyMKT easyMKT)
        {
            this.easyMKT = easyMKT;
        }

        public IEnumerator<Security> GetEnumerator()
        {
            return securities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return securities.GetEnumerator();
        }


        public Security Get(int index)
        {
            return securities[index];
        }

        public Security Get(string name)
        {
            foreach (Security s in securities)
            {
                if (s.GetName().Equals(name)) return s;
            }
            return null;
        }

        internal Security createSecurity(String ticker)
        {
            Log.LogMessage(LogLevels.DETAILED, "Adding new security: " + ticker);
            Security newSecurity = new Security(this, ticker);
            securities.Add(newSecurity);
            Log.LogMessage(LogLevels.DETAILED, "Added new security: " + newSecurity.GetName());
            return newSecurity;
        }

        public void addNotificationHandler(NotificationHandler notificationHandler)
        {
            notificationHandlers.Add(notificationHandler);
        }

        public void ProcessNotification(Notification notification)
        {
            foreach (NotificationHandler nh in notificationHandlers)
            {
                if (!notification.consume) nh.ProcessNotification(notification);
            }
        }

    }
}
