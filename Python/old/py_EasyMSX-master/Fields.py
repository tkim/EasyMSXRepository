# Fields.py

from Field import Field

class Fields:
    
    def __init__(self,owner):
        self.owner = owner
        self.fields=[]
        self.fieldChanges=[]
        
        self.loadFields()
        
    def loadFields(self):

        for sdf in self.owner.parent.fieldSource:
            f = Field(self,sdf.name,"")
            self.fields.append(f)

    def populateFields(self,msg,dynamicFieldsOnly):
        
        self.currentToOldValues()

        fieldCount = msg.numElements()

        self.fieldChanges = []
        
        for i in range(0, fieldCount):
            load = True
            f = msg.getElement(i)
            fieldName = str(f.name())
            
            if fieldName == "EMSX_ORD_REF_ID":
                fieldName = "EMSX_ORDER_REF_ID"
            
            if dynamicFieldsOnly:
                fld = None
                for sdf in self.owner.parent.fieldSource:
                    if sdf.name==fieldName:
                        fld = sdf
                
                if (not fld is None) and fld.isStatic():
                    load=False
                    
            if load:
                fd = self.field(fieldName)
                if fd == None:
                    fd = Field(self,fieldName)
                
                fd.setValue(f.getValueAsString())

#                print("loaded field: " + fd.name() + "\tValue: " + fd.value())
                
                fc = fd.getFieldChanged()
                if fc is not None:
#                    print("Added field to fieldChanges list")
                    self.fieldChanges.append(fc)
                    
        
    def currentToOldValues(self):
        for f in self.fields:
            f.currentToOld()
    
            
    def field(self,name):
        for f in self.fields:
            if f.name() == name:
                return f
        
        return None
    
    def getFieldChanges(self):
        return self.fieldChanges
    