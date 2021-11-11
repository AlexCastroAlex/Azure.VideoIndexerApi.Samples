using Azure.VideoIndexerApi.Samples.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoIndexerApiClient.Entities
{
    public class MediaAssetResults
    {
        public List<MediaAsset> results;
        public PageInfo nextPage;
    }
}
