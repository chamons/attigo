using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Octokit;

using attigo.Utilities;

namespace attigo.Requests
{
	public class Issues
	{
		GitHubClient Client;
		RequestOptions Options;

		public Issues (RequestOptions options)
		{
			Options = options;
			Client = new GitHubClient (new ProductHeaderValue ("chamons.attigo"));
			Client.Credentials = new Credentials (Options.Pat);
		}

		public async Task AssertLimits ()
		{
			var limits = await Client.Miscellaneous.GetRateLimits ();
			int coreLimit = limits.Resources.Core.Remaining;
			int searchLimit = limits.Resources.Search.Remaining;
			if (coreLimit < 1 || searchLimit < 1)
				Errors.Die ($"Rate Limit Hit: {coreLimit} {searchLimit}");
		}

		(string Owner, string Area) ParseLocation (string location)
		{
			var bits = location.Split ('/');
			if (bits.Length != 2)
				Errors.Die ("--github formatted incorrectly");
			return (bits[0], bits[1]);
		}

		public async Task Find ()
		{
			var (owner, area) = ParseLocation (Options.Repository);

			var issueRequest = new RepositoryIssueRequest { Filter = IssueFilter.All, State = ItemStateFilter.Open };
			//var allIssues = await Client.Issue.GetAllForRepository (owner, area, issueRequest);
			//var testIssue = allIssues.First (x => x.Number == 581);
			//Console.WriteLine (testIssue.ToString ());

			var timelineEventInfos = await Client.Issue.Timeline.GetAllForIssue (owner, area, 581);
			foreach (var x in timelineEventInfos) {
				if (x.Event.TryParse (out EventInfoState eventState)) {
					switch (eventState) {
					case EventInfoState.Crossreferenced:
						Console.WriteLine ($"{x.Event.StringValue} {x.Source.Issue.Number}");
						continue;
					}
				}
			}
		}
	}
}
