public class Message : RequestModel<Message>
{
    public int MessageType;
    public string Data;

    public Message()
    {
            
    }

    public Message(string data, MessageType type)
    {
        Data = data;
        MessageType = (int)type;
    }
}