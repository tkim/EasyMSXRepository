using System;
using System.Collections;
using System.Collections.Generic;
using Message = Bloomberglp.Blpapi.Message;
using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using Element = Bloomberglp.Blpapi.Element;

namespace com.bloomberg.emsx.samples {

    public class Fields : IEnumerable<Field> {
	
	    private List<Field> fields = new List<Field>();
	
	    internal FieldsOwner owner;
	
	    private List<FieldChange> fieldChanges;
	
	    internal Fields(FieldsOwner owner) {
		    this.owner = owner;
		    loadFields(owner);
	    }
	
	    internal void loadFields(FieldsOwner owner) {
		
		    if(owner is Order) {
			    Order o = (Order)owner;
			    foreach(SchemaFieldDefinition sdf in o.parent.emsxapi.orderFields) {
				    Field f = new Field(this,sdf.name,"");
				    fields.Add(f);
			    }
		    } else if(owner is Route) {
			    Route r = (Route)owner;
			    foreach(SchemaFieldDefinition sdf in r.parent.emsxapi.routeFields) {
				    Field f = new Field(this,sdf.name,"");
				    fields.Add(f);
			    }
		    } 
	    }
	
	    internal void populateFields(Message message, bool dynamicFieldsOnly) {

		    Log.LogMessage(LogLevels.BASIC, "Populate fields");
		
		    CurrentToOldValues();
		
		    int fieldCount = message.NumElements;
		
		    Element e = message.AsElement;
		
		    fieldChanges = new List<FieldChange>();
		
		    for(int i=0; i<fieldCount; i++) {

			    Boolean load=true;
			
			    Element f = e.GetElement(i);
			
			    String fieldName = f.Name.ToString();
			    // Workaround for schema field nameing
			    if(fieldName.Equals("EMSX_ORD_REF_ID")) fieldName = "EMSX_ORDER_REF_ID";
			
			    if(dynamicFieldsOnly) {
				    SchemaFieldDefinition sfd = null;
				    if(owner is Order) {
					    Order o = (Order)owner;
					    sfd = findSchemaFieldByName(fieldName,o.parent.emsxapi.orderFields);
				    } else if(owner is Route) {
					    Route r = (Route)owner;
					    sfd = findSchemaFieldByName(fieldName,r.parent.emsxapi.routeFields);
				    }
				    if(sfd!=null && sfd.isStatic()) {
					    load=false;
				    }
			    }
			
			    if(load) {
				    Field fd = field(fieldName);
				
				    if(fd==null) fd = new Field(this);
				
				    fd.setName(fieldName);
				    // set the CURRENT value NOT the new_value. new_value is only set by client side.
				    fd.setCurrentValue(f.GetValueAsString()); 
			
				    FieldChange fc = fd.getFieldChanged();
				    if(fc!=null) {
					    fieldChanges.Add(fc);
				    }
			    }
		    }

	    }
	
	    private SchemaFieldDefinition findSchemaFieldByName(String name, List<SchemaFieldDefinition>fields) {

		    foreach(SchemaFieldDefinition sfd in fields) {
			    if(sfd.name==name) {
				    return sfd;
			    }
		    }
		    return null;
	    }
	
	    internal List<FieldChange> getFieldChanges() {
		    return this.fieldChanges;
	    }
	
	    internal void CurrentToOldValues() {
	
		    foreach(Field f in fields) {
			    f.CurrentToOld();
		    }
	    }
	
	    public Field field(String name) {
		    foreach(Field f in fields) {
			    if(f.name().Equals(name)) {
				    return f;
			    }
		    }
		    return null;
	    }

        public IEnumerator<Field> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}