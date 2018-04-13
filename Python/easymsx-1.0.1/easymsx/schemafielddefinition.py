# schemafielddefinition.py

class SchemaFieldDefinition:
    
    def __init__(self,name):
        self.name=name
        self.status=""
        self.type=""
        self.min=0
        self.max=0
        self.description=""
        
    def is_static(self):
        return (self.description.find("Static") > -1)
   
    def is_order_field(self):
        return ((self.description.find("Order") > -1) or (self.description.find("O,R") > -1))
    
    def is_route_field(self):
        return ((self.description.find("Route") > -1) or (self.description.find("O,R") > -1))
    
    def is_special_field(self):
        return (self.description.find("Special") > -1)
    

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
