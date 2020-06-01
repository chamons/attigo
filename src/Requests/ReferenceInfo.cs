using System;

namespace attigo.Requests
{
	public class ReferenceInfo
	{
		public string Title;
		public int ID;
		public int ReferenceCount;

		public ReferenceInfo (string title, int id, int referenceCount)
		{
			Title = title;
			ID = id;
			ReferenceCount = referenceCount;
		}
	}
}