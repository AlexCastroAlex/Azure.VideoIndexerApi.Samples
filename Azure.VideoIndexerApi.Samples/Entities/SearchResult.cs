using Azure.VideoIndexerApi.Samples.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.VideoIndexerApi.Samples.Entitiess
{
    public class SearchResult
    {
        public string id;
        public string name;
        public string description;
        public DateTime? created;
        public DateTime? lastModified;
        public DateTime? lastIndexed;
        public string userName;
        public int durationInSeconds;
        public List<SearchMatch> searchMatches;
    }
}
