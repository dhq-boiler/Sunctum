using System;

namespace boilersUpdater.Models
{
    public class Asset
    {
        public Uri url { get; set; }
        public long id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public Uploader uploader { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public long size { get; set; }
        public long download_count { get; set; }
        public DateTime created_at { get; set; }
        public DateTime boilersUpdaterd_at { get; set; }
        public Uri browser_download_url { get; set; }
    }
}