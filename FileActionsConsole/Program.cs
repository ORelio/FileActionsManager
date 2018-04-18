using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpTools;

namespace FileActionsConsole
{
    /// <summary>
    /// Command-line utility for creating/deleting file context menu actions
    /// By ORelio - (c) 2015-2018 - Available under the CDDL-1.0 license
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //ShellFileType.AddAction("txt", "testaction", "Testing FileActionsManager", "cmd.exe /C echo %1 && pause > nul");
                //ShellFileType.RemoveAction("txt", "testaction");

                if (args.Length == 3 && args[0] == "del")
                {
                    ShellFileType.RemoveAction(args[1].Split(','), args[2]);
                }
                else if ((args.Length == 5 || args.Length == 6) && args[0] == "add")
                {
                    ShellFileType.AddAction(args[1].Split(','), args[2], args[3], args[4], args.Length == 6 && args[5] == "default");
                }
                else
                {
                    string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
                    Console.WriteLine(" - Windows Shell Menu Action Setter v1.0 - By ORelio -\n");
                    Console.WriteLine("Usage: " + exeName + " add <ext> <int> <dsp> <cmd> [def]");
                    Console.WriteLine("Usage: " + exeName + " del <ext> <int>\n");
                    Console.WriteLine("add : Add or update the action based on internal name");
                    Console.WriteLine("del : Remove the action based on internal name");
                    Console.WriteLine("ext : List of file extensions to affect eg mp3 or mp3,mp4 and so on");
                    Console.WriteLine("int : internal name to designate the action");
                    Console.WriteLine("dsp : display name of the shell menu item");
                    Console.WriteLine("cmd : command to execute when selecting item. File is provided as %1");
                    Console.WriteLine("def : add `default' as last argument for setting the action as the default one");
                }
            }
            catch (UnauthorizedAccessException)
            {
                RelaunchAsAdmin(args, true);
            }
        }

        public static void RelaunchAsAdmin(string[] args, bool waitforexit = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Verb = "runas";
            startInfo.Arguments = String.Join(" ", args.Select(arg => "\"" + args + '"').ToArray());
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            
            Process process = Process.Start(startInfo);
            
            if (waitforexit)
                process.WaitForExit();
        }
    }
}
