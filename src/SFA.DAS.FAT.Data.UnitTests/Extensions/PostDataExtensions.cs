﻿using Elasticsearch.Net;
using System.IO;
using System.Text;

namespace SFA.DAS.FAT.Data.UnitTests.Extensions
{
    public static class PostDataExtensions
    {
        public static string GetRequestString(this PostData pd)
        {
            using (var stream = new MemoryStream())
            {
                pd.Write(stream, new ConnectionConfiguration());
            }

            if (pd.WrittenBytes == null || pd.WrittenBytes.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(pd.WrittenBytes);
        }
    }
}