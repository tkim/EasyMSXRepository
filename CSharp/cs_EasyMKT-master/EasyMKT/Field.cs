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

namespace com.bloomberg.mktdata.samples {

    public class Field {

        private string _name;
        private string old_value;
        private string current_value;
        private Fields fields;

        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal Field(Fields fields) {
            this.fields = fields;
        }

        internal Field(Fields fields, string name, string value) {
            this.fields = fields;
            this._name = name;
            this.old_value = null;
            this.current_value = value;
        }

        public string Name() {
            return _name;
        }

        public string Value() {
            return this.current_value;
        }

        internal void SetName(string name) {
            this._name = name;
        }

        internal void SetCurrentValue(string value) {
            this.current_value = value;
        }

        internal void CurrentToOld() {
            this.old_value = this.current_value;
        }

        internal FieldChange GetFieldChanged(){

            FieldChange fc = null;

            if (!this.current_value.Equals(this.old_value))
            {
                fc = new FieldChange();
                fc.field = this;
                fc.oldValue = this.old_value;
                fc.newValue = this.current_value;
            }
            return fc;
        }

        public void AddNotificationHandler(NotificationHandler notificationHandler) {
            notificationHandlers.Add(notificationHandler);
        }

        internal void sendNotifications(List<FieldChange> fcl) {
            if (this.notificationHandlers.Count > 0)
            {
                Notification n = new Notification(Notification.NotificationCategory.MKTDATA, Notification.NotificationType.FIELD, this.fields.security, fcl);
                foreach (NotificationHandler nh in notificationHandlers) {
                    if (!n.consume) nh.ProcessNotification(n);
                }
            }
        }
    }
}