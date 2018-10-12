using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// [ServiceContract()]
// public interface INodeService { ... }

[ServiceBehavior(IncludeExceptionDetailInFaults=true,
     InstanceContextMode=InstanceContextMode.PerCall,
     ConcurrencyMode=ConcurrencyMode.Multiple)] // Reentrant)] 
public class ArcService : INodeService {
    public static AutoResetEvent Done = new AutoResetEvent (false);
    
    public static Func<int> Tid = () => Thread.CurrentThread.ManagedThreadId;
    
    public static Func<double> Millis = () => DateTime.Now.TimeOfDay.TotalMilliseconds;
    
    public static int N;
    
    public static int PortFor (int n = 0) {
        return n==0? 8090: 8080+n;
    }
    
    public static SortedDictionary<int, List<Message>> MessageQueue =
        new SortedDictionary<int, List<Message>> ();
    
    public static async Task MessageLoop () {
        await Task.Delay (0);
        
        while (MessageQueue.Count > 0) {
            var topk = MessageQueue.Keys.First ();
            var msgs = MessageQueue[topk].GroupBy (m => m.To);
            MessageQueue.Remove (topk);
            
            foreach (var togroup in msgs) {
                var to = togroup.Key;
                var omsgs = togroup.ToArray();
                
                if (to == 0) {
                    // Console.Error.WriteLine ($"... {topk}->{msgs.Count()} {omsgs.Length}!");
                    var msg = omsgs[0];
                    var done = ($"All Done: Time={msg.Time,3}, Msg={msg.Msg}!");
                    Console.Error.WriteLine (done);
                    Console.WriteLine (done);
                    break;
                }
                
                var imsgs = Invoke (to, omsgs);
                
                foreach (var m in imsgs) {
                    Console.WriteLine ($"... {m.Time,3} {0,2} < {m.From,2} {m.To,2} {m.Msg}");

                    int delay = 1;
                    if (m.To != 0) {
                        m.Time = m.Time + delay;
                    }
                    
                    if (! MessageQueue.Keys.Contains(m.Time)) {
                        MessageQueue[m.Time] = new List<Message> ();
                    }
                    MessageQueue[m.Time].Add(m);                        
                }
            }
        }

        //Environment.Exit (0);
        //Done.Set ();
    }
    
    public static Message[] Invoke (int to, Message[] msgs) {
        WebChannelFactory<INodeService> wcf = null;
        OperationContextScope scope = null;
        
        try {
            var uri = new Uri ($"http://localhost:{PortFor(to)}/");
            wcf = new WebChannelFactory<INodeService> (uri);
            var channel = wcf.CreateChannel ();

            scope = new OperationContextScope ((IContextChannel)channel);
            foreach (var msg in msgs) {
                Console.WriteLine ($"... {msg.Time,3} {0,2} \t> {msg.From,2} {msg.To,2} {msg.Msg}");
            }
            //return new Message[0];
            channel.sayHello();
            return channel.Messages (msgs);
           
        } catch (Exception ex) {
            var exmsg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine (exmsg);
            Console.WriteLine (exmsg);
            wcf = null;
            scope = null;
        
        } finally {
            if (wcf != null) ((IDisposable)wcf).Dispose();
            if (scope != null) ((IDisposable)scope).Dispose();
        }

        return new Message[0];
    }

    public Message[] Messages (Message[] imsgs) {
        var exmsg = ($"*** Exception SHOULD NOT HAPPEN!");
        Console.Error.WriteLine (exmsg);
        Console.WriteLine (exmsg);
        return null;
    }

    public void sayHello() {
        Console.WriteLine("HHHH");
    }
}

public class ArcHost {    
    public static void Main (string[] args) {
        WebServiceHost host = null;

        try {
            var n = int.TryParse (args[0], out ArcService.N);
            if (!n || ArcService.N < 1 || ArcService.N > 9) throw new Exception ("Argument N Incorrect");
            
            var baseAddress = new Uri($"http://localhost:{ArcService.PortFor()}/");
            host = new WebServiceHost (typeof(ArcService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint (typeof(INodeService), new WebHttpBinding(), "");

            host.Open();

            // http://localhost:8090/message?from=1,to=0,msg=...
            var msg = ($"Arc=0: {baseAddress}Message?from=?,to=?,msg=?");
            Console.Error.WriteLine (msg);
            Console.WriteLine (msg);
            
            ArcService.MessageQueue[1] = 
                Enumerable.Range (1, ArcService.N)
                .Select (I => new Message(0, 0, I, "+"))
                .ToList();
            var msgloop = ArcService.MessageLoop ();
            msgloop.Wait ();
            
            //Console.Error.WriteLine ("Press <Enter> to stop the service.");
            //Console.ReadLine ();
            //ArcService.Done.WaitOne ();

            host.Close ();
        
        } catch (Exception ex) {
            var msg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine (msg);
            Console.WriteLine (msg);
            host = null;
        
        } finally {
            if (host != null) ((IDisposable)host).Dispose();
        }
    }
}

