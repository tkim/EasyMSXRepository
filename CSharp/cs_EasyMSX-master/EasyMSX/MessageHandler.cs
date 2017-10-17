using Message = Bloomberglp.Blpapi.Message;

namespace com.bloomberg.emsx.samples {

    public interface MessageHandler {
	    void processMessage(Message message);
    }

}