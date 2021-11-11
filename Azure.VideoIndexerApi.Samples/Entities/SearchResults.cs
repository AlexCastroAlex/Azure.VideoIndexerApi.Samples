using Azure.VideoIndexerApi.Samples.Entitiess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.VideoIndexerApi.Samples.Entities
{
    public class SearchResults
    {
        public List<SearchResult> results;
        public PageInfo nextPage;
    }
}
