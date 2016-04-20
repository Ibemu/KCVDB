namespace KCVDB
{
	public static class Constants
	{
		public static class BlobStorage
		{
			public static string ApiDataStorageKey { get; } = "KCVDBStorageConnectionString";
			public static string ApiDataBlobContainerName { get; } = "kancolleapidataraw"; // 必ず小文字な！
            public static string ApiDataTableContainerName { get; } = "sessionmanage";
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

            /// <summary>
            /// api_portの部分一致検索用
            /// </summary>
            public static string ApiPortPath { get; } = "/kcsapi/api_port/port";

            /// <summary>
            /// 24時を基準にBlobを分ける時間を設定
            /// </summary>
            public static System.TimeSpan OffsetTime { get; } = new System.TimeSpan(0, 0, 0);
		}
	}
}
