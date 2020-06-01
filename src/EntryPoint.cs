using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mono.Options;

using attigo.Requests;

namespace attigo
{
	class EntryPoint
	{
		static async Task Main (string[] args)
		{
			var options = new RequestOptions ();
			bool showHelp = false;

			OptionSet os = new OptionSet ()
			{
				{ "h|?|help", "Displays the help", v => showHelp = true },
				{ "r|repository=", "Github repository to consider.", r => options.Repository = r },
				{ "t|token=", "Token to use to communicate with Github via OctoKit", t => options.Pat = t },
				{ "d|days=", "Show mentions in last given Days (Default 14)", d => options.Days = Int32.Parse(d) },
				{ "l|label=", "Show mentions on issues with given Label (Default ci-failure)", l => options.Label = l },
				{ "c|count=", "Show given top cross referenced items (Default 10)", c => options.Count = Int32.Parse (c) },
			};

			try {
				IList<string> unprocessed = os.Parse (args);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not parse the command line arguments: {0}", e.Message);
				return;
			}

			if (showHelp) {
				ShowHelp (os);
				return;
			}

			options.Validate ();

			var crossRefCount = await Issues.Create (options).Find ();
			foreach (var issue in crossRefCount.OrderBy (x => x.ReferenceCount).Reverse ().Take (options.Count)) {
				Console.WriteLine ($"{issue.ID} {issue.Title} {issue.ReferenceCount}");
			}
		}

		static void ShowHelp (OptionSet os)
		{
			Console.WriteLine ("attigo [options] path");
			os.WriteOptionDescriptions (Console.Out);
		}
	}
}
