using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.ComponentModel;

namespace EventNotification
{
    class Program
    {
        private static int errorCount = 0;
        private static int warningCount = 0;
        private static Dictionary<string, List<DateTime>> messages = new Dictionary<string, List<DateTime>>();

        static void Main(string[] args)
        {
            try
            {
                // Retrieving application settings from the commandline
                if (!CommandLineSettings.Parse(args))
                    Error("Unable to parse commandline. Program terminating.");

                Settings.Get();

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
                    using (input = new StreamReader(new FileStream(CommandLineSettings.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        while (!input.EndOfStream)
                        {
                            // Get next line
                            string message = input.ReadLine();

                            // Parse error message
                            ErrorMessage errMsg = new ErrorMessage(message);

                            // Check that message is ok
                            if (!errMsg.ParsingError)
                            {
                                // Add error to list of similar messages and check for threshold
                                CheckForEmail(errMsg);
                            }
                            else Warn("Unable to parse record - '" + message + "'");
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

        private static void CheckForEmail(ErrorMessage errMsg)
        {
            List<DateTime> currentErrorList;                // Holds current error list
            DateTime cutOff = errMsg.Date.AddMinutes(-5);   // Holds current cut off time

            // Add a list for error type if it currently does not exist
            if (!messages.ContainsKey(errMsg.FullKey))
                messages.Add(errMsg.FullKey, new List<DateTime>());

            // Selected the list that matches current error type
            currentErrorList = messages[errMsg.FullKey];

            // Remove entries that are 5 min older then current error
            for (int i = currentErrorList.Count - 1; i > -1; --i)
                if (currentErrorList[i] < cutOff)
                    currentErrorList.RemoveAt(i);

            // Add current error to list.
            currentErrorList.Add(errMsg.Date);

            // Check to see if list meets threshold required
            //  if it is send email and reset list for current error type
            if (currentErrorList.Count >= CommandLineSettings.Threshold)
            {
                currentErrorList.Clear();
                SendEmail(errMsg);
            }
        }

        // Writes a line to the console
        private static void Info(string infoMessage)
        {
            Console.WriteLine("[Info] " + infoMessage);
        }

        // Writes a line to the console and increments the number of warnings
        private static void Warn(string warningMessage)
        {
            ++warningCount;
            Console.WriteLine("[Warning] " + warningMessage);
        }

        // Writes a line to the console and increments the number of errors
        private static void Error(string errorMessage)
        {
            ++errorCount;
            Console.WriteLine("[Error] " + errorMessage);
        }

        private static void SendEmail(ErrorMessage errMsg)
        {
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient(Settings.Host, Settings.Port);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(Settings.SendFrom, Settings.Password);

            // Specify the e-mail sender. 
            // Create a mailing address that includes a UTF8 character 
            // in the display name.
            MailAddress from = new MailAddress(Settings.SendFrom);
            // Set destinations for the e-mail message.
            MailAddress to = new MailAddress(Settings.SendTo);

            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = errMsg.ToString();
            message.Subject = errMsg.Description;

            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            // Sending message
            client.Send(message);

            // Clean up.
            message.Dispose();
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
        }
    }
}
