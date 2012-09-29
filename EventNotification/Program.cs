using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventNotification
{
    class Program
    {
        private static int errorCount = 0;

        static void Main(string[] args)
        {
            // Retrieving application settings from the commandline
            if (!CommandLineSettings.Parse(args))
                Error("Unable to parse commandline.  Program terminating.");

            // Do not run through main logic if there are currently errors
            if (!HasErrors)
            {

            }

#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();
#endif
        }

        // Writes a line to the console
        private static void Info(string infoMessage)
        {
            Console.WriteLine(infoMessage);
        }

        // Writes a line to the console and increments the number of errors
        private static void Error(string errorMessage)
        {
            ++errorCount;
            Console.WriteLine(errorMessage);
        }

        // Returns true if the number of errors is more than 0
        private static bool HasErrors { get { return errorCount > 0; } }

    }
}
