using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.VideoIndexerApi.Samples.Entities
{
    public class MediaAsset
    {
        public string id { get; set; }
        public string name { get; set; }
        public string partition { get; set; }
        public string description { get; set; }
        public DateTime created { get; set; }
        public string userName { get; set; }
        public string sourceLanguage { get; set; }
        public int durationInSeconds { get; set; }
    }
}
