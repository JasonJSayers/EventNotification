Searched for how to use smtp from link
	http://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.aspx
And used the following link to understand how to set up credentials
	http://stackoverflow.com/questions/1311749/c-sharp-smtpclient-class-not-able-to-send-email-using-gmail




Thoughts:

Should the program run continuously?  
	- Potentially having the main logic running in a background thread.  
		Reading the log file every 1-5 minutes.
		With the ability to terminate the program from a prompt.
	- Should the module store off information to know where to start off when it runs again if it's used as currently coded?
	- If the application were written continuously, should it be running as a service?

Where is the best place to store the smtp information?
	- Should this information be hard coded, pulled from an ini file, entered by the user, etc.?
