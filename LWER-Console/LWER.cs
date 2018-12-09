using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace LWER_Console
{
    class LWER
    {
        static readonly int major = 0;
        static readonly int minor = 1;
        static readonly int patch = 0;
        private string[] args;
        private string arg_string;
        private string subsystem;
        public LWER()
        {
            WelcomeMessage();
        }
        public bool Exec()
        {
            switch(subsystem)
            {
                case "lwer":
                    ExecLWER(args);
                    return true;
                case "wsl":
                    ExecWSL(args);
                    return true;
                case "cmd":
                    ExecCMD(args);
                    return true;
                default:
                    return false;
            }
        }
        public string DiscernSubsystem(string input)
        {
            args = input.Split();
            arg_string = input;
            if (args[0] == "cd" || args[0] == "clear")
            {
                subsystem = "lwer";
                return "lwer";
            }
            if (IsWSL(args[0]))
            {
                subsystem = "wsl";
                return "wsl";
            }
            if (IsCMD(args[0]))
            {
                subsystem = "cmd";
                return "cmd";
            }
            else
                return "err";
        }
        public void WelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("LWER Console by James Anderson");
            Console.WriteLine("Version: {0}.{1}.{2}", major, minor, patch);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ExecLWER(string [] args)
        {
            switch(args[0])
            {
                case "cd":
                    ChangeDirectory(args);
                    break;
                case "clear":
                    Console.Clear();
                    break;
            }
        }
        public void ExecCMD(string [] args)
        {
            Console.WriteLine("Using CMD");
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine(arg_string);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string output = cmd.StandardOutput.ReadToEnd();
            string[] clean = output.Split('\n');
            for (int i = 4; i < clean.Length - 2; i++)
                Console.WriteLine(clean[i]);
        }
        public void ExecWSL(string [] args)
        {
            Console.WriteLine("Using WSL");
            Process wsl = new Process();
            wsl.StartInfo.FileName = "cmd.exe";
            wsl.StartInfo.RedirectStandardInput = true;
            wsl.StartInfo.RedirectStandardOutput = true;
            wsl.StartInfo.CreateNoWindow = true;
            wsl.StartInfo.UseShellExecute = false;
            wsl.Start();
            wsl.StandardInput.WriteLine("wsl " + arg_string);
            wsl.StandardInput.Flush();
            wsl.StandardInput.Close();
            wsl.WaitForExit();
            string output = wsl.StandardOutput.ReadToEnd();
            string[] clean = output.Split('\n');
            for (int i = 4; i < clean.Length - 2; i++)
                Console.WriteLine(clean[i]);
        }
        public bool IsCMD(string cmd)
        {
            if (File.Exists(@"C:\Windows\System32\" + cmd + ".com"))
                return true;
            else if (File.Exists(@"C:\Windows\System32\" + cmd + ".exe"))
                return true;
            return false;
        }
        public bool IsWSL(string cmd)
        {
            if (File.Exists(@"C:\Users\james\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu18.04onWindows_79rhkp1fndgsc\LocalState\rootfs\bin\" + cmd))
                return true;
            else if (File.Exists(@"C:\Users\james\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu18.04onWindows_79rhkp1fndgsc\LocalState\rootfs\usr\bin\" + cmd))
                return true;
            return false;
        }
        public bool ChangeDirectory(string[] input)
        {
            if (input.Length > 2)
            {
                ErrorMsg(arg_string, "Too many arguments");
                return false;
            }
            else
            {
                try
                {
                    Directory.SetCurrentDirectory(input[1]);
                    return true;
                }
                catch (ArgumentException)
                {
                    ErrorMsg(arg_string, "No path provided");
                }
                catch (PathTooLongException)
                {
                    ErrorMsg(arg_string, "Path is too long");
                }
                catch (SecurityException)
                {
                    ErrorMsg(arg_string, "You do not have required permissions");
                }
                catch (FileNotFoundException)
                {
                    ErrorMsg(arg_string, "Invalid path");
                }
                catch (IOException)
                {
                    ErrorMsg(arg_string, "An I/O exception has occured.");
                }
                return false;
            }
        } 
        public void ErrorMsg(string command, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write("Encontered an error processing command: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine("{0}", command);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("\t{0,-14}\t{1}", "Target system:", subsystem);
            Console.Error.WriteLine("\t{0,-14}\t{1}", "Error:", message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
