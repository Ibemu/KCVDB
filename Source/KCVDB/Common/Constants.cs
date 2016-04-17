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

            /// <summary>
            /// api_portの部分一致検索用
            /// </summary>
            public static string ApiPortEqual { get; } = "/kcsapi/api_port/port";

            /// <summary>
            /// api_start2の部分一致検索用
            /// </summary>
            public static string ApiStart2Equal { get; } = "/kcsapi/api_start2";

            /// <summary>
            /// 24時を基準にBlobを分ける時間を設定
            /// 例）5時なら-5と設定。23時なら1と設定。
            /// </summary>
            public static int OffsetTime { get; } = 0;
		}
	}
}
