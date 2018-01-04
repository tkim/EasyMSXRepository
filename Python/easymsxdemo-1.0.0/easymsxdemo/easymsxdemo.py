from easymsx import easymsx 
from easymsx.notification import Notification

class EasyMSXDemo():
    
    def __init__(self):
        
        print("Initializing EasyMSX...")
        self.emsx = easymsx.EasyMSX()
        print("...done initializing.\n")
        
        print("Teams: ")
        for t in self.emsx.teams:
            print("\t"+t.name)

        print("\nBrokers: ")
        for b in self.emsx.brokers:
            print("\t"+b.name)

        self.emsx.orders.add_notification_handler(self.process_order_notification)
        self.emsx.routes.add_notification_handler(self.process_route_notification)
        
        self.emsx.start()
        
    def print_order_blotter(self):
        print("\nOrder Blotter: \n")
        for o in self.emsx.orders:
            print("Side: " + o.field("EMSX_SIDE").value() + "\tSequence No.: " + o.field("EMSX_SEQUENCE").value() + "\tStatus: " + o.field("EMSX_STATUS").value() + " \tTicker: " + o.field("EMSX_TICKER").value() + "\tAmount: " + o.field("EMSX_AMOUNT").value() + "\tTIF: " + o.field("EMSX_TIF").value())
                        
    def print_route_blotter(self):
        print("\nRoute Blotter: \n")
        for r in self.emsx.routes:
            print("Sequence No.: " + r.field("EMSX_SEQUENCE").value() + "\tRoute ID: " + r.field("EMSX_ROUTE_ID").value() + "\tStatus: " + r.field("EMSX_STATUS").value() + "  \tWorking: " + r.field("EMSX_WORKING").value() + "\tFilled: " + r.field("EMSX_FILLED").value() + "\tAverage Price: " + r.field("EMSX_AVG_PRICE").value())

    def print_field_changes(self,field_changes):
        for fc in field_changes:
            print("Field: " + fc.field.name() + "\tOld: " + fc.old_value + "\tNew: " + fc.new_value)
            
    def process_order_notification(self,notification):
        print("\nChange to Order (" + notification.type.name + "): " + notification.source.field("EMSX_SEQUENCE").value())
        self.print_field_changes(notification.field_changes)
        self.print_order_blotter()

    def process_route_notification(self,notification):
        print("\nChange to Route (" + notification.type.name + "): " + notification.source.field("EMSX_SEQUENCE").value() + "/" + notification.source.field("EMSX_ROUTE_ID").value())
        self.print_field_changes(notification.field_changes)
        self.print_route_blotter()

    def stop(self):
        self.emsx.stop()
        
        
if __name__ == '__main__':
    
    easymsxdemo = EasyMSXDemo();
    
    input("Press any to terminate\n")

    print("Terminating...\n")

    easymsxdemo.stop()
    
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