using System;
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
        var exmsg = ($"*** Exception SHOULD NOT HAPPEN!");
        //Console.Error.WriteLine(exmsg);
        Console.WriteLine(exmsg);
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
    public static Dictionary<String, int> EIG;



    private static void Main()
    {
        ReadParamater();
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
        MaxLevel = 2;
        Index = 1;
        Init = 0;
        V0 = 0;
        IsFaulty = 0;
        */
        Console.WriteLine($"Receive: {MaxIndex}+{MaxLevel}+{Index}+{Init}+{V0}+{IsFaulty}+{FileName}");
    }
}