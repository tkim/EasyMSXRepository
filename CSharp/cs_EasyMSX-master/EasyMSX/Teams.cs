
using System;
using System.Collections;
using System.Collections.Generic;

using LogLevels = com.bloomberg.emsx.samples.Log.LogLevels;
using Element =  Bloomberglp.Blpapi.Element;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;

namespace com.bloomberg.emsx.samples {

    public class Teams : IEnumerable<Team> {

        private static readonly Name    GET_TEAMS = new Name("GetTeams");
	    private static readonly Name 	ERROR_INFO = new Name("ErrorInfo");

	    private List<Team> teams = new List<Team>(); 
        internal EasyMSX emsxapi;
    
	    internal Teams(EasyMSX emsxapi) {
		    this.emsxapi = emsxapi;
		    loadTeams();
	    }
	
	    private void loadTeams() {
		    Log.LogMessage(LogLevels.BASIC, "Teams: Loading");
		    Request request = emsxapi.emsxService.CreateRequest(GET_TEAMS.ToString());
            emsxapi.submitRequest(request,new TeamHandler(this));
	    }

	    class TeamHandler : MessageHandler {
		
		    Teams teams;
		
		    internal TeamHandler(Teams teams) {
			    this.teams = teams;
		    }
		
		    public void processMessage(Message message) {
			
			    Log.LogMessage(LogLevels.BASIC, "Teams: processing message");
			
	    	    if(message.MessageType.Equals(ERROR_INFO)) {
	        	    Log.LogMessage(LogLevels.BASIC, "Teams: processing RESPONSE error");
	    		    int errorCode = message.GetElementAsInt32("ERROR_CODE");
	    		    String errorMessage = message.GetElementAsString("ERROR_MESSAGE");
	    		    Log.LogMessage(LogLevels.BASIC, "Error getting teams: [" + errorCode + "] " + errorMessage);
	    	    } else if(message.MessageType.Equals(GET_TEAMS)) {
	        	    Log.LogMessage(LogLevels.BASIC, "Teams: processing successful RESPONSE");
	
	        	    Element teamList = message.GetElement("TEAMS");
	    		
				    int numValues = teamList.NumValues;
	    		
	    		    for(int i = 0; i < numValues; i++) {
	    			    String teamName = teamList.GetValueAsString(i);
	    			    Team newTeam = new Team(teams,teamName);
	    			    teams.add(newTeam);
	            	    Log.LogMessage(LogLevels.DETAILED, "Teams: Added new team " + newTeam.name);
	    		    }
	    	    }
		    }
	    }

        public IEnumerator<Team> GetEnumerator()
        {
            return teams.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
	
	    public Team get(int index) {
		    return teams[index];
	    }
	
	    public Team get(String name) {
		    foreach(Team t in teams) {
			    if(t.name.Equals(name)) return t;
		    }
		    return null;
	    }

	    private void add(Team newTeam) {
		    teams.Add(newTeam);
	    }
    }
}
