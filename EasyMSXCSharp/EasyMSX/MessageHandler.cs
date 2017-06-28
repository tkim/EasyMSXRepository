using Message = Bloomberglp.Blpapi.Message;

namespace com.bloomberg.emsx.samples {

    interface MessageHandler {
	    void processMessage(Message message);
    }

}