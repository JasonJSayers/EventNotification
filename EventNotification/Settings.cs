using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventNotification
{
    // Settings class for potentially prompting user for settings or pulling settings from commandline
    class Settings
    {
        const string DEFAULT_HOST = "smtp.gmail.com";
        const int DEFAULT_PORT = 587;
        const string DEFAULT_ADDRESS = "csg.developer@gmail.com";

        // Holds the host string for the smtp client
        private static string _host;
        public static string Host { get { return _host; } }

        // Holds the port to use for the smpt client
        private static int _port;
        public static int Port { get { return _port; } }

        // Holds the address to send the e-mail to
        private static string _sendTo;
        public static string SendTo { get { return _sendTo; } }

        // Holds the address the e-mail is sent from
        private static string _sendFrom;
        public static string SendFrom { get { return _sendFrom; } }

        // Holds password used to connect to smtp host
        private static string _password;
        public static string Password { get { return _password; } }

        public static void Get()
        {
            if (CommandLineSettings.ProptUserForHostInfo)
            {
                Console.Write("Host: ");
                _host = Console.ReadLine();

                Console.Write("Port: ");
                if (!int.TryParse(Console.ReadLine(), out _port))
                {
                    Console.WriteLine("Invalid port. Defaulting to " + DEFAULT_PORT.ToString());
                    _port = DEFAULT_PORT;
                }
            }
            else
            {
                // Get the host from the commandline, default if blank
                _host = CommandLineSettings.GetString("Host", DEFAULT_HOST);
                Console.WriteLine("Host: " + Host);

                // Get the port from the commandline, default if blank
                _port = CommandLineSettings.GetInt("Port", DEFAULT_PORT);
                Console.WriteLine("Port: " + Port.ToString());
            }

            if (CommandLineSettings.PromptForAddressInformation)
            {
                Console.Write("Send From: ");
                _sendFrom = Console.ReadLine();

                Console.Write("Send To: ");
                _sendTo = Console.ReadLine();
            }
            else
            {
                // Get the send to address, if not set place in a default value
                _sendTo = CommandLineSettings.GetString("SendTo", "csg.developer@gmail.com");
                Console.WriteLine("Send To: " + SendTo);

                // Get the send from address, if not set place in a default value
                _sendFrom = CommandLineSettings.GetString("SendFrom", "csg.developer@gmail.com");
                Console.WriteLine("Send From: " + SendFrom);
            }

            _password = CommandLineSettings.GetString("Password");
            if (string.IsNullOrEmpty(_password))
            {
                Console.Write("Password:");
                _password = Console.ReadLine();
            }
        }
    }

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

        // Prompt the user for Host information?
        private static bool _promptForHostInfo;
        public static bool ProptUserForHostInfo { get { return _promptForHostInfo; } }

        // Prompt the user for send to and send from information
        private static bool _promptForAddressInformation;
        public static bool PromptForAddressInformation { get { return _promptForAddressInformation; } }
        
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
                Console.WriteLine("Threshold: " + _threshold.ToString());
            else
            {
                Console.WriteLine("Threshold needs to be set to a number 1-9.");
                success = false;
            }

            // Retrieves name of the log file to analyze
            //  reports an issue if the logfile is not set.
            _logFile = GetString("LogFile");
            if (!string.IsNullOrEmpty(_logFile))
                Console.WriteLine("Log File: '" + _logFile + "'");
            else
            {
                Console.WriteLine("Log File was not specified.");
                success = false;
            }

            // Flag indicates that user needs to enter host information through the console
            _promptForHostInfo = GetFlag("-H");

            // Flag indicates that user needs to enter to and from information from the console.
            _promptForAddressInformation = GetFlag("-A");
            
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
                    _arguments[argument.Substring(0, i).ToUpper()] = argument.Substring(i + 1).Trim('"');  // Triming  double quotes from settings that require it.
                else
                    _arguments[argument.ToUpper()] = string.Empty;

            }
        }

        // Checks setting dictionary to check if a flag has been set on the commandline
        public static bool GetFlag(string flag)
        {
            return _arguments.ContainsKey(flag.ToUpper());
        }

        // Returns a string setting from commandline if it exists, else returns an empty string
        public static string GetString(string setting)
        {
            return _arguments.ContainsKey(setting.ToUpper()) ? _arguments[setting.ToUpper()] : string.Empty;
        }

        // Returns a Default value if setting is not available
        public static string GetString(string setting, string defaultValue)
        {
            return _arguments.ContainsKey(setting.ToUpper()) ? _arguments[setting.ToUpper()] : defaultValue;
        }

        // Returns an integer setting from commandline if it exists and is numeric, else returns a 0
        public static int GetInt(string setting)
        {
            int i = 0;

            if (_arguments.ContainsKey(setting.ToUpper()) && int.TryParse(_arguments[setting.ToUpper()], out i))
                return i;

            return 0;
        }

        // Returns an integer setting from commandline if it exists and is numeric, else returns a 0
        public static int GetInt(string setting, int defaultValue)
        {
            int i = 0;

            if (_arguments.ContainsKey(setting.ToUpper()) && int.TryParse(_arguments[setting.ToUpper()], out i))
                return i;

            return defaultValue;
        }

    }
}
