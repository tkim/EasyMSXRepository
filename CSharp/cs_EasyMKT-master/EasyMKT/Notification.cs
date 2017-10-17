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

namespace com.bloomberg.mktdata.samples
{

    public class Notification
    {

        public enum NotificationCategory
        {
            MKTDATA,
            ADMIN
        }

        public enum NotificationType
        {
            NEW,
            UPDATE,
            FIELD,
            ERROR
        }

        public NotificationCategory category;
        public NotificationType type;
        public bool consume = false;

        private Security security;

        private List<FieldChange> fieldChanges;
        private int _errorCode;
        private string _errorMessage;

        internal Notification(NotificationCategory category, NotificationType type, Security security, List<FieldChange> fieldChanges)
        {
            this.category = category;
            this.type = type;
            this.security = security;
            this.fieldChanges = fieldChanges;
        }

        internal Notification(NotificationCategory category, NotificationType type, Security security, int errorCode, string errorMessage)
        {
            this.category = category;
            this.type = type;
            this.security = security;
            this._errorCode = errorCode;
            this._errorMessage = errorMessage;
        }

        public Security GetSecurity()
        {
            return this.security;
        }

        public List<FieldChange> GetFieldChanges()
        {
            return this.fieldChanges;
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
