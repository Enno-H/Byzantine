using System;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


[ServiceBehavior(IncludeExceptionDetailInFaults = true,
    InstanceContextMode = InstanceContextMode.PerCall,
    ConcurrencyMode = ConcurrencyMode.Multiple)] // Reentrant)] 
public class ByzService : INodeService
{


    public Message[] Messages(Message[] imsgs)
    {
        List<Message> outputList = new List<Message>();

        foreach (var msg in imsgs)
        {

            //第一次发送
            if (msg.From == 0)
            {
                for (int i = 1; i <= byz.MaxIndex; i++)
                {
                    outputList.Add(new Message(msg.Time, byz.Index, i, Convert.ToString(byz.Init)));
                }

                return outputList.ToArray();
            }
            //每次都会收到四个消息
            //更新EIG
            int index = 0;
            var Keys = new List<string>(byz.EIG.Keys);
            foreach (string key in Keys)
            {
                if (key.Length == msg.Time && key[key.Length - 1].ToString().Equals(msg.From.ToString()))
                {
                    byz.EIG[key] = int.Parse(msg.Msg[index].ToString());
                    index++;
                }
            }
        }
        string m = "";
        foreach (var key in byz.EIG.Keys)
        {
            if (key.Length == imsgs[0].Time && !key.Contains(byz.Index.ToString()) && !key.Equals("λ"))
            {
                m = m + byz.EIG[key].ToString();
            }
        }
        for (int i = 1; i <= byz.MaxIndex; i++)
        {
            outputList.Add(new Message(imsgs[0].Time, byz.Index, i, m));
        }
        return outputList.ToArray();
    }

    public void sayHello()
    {
        Console.WriteLine("HHHH");
    }

}

public class byz
{
    //rem                          N  L  ID V  V0 F  resp
    //start "byz1" cmd /k byz.exe  4  2  1  0  0  1  byz1.txt

    public static int MaxIndex;
    public static int MaxLevel;
    public static int Index;
    public static int Init;
    public static int V0;
    public static int IsFaulty;
    public static string FileName = "None";
    public static Dictionary<string, int> EIG = new Dictionary<string, int>();

    public static Message[] Messages(Message[] imsgs)
    {
        List<Message> outputList = new List<Message>();
        Console.WriteLine("Receive");

        foreach (var msg in imsgs)
        {

            //第一次发送
            if (msg.From == 0)
            {
                for (int i = 1; i <= byz.MaxIndex; i++)
                {
                    outputList.Add(new Message(msg.Time, byz.Index, i, Convert.ToString(byz.Init)));
                }

                return outputList.ToArray();
            }
            //每次都会收到四个消息
            //更新EIG
            int index = 0;
            var Keys = new List<string>(EIG.Keys);
            foreach (string key in Keys)
            {
                if (key.Length == msg.Time && key[key.Length - 1].ToString().Equals(msg.From.ToString()))
                {
                    byz.EIG[key] = int.Parse(msg.Msg[index].ToString());
                    Console.WriteLine($"{key}-->{byz.EIG[key]}");
                    index++;
                }
            }
        }
        string m = "";
        foreach (var key in byz.EIG.Keys)
        {
            if (key.Length == imsgs[0].Time && !key.Contains(byz.Index.ToString()))
            {
                m = m + byz.EIG[key].ToString();
            }
        }
        for (int i = 1; i <= byz.MaxIndex; i++)
        {
            outputList.Add(new Message(imsgs[0].Time, byz.Index, i, m));
        }
        return outputList.ToArray();
    }

    private static void Main()
    {
        ReadParamater();
        createEIG();
        BuildHost();
        
        /*
        Message m1 = new Message(1, 1, 1, "0");
        Message m2 = new Message(1, 2, 1, "0");
        Message m3 = new Message(1, 2, 1, "1");
        Message m4 = new Message(1, 2, 1, "1");


        Message[] input = { m1, m2, m3, m4 };
        var output = Messages(input);
        Console.WriteLine("MMMMM");
        */
    }

    private static void ReadParamater()
    {
        
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        foreach (string item in commandLineArgs) {
            Console.WriteLine(item);
        }
        if (commandLineArgs.Length >= 2){
            MaxIndex = int.Parse(commandLineArgs[1]);
        }
        if (commandLineArgs.Length >= 3){
            MaxLevel = int.Parse(commandLineArgs[2]);
        }
        if (commandLineArgs.Length >= 4){
            Index = int.Parse(commandLineArgs[3]);
        }
        if (commandLineArgs.Length >= 5){
            Init = int.Parse(commandLineArgs[4]);
        }
        if (commandLineArgs.Length >= 6){
            V0 = int.Parse(commandLineArgs[5]);
        }
        if (commandLineArgs.Length >= 7){
            IsFaulty = int.Parse(commandLineArgs[6]);
        }
        if (commandLineArgs.Length >= 8){
            FileName = commandLineArgs[7];
        }
        
        /*
        MaxIndex = 4;
        MaxLevel = 4;
        Index = 1;
        Init = 0;
        V0 = 0;
        IsFaulty = 0;
        EIG.Add("λ", Index);
        */
    }

    private static void BuildHost()
    {
        WebServiceHost host = null;

        try
        {
            var baseAddress = new Uri($"http://localhost:{Index + 8080}/");
            host = new WebServiceHost(typeof(ByzService), baseAddress);
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INodeService), new WebHttpBinding(), "");

            host.Open();

            var msg = ($"Byz={Index}: {baseAddress}Message?from=?,to=?,msg=?");
            Console.Error.WriteLine(msg);
            Console.WriteLine(msg);

            Console.ReadLine();
            host.Close();

        }
        catch (Exception ex)
        {
            var msg = ($"*** Exception {ex.Message}");
            Console.Error.WriteLine(msg);
            Console.WriteLine(msg);
            host = null;
        }
        finally
        {
            if (host != null) ((IDisposable)host).Dispose();
        }
    }

    private static void createEIG()
    {
        AppendString("");
        Console.WriteLine($"EIG size: {EIG.Count}");
    }

    private static void AppendString(string str)
    {
        for (int i = 1; i <= MaxIndex; i++)
        {
            if (!str.Contains(i.ToString()))
            {
                string s = str + i;
                //Console.WriteLine(s);
                EIG.Add(s, -1);
                if (s.Length < MaxLevel)
                {
                    AppendString(s);
                }
            }
        }
    }


}