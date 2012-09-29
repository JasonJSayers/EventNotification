using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventNotification
{
    class ErrorMessage
    {
        public ErrorMessage(string error)
        {
            _error = error;
            string[] chunks = error.Split('|');

            if (chunks.Length > 3 && DateTime.TryParse(chunks[0], out _date))
            {
                _key1 = chunks[1];
                _key2 = chunks[2];
                _message = chunks[3];
            }
            else _parsingError = true;
        }

        private DateTime _date;
        public DateTime Date { get { return _date; } }

        private string _key1;
        public string Key1 { get { return _key1; } }

        private string _key2;
        public string Key2 { get { return _key2; } }

        private string _message;
        public string Message { get { return _message; } }

        private string _error;
        public string Error { get { return _error; } }

        public bool _parsingError = false;
        public bool ParsingError { get { return _parsingError; } }
    }
}
