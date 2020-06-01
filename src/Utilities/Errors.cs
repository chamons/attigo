using System;

namespace attigo.Utilities
{
	public static class Errors
	{
		public static void Die (string v)
		{
			Console.Error.WriteLine (v);
			Environment.Exit (-1);
		}
	}
}