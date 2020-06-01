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

			OptionSet os = new OptionSet ()
			{
				{ "r|repository=", "Github repository to consider.", b => options.Repository = b },
				{ "t|token=", "Token to use to communicate with Github via OctoKit", t => options.Pat = t },
				{ "d|days=", "Show mentions in last given Days (Default 14)", d => options.Days = Int32.Parse(d) },
				{ "l|label=", "Show mentions on issues with given Label (Default ci-failure)", l => options.Label = l },
			};

			try {
				IList<string> unprocessed = os.Parse (args);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not parse the command line arguments: {0}", e.Message);
				return;
			}

			options.Validate ();

			var issues = new Issues (options);
			var crossRefCount = await issues.Find ();
			foreach (var issue in crossRefCount.OrderBy (x => x.ReferenceCount).Reverse ().Take (10)) {
				Console.WriteLine ($"{issue.ID} {issue.Title} {issue.ReferenceCount}");
			}
		}
	}
}
