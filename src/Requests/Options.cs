using System;
using System.IO;
using System.Linq;

using attigo.Utilities;

namespace attigo.Requests
{
	public class RequestOptions
	{
		public string Repository = null;
		public string Pat = null;
		public int Days = 14;

		public void Validate ()
		{
			if (String.IsNullOrEmpty (Repository))
				Errors.Die ("Unable to read repository location");

			if (String.IsNullOrEmpty (Pat)) {
				var defaultPat = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), ".github-status-pat");
				if (File.Exists (defaultPat)) {
					Pat = File.ReadAllLines (defaultPat).First ();
				}
			}
			if (String.IsNullOrEmpty (Pat)) {
				Errors.Die ("Unable to read GitHub PAT token");
			}
		}
	}
}