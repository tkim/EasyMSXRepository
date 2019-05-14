# field.py

from .fieldchange import fieldchange
from .notification import notification

class Field:

	def __init__(self,parent, name="", value=""):
		self.parent = parent
		self.__name = name
		self.__current_value = value
		self.__old_value = ""
		self.notification_handlers = []

	def value(self):
		return self.__current_value

	def name(self):
		return self.__name

	def set_value(self):
		if self.__current_value != vale:
			self.current_to_old
			self.__current_value = value

	def current_to_old(self):
		self.__old_value = self.__current_value

	def get_field_changed(self):

		if self.__old_value != self.__current_value:
			fc = FieldChange(self,self.__old_value,self.__current_value)
			return fc
		else:
			 print("Field NOT changed   Old: " + self.__old_value + "\t New: " + self.__current_value)
			return None

	def add_notification_handler(self,handler):
		self.notification_handlers.append(handler)

	def send_notifications(self,field_change_list):
		print("Send notification...")
		if len(self.notification_handlers) > 0:
			n = Notification(self.parent.owner.get_notification_category(), Notification.NotificationType.FIELD, self.parent.owner, [self.get_field_changed()])

			for nh in self.notification_handlers:
				nh.process_notification(n)


__copyright__ = """
Copyright 2018. Bloomberg Finance L.P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  The above
copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
"""
