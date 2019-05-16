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

using System.Collections.Generic;

namespace com.bloomberg.ioiapi.samples
{

    public class Notification
    {

        public enum NotificationCategory
        {
            IOIDATA,
            ADMIN
        }

        public enum NotificationType
        {
            NEW,
            REPLACE,
            CANCEL,
            ERROR
        }

        public NotificationCategory category;
        public NotificationType type;
        public bool consume = false;

        private IOI ioi;

        private int _errorCode;
        private string _errorMessage;

        internal Notification(NotificationCategory category, NotificationType type, IOI ioi)
        {
            this.category = category;
            this.type = type;
            this.ioi = ioi;
        }

        internal Notification(NotificationCategory category, NotificationType type, IOI ioi, int errorCode, string errorMessage)
        {
            this.category = category;
            this.type = type;
            this.ioi = ioi;
            this._errorCode = errorCode;
            this._errorMessage = errorMessage;
        }

        public IOI GetIOI()
        {
            return this.ioi;
        }

        public int errorCode()
        {
            return this._errorCode;
        }

        public string errorMessage()
        {
            return this._errorMessage;
        }
    }
}
