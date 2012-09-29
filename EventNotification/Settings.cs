using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventNotification
{
    // Commandline settings class for parsing potential settings
    //  forces all setting names to be upper case to avoid potential setup issues
    class CommandLineSettings
    {
        // Holds the integer value for the number of errors to report on within a 5 min timespan
        private static int _threshold = 0;
        public static int Threshold { get { return _threshold; } }

        // Holds the location of the log file to check for error messages in
        private static string _logFile = string.Empty;
        public static string LogFile { get { return _logFile; } }

        // Holds settings for easy retrieval
        private static Dictionary<string, string> _arguments;

        // Parses through the commandline arguments to determine settings what settings to use
        //  returns false if settings have not been set or are outside specifications.
        public static bool Parse(string[] args)
        {
            bool success = true;

            ParseArguments(args);

            // Retrieves the error threshold for determining if an e-mail must be sent
            //  reports an issue if threshold is set not set or set incorrectly.
            _threshold = GetInt("Threshold");
            if (0 < _threshold && _threshold < 10)
                Console.WriteLine("Threshold set to " + _threshold.ToString());
            else
            {
                Console.WriteLine("Threshold needs to be set to a number 1-9.");
                success = false;
            }

            // Retrieves name of the log file to analyze
            //  reports an issue if the logfile is not set.
            _logFile = GetString("LogFile");
            if (!string.IsNullOrEmpty(_logFile))
                Console.WriteLine("Log file set to '" + _logFile + "'");
            else
            {
                Console.WriteLine("Log File was not specified.");
                success = false;
            }
            
            // Returns if commandline settings have been successfully retrieved
            return success;
        }

        // Parses individual arguments and places them into a dictionary for simple retrieval
        //  forces settings names from commandline to be in upper case
        private static void ParseArguments(string[] args)
        {
            _arguments = new Dictionary<string, string>();

            foreach (string argument in args)
            {
                int i = argument.IndexOf(':');

                if (i >= 1)
                    _arguments[argument.Substring(0, i).ToUpper()] = argument.Substring(i + 1);
                else
                    _arguments[argument.ToUpper()] = string.Empty;

            }
        }

        // Checks setting dictionary to check if a flag has been set on the commandline
        private static bool GetFlag(string flag)
        {
            return _arguments.ContainsKey(flag.ToUpper());
        }

        // Returns a string setting from commandline if it exists, else returns an empty string
        private static string GetString(string setting)
        {
            return _arguments.ContainsKey(setting.ToUpper()) ? _arguments[setting.ToUpper()] : string.Empty;
        }

        // Returns an integer setting from commandline if it exists and is numeric, else returns a 0
        private static int GetInt(string setting)
        {
            int i = 0;

            if (_arguments.ContainsKey(setting.ToUpper()) && int.TryParse(_arguments[setting.ToUpper()], out i))
                return i;

            return 0;
        }

    }
}
