namespace KCVDB
{
	public static class Constants
	{
		public static class BlobStorage
		{
			public static string ApiDataStorageKey { get; } = "KCVDBStorageConnectionString";
			public static string ApiDataBlobContainerName { get; } = "kancolleapidataraw"; // 必ず小文字な！
			public static string BlobFileNameDateTimeToStringFormat { get; } = @"yyyy\\MM\\dd";

			/// <summary>
			/// Blobファイル名のフォーマット
			/// </summary>
			/// <remarks>
			/// {0}: ディレクトリ名(日付)
			/// {1}: SessionID
			/// </remarks>
			public static string BlobFileNameFormat { get; } = @"{0}\{1}.log";
			public static string ApiRawFileNewLine { get; } = "\r\n";
		}
	}
}
