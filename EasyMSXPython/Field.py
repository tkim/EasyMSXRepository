# Field.py

from FieldChange import FieldChange

class Field:
    
    def __init__(self,parent, name="", value=""):
        self.parent = parent
        self.__name = name
        self.__current_value = value
        self.__old_value = ""
        
    def value(self):
        return self.__current_value
    
    def name(self):
        return self.__name
    
    def setValue(self,value):
        if self.__current_value != value:
            self.currentToOld
            self.__current_value = value
            
    def currentToOld(self):
        self.__old_value = self.__current_value
        
    def getFieldChanged(self):
        
        if self.__old_value != self.__current_value:
            fc = FieldChange(self,self.__old_value,self.__current_value)
            return fc
        else:
            ##print "Field NOT changed   Old: " + self.__old_value + "\t New: " + self.__current_value 
            return None