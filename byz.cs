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
        int time = imsgs[0].Time;

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
        //发送阶段
        if (time < byz.MaxLevel)
        {
            //发送新消息
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
        }
        //判断阶段
        else
        {
            Evaluate();
            outputList.Add(new Message(imsgs[0].Time, byz.Index, 0, "Finish"));
        }

        return outputList.ToArray();
    }

    public void Evaluate()
    {
        //复制底层
        foreach (var key in byz.EIG.Keys)
        {
            if (key.Length == byz.MaxLevel)
            {
                byz.EIG_eva[key] = byz.EIG[key];
            }
        }

        VoteResult("");

    }

    public int VoteResult(string str)
    {
        if (str.Length == byz.MaxLevel)
        {
            Console.WriteLine($"{str}: {byz.EIG_eva[str]}");
            return byz.EIG_eva[str];
        }
        else
        {
            int sum0 = 0;
            int sum1 = 0;
            int sum2 = 0;
            for (int i = 1; i <= byz.MaxIndex; i++)
            {
                string childKey = str + i;
                if (byz.EIG_eva.ContainsKey(childKey))
                {
                    int childResult = VoteResult(childKey);
                    if (childResult == 0)
                    {
                        sum0++;
                    }
                    if (childResult == 1)
                    {
                        sum1++;
                    }
                    if (childResult == 2)
                    {
                        sum2++;
                    }
                }
            }

            int max = Math.Max(Math.Max(sum0, sum1), sum2);
            if (sum0 == max)
            {
                Console.WriteLine($"{str}: 0");
                return 0;
            }
            if (sum1 == max)
            {
                Console.WriteLine($"{str}: 1");
                return 1;
            }
            if (sum2 == max)
            {
                Console.WriteLine($"{str}: 2");
                return 2;
            }

        }

        return -1;
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
    public static Dictionary<string, int> EIG_eva = new Dictionary<string, int>();


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
                EIG_eva.Add(s, -1);
                if (s.Length < MaxLevel)
                {
                    AppendString(s);
                }
            }
        }
    }


}