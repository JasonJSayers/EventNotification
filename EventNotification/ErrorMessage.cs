using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventNotification
{
    // Class for parsing out error messages
    class ErrorMessage
    {
        public ErrorMessage(string error)
        {
            // Split the error into chunks based on pipes in message
            string[] chunks = error.Split('|');

            // Check that we are able to parse the date and that there are at least 4 sections
            if (chunks.Length > 3 && DateTime.TryParse(chunks[0], out _date))
            {
                _key1 = chunks[1];
                _key2 = chunks[2];
                _description = chunks[3];

                _fullKey = _key1 + Key2;
            }
            else _parsingError = true;
        }

        // holds date and time of error
        private DateTime _date;
        public DateTime Date { get { return _date; } }

        private string _fullKey;
        public string FullKey { get { return _fullKey; } }

        // Holds the first key
        private string _key1;
        public string Key1 { get { return _key1; } }

        // Holds the second key
        private string _key2;
        public string Key2 { get { return _key2; } }

        // Hold the error message
        private string _description;
        public string Description { get { return _description; } }

        // Indicates if the error was formatted incorrectly
        public bool _parsingError = false;
        public bool ParsingError { get { return _parsingError; } }

        public override string ToString()
        {
            return _date.ToString("MM/dd/yyyy HH:mm:ss") + "|" + _key1 + "|" + _key2 + "|" + _description;
        }
    }
}
