using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boilersUpdater.Models
{
    public class Author
    {
        public string login { get; set; }
        public long id { get; set; }
        public string node_id { get; set; }
        public Uri avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public Uri url { get; set; }
        public Uri html_url { get; set; }
        public Uri followers_url { get; set; }
        public Uri following_url { get; set; }
        public Uri gists_url { get; set; }
        public Uri starred_url { get; set; }
        public Uri subscriptions_url { get; set; }
        public Uri organizations_url { get; set; }
        public Uri repos_url { get; set; }
        public Uri events_url { get; set; }
        public Uri received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }
}
