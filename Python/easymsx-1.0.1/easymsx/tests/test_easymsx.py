'''
Created on 21 Dec 2017

@author: RCLEGG2@BLOOMBERG.NET
'''

import unittest
import time
from easymsx import easymsx 
from easymsx.notification import Notification

class TestEasyMSX(unittest.TestCase):

    def process_notification(self,notification):

        if notification.category == Notification.NotificationCategory.ORDER:
            if notification.type == Notification.NotificationType.NEW or notification.type == Notification.NotificationType.INITIALPAINT: 
                print("EasyMSX Notification ORDER -> NEW/INIT_PAINT")
                #self.parse_order(notification.source)
        
        if notification.category == Notification.NotificationCategory.ROUTE:
            if notification.type == Notification.NotificationType.NEW or notification.type == Notification.NotificationType.INITIALPAINT: 
                print("EasyMSX Notification ROUTE -> NEW/INIT_PAINT")
                #self.parse_route(notification.source)

    def test_start_easymsx_does_not_fail(self):

        raised = False
        
        try:
            emsx = easymsx.EasyMSX()
            emsx.orders.add_notification_handler(self.process_notification)
            emsx.routes.add_notification_handler(self.process_notification)

            emsx.start()
            
        except BaseException as e:
            print("Error: " + str(e))
            raised=True
        
        self.assertFalse(raised)
        
    def test_external_request_non_blocking(self):
        
        raised = False

        try:
            emsx = easymsx.EasyMSX()
            emsx.start()
            
            req = emsx.create_request("CreateOrder")
            
            req.set("EMSX_TICKER", "IBM US Equity")
            req.set("EMSX_AMOUNT", 1000)
            req.set("EMSX_ORDER_TYPE", "MKT")
            req.set("EMSX_TIF", "DAY")
            req.set("EMSX_HAND_INSTRUCTION", "ANY")
            req.set("EMSX_SIDE", "BUY")
            
            self.pending_result=True
            
            emsx.send_request(req, self.message_handler)
            
        except BaseException as e:
            print("Error: " + str(e))
            raised=True
            
        while self.pending_result:
            pass
            
        self.assertFalse(raised)

    def message_handler(self,msg):
        print (msg)
        self.pending_result=False
        
        
    def test_external_request_blocking(self):
        
        raised = False

        try:
            emsx = easymsx.EasyMSX()
            emsx.start()
            
            req = emsx.create_request("CreateOrder")
            
            req.set("EMSX_TICKER", "IBM US Equity")
            req.set("EMSX_AMOUNT", 1000)
            req.set("EMSX_ORDER_TYPE", "MKT")
            req.set("EMSX_TIF", "DAY")
            req.set("EMSX_HAND_INSTRUCTION", "ANY")
            req.set("EMSX_SIDE", "BUY")
            
            msg = emsx.send_request(req)

            if msg.messageType()=="ErrorInfo":
                raised=True


        except BaseException as e:
            print("Error: " + str(e))
            raised=True
            
           
        print(msg)
        
        self.assertFalse(raised)

