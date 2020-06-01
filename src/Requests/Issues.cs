using System;
using System.IO;
using System.Collections.Generic;
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
		string Owner;
		string Area;

		public Issues (RequestOptions options)
		{
			Options = options;
			Client = new GitHubClient (new ProductHeaderValue ("chamons.attigo"));
			Client.Credentials = new Credentials (Options.Pat);
			(Owner, Area) = ParseLocation (Options.Repository);
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

		public async Task<List<ReferenceInfo>> Find ()
		{
			var issueRequest = new RepositoryIssueRequest { Filter = IssueFilter.All, State = ItemStateFilter.Open };

			var allIssues = await Client.Issue.GetAllForRepository (Owner, Area, issueRequest);
			var issuesWithLabel = allIssues.Where (x => x.Labels.Any (x => x.Name == Options.Label)).ToList ();

			var references = new List<ReferenceInfo> ();
			foreach (var issue in issuesWithLabel) {
				references.Add (await ParseIssueTimeline (issue));
			}
			return references;
		}

		async Task<ReferenceInfo> ParseIssueTimeline (Issue issue)
		{
			var timeline = await Client.Issue.Timeline.GetAllForIssue (Owner, Area, issue.Number);

			int crossRefCount = 0;
			foreach (var x in timeline) {
				if (x.Event.TryParse (out EventInfoState eventState) && eventState == EventInfoState.Crossreferenced) {
					crossRefCount++;
				}
			}

			return new ReferenceInfo (issue.Title, issue.Id, crossRefCount);
		}
	}
}