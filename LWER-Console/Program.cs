using System;

namespace LWER_Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            LWER lwer = new LWER();
            while(true)
            {
                Console.Write(System.IO.Directory.GetCurrentDirectory() + "> ");
                string input = Console.ReadLine();
                if (input == "exit") break;
                string ss = lwer.DiscernSubsystem(input);
                lwer.Exec();
            }
        }
        
    }
}
