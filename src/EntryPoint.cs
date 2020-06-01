using System;
using System.Collections.Generic;

using Mono.Options;

using attigo.Requests;

namespace attigo
{
	class EntryPoint
	{
		static void Main (string[] args)
		{
			var options = new RequestOptions ();

			OptionSet os = new OptionSet ()
			{
				{ "r|repository=", "Github repository to consider.", b => options.Repository = b },
				{ "t|token=", "Token to use to communicate with Github via OctoKit", t => options.Pat = t },
			};

			try {
				IList<string> unprocessed = os.Parse (args);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not parse the command line arguments: {0}", e.Message);
				return;
			}

			options.Validate ();
		}
	}
}
