using System;
using com.bloomberg.mktdata.samples;

namespace com.bloomberg.test { 

    public class EasyMKTSample : NotificationHandler {

        EasyMKT emkt;

        static void Main(string[] args) {
            System.Console.WriteLine("Bloomberg - EasyMKT Example - EasyMKTAPISample");

            EasyMKTSample example = new EasyMKTSample();
            example.run(args);

            System.Console.WriteLine("Press Enter to terminate...");
            System.Console.ReadLine();
            System.Console.WriteLine("Terminating.");
        }

	    private void run(String[] args) {

            Log.logLevel = Log.LogLevels.BASIC;

            System.Console.WriteLine("Initializing EasyMKT ");

            // Uncomment the appropriate constructor call
            emkt = new EasyMKT();
            //emkt = new EasyMKT("1.2.3.4",8194);

            System.Console.WriteLine("EasyMKT initialized OK");

            // Add required fields...
            emkt.AddField("BID");
		    emkt.AddField("ASK");
		    emkt.AddField("MID");
		    emkt.AddField("LAST_PRICE");

		    // Add required securities...
		    emkt.AddSecurity("HSBA LN Equity");
		    emkt.AddSecurity("BATS LN Equity");
		    emkt.AddSecurity("RDSA LN Equity");
		    emkt.AddSecurity("BP/ LN Equity");
		    emkt.AddSecurity("RDSB LN Equity");
		    emkt.AddSecurity("GSK LN Equity");
		    emkt.AddSecurity("DGE LN Equity");
		    emkt.AddSecurity("VOD LN Equity");
		    emkt.AddSecurity("AZN LN Equity");
		    emkt.AddSecurity("ULVR LN Equity");
		    emkt.AddSecurity("PRU LN Equity");
		    emkt.AddSecurity("LLOY LN Equity");
		    emkt.AddSecurity("RIO LN Equity");
		    emkt.AddSecurity("RB/ LN Equity");
		    emkt.AddSecurity("GLEN LN Equity");
		    emkt.AddSecurity("SHP LN Equity");
		    emkt.AddSecurity("NG/ LN Equity");
		    emkt.AddSecurity("BARC LN Equity");
		    emkt.AddSecurity("BLT LN Equity");
		    emkt.AddSecurity("IMB LN Equity");
		    emkt.AddSecurity("CPG LN Equity");
		    emkt.AddSecurity("BT/A LN Equity");
		    emkt.AddSecurity("CRH LN Equity");
		    emkt.AddSecurity("AV/ LN Equity");
		    emkt.AddSecurity("STAN LN Equity");
		    emkt.AddSecurity("BA/ LN Equity");
		    emkt.AddSecurity("REL LN Equity");
		    emkt.AddSecurity("WPP LN Equity");
		    emkt.AddSecurity("AAL LN Equity");
		    emkt.AddSecurity("LGEN LN Equity");
		    emkt.AddSecurity("TSCO LN Equity");
		    emkt.AddSecurity("RR/ LN Equity");
		    emkt.AddSecurity("SSE LN Equity");
		    emkt.AddSecurity("EXPN LN Equity");
		    emkt.AddSecurity("LSE LN Equity");
		    emkt.AddSecurity("SN/ LN Equity");
		    emkt.AddSecurity("FERG LN Equity");
		    emkt.AddSecurity("SLA LN Equity");
		    emkt.AddSecurity("ABF LN Equity");
		    emkt.AddSecurity("CNA LN Equity");
		    emkt.AddSecurity("IAG LN Equity");
		    emkt.AddSecurity("SKY LN Equity");
		    emkt.AddSecurity("OML LN Equity");
		    emkt.AddSecurity("CCL LN Equity");
		    emkt.AddSecurity("III LN Equity");
		    emkt.AddSecurity("AHT LN Equity");
		    emkt.AddSecurity("ITRK LN Equity");
		    emkt.AddSecurity("PSN LN Equity");
		    emkt.AddSecurity("RBS LN Equity");
		    emkt.AddSecurity("BRBY LN Equity");
		    emkt.AddSecurity("WPG LN Equity");
		    emkt.AddSecurity("MNDI LN Equity");
		    emkt.AddSecurity("BNZL LN Equity");
		    emkt.AddSecurity("LAND LN Equity");
		    emkt.AddSecurity("SGE LN Equity");
		    emkt.AddSecurity("RRS LN Equity");
		    emkt.AddSecurity("IHG LN Equity");
		    emkt.AddSecurity("WTB LN Equity");
		    emkt.AddSecurity("RSA LN Equity");
		    emkt.AddSecurity("KGF LN Equity");
		    emkt.AddSecurity("DCC LN Equity");
		    emkt.AddSecurity("TW/ LN Equity");
		    emkt.AddSecurity("BLND LN Equity");
		    emkt.AddSecurity("BDEV LN Equity");
		    emkt.AddSecurity("UU/ LN Equity");
		    emkt.AddSecurity("SMIN LN Equity");
		    emkt.AddSecurity("STJ LN Equity");
		    emkt.AddSecurity("NXT LN Equity");
		    emkt.AddSecurity("SMT LN Equity");
		    emkt.AddSecurity("ITV LN Equity");
		    emkt.AddSecurity("PPB LN Equity");
		    emkt.AddSecurity("TUI LN Equity");
		    emkt.AddSecurity("INF LN Equity");
		    emkt.AddSecurity("SKG LN Equity");
		    emkt.AddSecurity("RTO LN Equity");
		    emkt.AddSecurity("GKN LN Equity");
		    emkt.AddSecurity("SGRO LN Equity");
		    emkt.AddSecurity("JMAT LN Equity");
		    emkt.AddSecurity("SVT LN Equity");
		    emkt.AddSecurity("MRW LN Equity");
		    emkt.AddSecurity("MKS LN Equity");
		    emkt.AddSecurity("DLG LN Equity");
		    emkt.AddSecurity("CCH LN Equity");
		    emkt.AddSecurity("PSON LN Equity");
		    emkt.AddSecurity("CRDA LN Equity");
		    emkt.AddSecurity("MCRO LN Equity");
		    emkt.AddSecurity("HMSO LN Equity");
		    emkt.AddSecurity("GFS LN Equity");
		    emkt.AddSecurity("BAB LN Equity");
		    emkt.AddSecurity("ADM LN Equity");
		    emkt.AddSecurity("SBRY LN Equity");
		    emkt.AddSecurity("SDR LN Equity");
		    emkt.AddSecurity("CTEC LN Equity");
		    emkt.AddSecurity("ANTO LN Equity");
		    emkt.AddSecurity("RMG LN Equity");
		    emkt.AddSecurity("HL/ LN Equity");
		    emkt.AddSecurity("MERL LN Equity");
		    emkt.AddSecurity("FRES LN Equity");
		    emkt.AddSecurity("MDC LN Equity");
		    emkt.AddSecurity("EZJ LN Equity");
		    emkt.AddSecurity("PFG LN Equity");

		    // Notify me if anything changes for any security
		    //emkt.securities.addNotificationHandler(this);
		
		    // Notify me if anything changes for a specific security
		    //emkt.securities.Get("HSBA LN Equity").addNotificationHandler(this);

		    // Notify me if anything changes for specific field on specific security.
		    //emkt.securities.Get("HSBA LN Equity").field("ASK").AddNotificationHandler(this);
		    //emkt.securities.Get("HSBA LN Equity").field("BID").AddNotificationHandler(this);

		    // Add specific field handlers for every loaded security
		    foreach (Security s in emkt.securities) {
			    s.field("ASK").AddNotificationHandler(this);
                s.field("BID").AddNotificationHandler(this);
		    }
		
		    emkt.start();
	    }

        public void ProcessNotification(Notification notification) {
            System.Console.WriteLine("Notification: Type=" + notification.type.ToString() + " Security=" + notification.GetSecurity().GetName());
            foreach (FieldChange fc in notification.GetFieldChanges()) {
                System.Console.WriteLine("\tField change: name = " + fc.field.Name() + "\told=" + fc.oldValue + "\tnew=" + fc.newValue);
            }
            notification.consume = true;
        }
    }
}
