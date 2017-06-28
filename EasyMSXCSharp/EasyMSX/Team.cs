using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;

namespace com.bloomberg.emsx.samples {

    public class Team {
	
	    public string name;
	    Teams parent;
	
	    internal Team(Teams parent, string name) {
		    this.name = name;
		    this.parent = parent;
	    }

	    public void select() {
		    parent.emsxapi.setTeam(this);
		    Log.LogMessage(LogLevels.BASIC, "Team selected: " + name);
	    }

    }
}