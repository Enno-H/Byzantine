using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

[DataContract]
public class Message {
    public Message (int time, int from, int to, string msg) {
        Time = time;
        From = from;
        To = to;
        Msg = msg;
    }
    [DataMember]
    public int Time { get; set; }
    [DataMember]
    public int From { get; set; }
    [DataMember]
    public int To { get; set; }
    [DataMember]
    public string Msg { get; set; }
}

[ServiceContract()]
public interface INodeService {
    [OperationContract(IsOneWay=false)]
    [WebInvoke(RequestFormat=WebMessageFormat.Json,ResponseFormat=WebMessageFormat.Json)]
    Message[] Messages(Message[] msgs);

    [OperationContract()]
    [WebGet(ResponseFormat = WebMessageFormat.Json)]
    void sayHello();
}
