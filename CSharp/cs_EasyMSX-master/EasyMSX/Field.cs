using System.Collections.Generic;
using static com.bloomberg.emsx.samples.Notification;

namespace com.bloomberg.emsx.samples
{
    public class Field
    {

        private string _name;
        private string old_value;		// Used to store the previous value when a current value is set from BLP event
        private string current_value;	// Used to store the value last provided by an event - matches BLP
        private Fields parent;

        List<NotificationHandler> notificationHandlers = new List<NotificationHandler>();

        internal Field(Fields parent)
        {
            this.parent = parent;
        }

        internal Field(Fields parent, string name, string value)
        {
            this.parent = parent;
            this._name = name;
            this.old_value = null;
            this.current_value = value;
        }

        public string name()
        {
            return _name;
        }

        public string value()
        {
            return this.current_value;
        }

        internal void setName(string name)
        {
            this._name = name;
        }

        internal void setCurrentValue(string value)
        {

            if (value != this.current_value)
            {

                this.old_value = this.current_value;
                this.current_value = value;

                if (this.parent.owner is Order) {
                    this.notify(new Notification(NotificationCategory.ORDER, NotificationType.FIELD, this.parent.owner, new List<FieldChange> {this.getFieldChanged()}));
                } else if (this.parent.owner is Route) {
                    this.notify(new Notification(NotificationCategory.ROUTE, NotificationType.FIELD, this.parent.owner, new List<FieldChange> { this.getFieldChanged()}));
                }

            }
        }

        internal void CurrentToOld()
        {
            this.old_value = this.current_value;
        }

        internal FieldChange getFieldChanged()
        {

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

        public void addNotificationHandler(NotificationHandler notificationHandler)
        {
            notificationHandlers.Add(notificationHandler);
        }

        void notify(Notification notification)
        {

            foreach (NotificationHandler nh in notificationHandlers)
            {
                if (!notification.consume) nh.processNotification(notification);
            }
        }

    }
}
