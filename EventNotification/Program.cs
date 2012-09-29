using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EventNotification
{
    class Program
    {
        private static int errorCount = 0;

        static void Main(string[] args)
        {

            try
            {
                // Retrieving application settings from the commandline
                if (!CommandLineSettings.Parse(args))
                    Error("Unable to parse commandline. Program terminating.");

                // Do not run through main logic if there are currently errors
                if (!HasErrors)
                {
                    ParseEventLog();
                }
            }
            catch (Exception ex)
            {
                Error("Error encountered:\r\n" + ex.ToString()); 
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

        private static void ParseEventLog()
        {
            try
            {
                // Check to see that file exists.
                if (System.IO.File.Exists(CommandLineSettings.LogFile))
                {
                    StreamReader input;

                    // Open log file for reading and allow access to other programs to continue to read and write to file
                    input = new StreamReader(new FileStream(CommandLineSettings.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                    while (!input.EndOfStream)
                    {
                        string line = input.ReadLine();

                        try
                        {

                            ErrorMessage myError = new ErrorMessage(line);
                        }
                        catch (Exception ex)
                        {
                            Error("Error encountered while line" + ex.ToString());
                        }
                    }


                }
                else Error("Log file '" + CommandLineSettings.LogFile + "' does not exist. Program terminating.");

            }
            catch (Exception ex)
            {
                Error("Error encountered while parsing file: \r\n" + ex.ToString()); 
            }
        }
    }
}
