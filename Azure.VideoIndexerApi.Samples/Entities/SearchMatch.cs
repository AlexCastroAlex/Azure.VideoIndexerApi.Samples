using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.VideoIndexerApi.Samples.Entities
{
    public class SearchMatch
    {
        public TimeSpan startTime;
        public string type;
        public string text;
        public string exactText;
    }
}
