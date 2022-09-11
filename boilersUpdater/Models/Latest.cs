using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boilersUpdater.Models
{
    public class Latest
    {
        public Uri url { get; set; }
        public Uri assets_url { get; set; }
        public Uri upload_url { get; set; }
        public Uri html_url { get; set; }
        public long id { get; set; }
        public Author author { get; set; }

        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public Asset[] assets { get; set; }
        public Uri tarball_url { get; set; }
        public Uri zipball_url { get; set; }
        public string body { get; set; }
    }
}
