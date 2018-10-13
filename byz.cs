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

        /*var exmsg = ($"*** Exception SHOULD NOT HAPPEN!");
        //Console.Error.WriteLine(exmsg);
        Console.WriteLine(exmsg);
        return null;
        */

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
            }
        }

        return null;
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



    private static void Main()
    {
        ReadParamater();
        //BuildHost();
        createEIG();
    }

    private static void ReadParamater()
    {
        /*
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
        */
        MaxIndex = 4;
        MaxLevel = 4;
        Index = 1;
        Init = 0;
        V0 = 0;
        IsFaulty = 0;
        EIG.Add("λ", Index);
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
                Console.WriteLine(s);
                EIG.Add(s, -1);
                if (s.Length < MaxLevel)
                {
                    AppendString(s);
                }
            }
        }
    }


}