from easymkt import easymkt 
from easymkt.notification import Notification

class EasyMKTDemo():
    
    def __init__(self):
        
        self.emkt = easymkt.EasyMKT()
        
        print("Setting up field list...")
        
        self.emkt.add_field("TICKER")
        self.emkt.add_field("LAST_PRICE")
        self.emkt.add_field("MID")
        self.emkt.add_field("BID")
        self.emkt.add_field("ASK")
        
        print("Field List: ")
        
        for f in self.emkt.subscription_fields:
            print("\t" + f)
        
        
        print("Setting up security universe...")
        
        self.emkt.add_security("BBHBEAT Index")
        self.emkt.add_security("VOD LN Equity")
        self.emkt.add_security("IBM US Equity")

        print("Security universe: ")
        
        for s in self.emkt.securities:
            print("\t"+ s.name)

        self.emkt.add_notification_handler(self.process_notification)

        self.emkt.start()

    def process_notification(self,notification):

        if notification.category == Notification.NotificationCategory.MKTDATA:
            print("Event: " + Notification.NotificationType.as_text(notification.type) + "\tSource: " + notification.source.name)
            for fc in notification.field_changes:
                print("\tField: " + fc.field.name() + "\t " + fc.old_value + " -> " + fc.new_value)    


    def stop(self):
        self.emkt.stop()
        
if __name__ == '__main__':
    
    easymktdemo = EasyMKTDemo();
    
    input("Press any to terminate\n")

    print("Terminating...\n")

    easymktdemo.stop()
    
    quit()
    
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