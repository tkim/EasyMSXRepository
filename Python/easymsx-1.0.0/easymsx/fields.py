# fields.py

from .field import Field

class Fields:
    
    def __init__(self,owner):
        self.owner = owner
        self.fields=[]
        self.field_changes=[]
        
        self.load_fields()
        
    def load_fields(self):

        for sdf in self.owner.parent.field_source:
            f = Field(self,sdf.name,"")
            self.fields.append(f)

    def populate_fields(self,msg,dynamic_fields_only):
        
        self.current_to_old_values()

        field_count = msg.numElements()

        self.field_changes = []
        
        for i in range(0, field_count):
            load = True
            f = msg.getElement(i)
            field_name = str(f.name())
            
            if field_name == "EMSX_ORD_REF_ID":
                field_name = "EMSX_ORDER_REF_ID"
            
            if dynamic_fields_only:
                fld = None
                for sdf in self.owner.parent.field_source:
                    if sdf.name==field_name:
                        fld = sdf
                
                if (not fld is None) and fld.is_static():
                    load=False
                    
            if load:
                fd = self.field(field_name)
                if fd == None:
                    fd = Field(self,field_name)
                
                fd.set_value(f.getValueAsString())

#                print("loaded field: " + fd.name() + "\tValue: " + fd.value())
                
                fc = fd.get_field_changed()
                if fc is not None:
#                    print("Added field to fieldChanges list")
                    self.field_changes.append(fc)
                    
        
    def current_to_old_values(self):
        for f in self.fields:
            f.current_to_old()
    
            
    def field(self,name):
        for f in self.fields:
            if f.name() == name:
                return f
        
        return None
    
    def get_field_changes(self):
        return self.field_changes
    

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
