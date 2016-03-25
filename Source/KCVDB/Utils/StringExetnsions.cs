namespace KCVDB.Utils
{
	public static class StringExetnsions
	{
		public static string RemoveNewLiens(this string str)
		{
			return str.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
		}
	}
}
