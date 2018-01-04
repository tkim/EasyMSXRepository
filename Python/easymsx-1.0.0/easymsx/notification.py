# notification.py
from enum import Enum

class Notification:

    class NotificationCategory(Enum):
        ORDER=0
        ROUTE=1
        ADMIN=2
        
    class NotificationType(Enum):
        NEW=0
        INITIALPAINT=1
        UPDATE=2
        DELETE=3
        CANCEL=4
        ERROR=5
        FIELD=6

    def __init__(self,notification_category,notification_type,notification_source,field_changes=[],error_code=0,error_message=""):
        self.category = notification_category
        self.type = notification_type
        self.source = notification_source
        self.field_changes = field_changes
        self.error_code = error_code
        self.error_message = error_message
        self.consumed=False


__copyright__ = """
Copyright 2017. Bloomberg Finance L.P.

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
