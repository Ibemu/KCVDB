namespace KCVDB
{
	public static class Constants
	{
		public static class Storage
		{
			public static string ApiDataStorageKey { get; } = "KCVDBStorageConnectionString";
			public static string ApiDataBlobContainerName { get; } = "kancolleapidataraw"; // 必ず小文字な！
			public static string BlobFileNameDateTimeToStringFormat { get; } = @"yyyy\\MM\\dd";
			public static string BlobFileNameFormat { get; } = @"{0}.log";
			public static string ApiRawFileNewLine { get; } = "\r\n";
		}
	}
}
