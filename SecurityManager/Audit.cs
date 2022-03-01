using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
	public class Audit : IDisposable
	{
		private static EventLog customLog = null;
		const string SourceName = "XMS.Audit";
		const string LogName = "XMS";

		static Audit()
		{
			try
			{
				if (!EventLog.SourceExists(SourceName))
				{
					EventLog.CreateEventSource(SourceName, LogName);
				}
				customLog = new EventLog(LogName, Environment.MachineName, SourceName);
			}
			catch (Exception e)
			{
				customLog = null;
				Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
			}
		}


		public static void NewDataStored(string time, string user, string file, string deniedAccessType)
		{
			if (customLog != null)
			{
				string message = $"Time: {time}, User: {user}, File: {file}, DeniedAccessType: {deniedAccessType}";
				customLog.WriteEntry(message);
			}
			else
			{
				throw new ArgumentException(string.Format("Error while trying to write event to event log."));
			}
		}

		public void Dispose()
		{
			if (customLog != null)
			{
				customLog.Dispose();
				customLog = null;
			}
		}
	}
}
