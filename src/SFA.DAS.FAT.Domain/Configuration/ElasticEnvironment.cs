﻿namespace SFA.DAS.FAT.Domain.Configuration
{
    public class ElasticEnvironment
    {
        public string Prefix { get; }

        public ElasticEnvironment(string prefix)
        {
            Prefix = prefix.ToLower();
        }
    }
}
