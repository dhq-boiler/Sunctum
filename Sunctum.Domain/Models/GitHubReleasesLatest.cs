using Homura.ORM;
using Homura.ORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models
{
    public class GitHubReleasesLatest : EntityBaseObject
    {
        private string _URL;

        [Column("URL", "Text", 0), PrimaryKey, Index]
        public string URL
        {
            get { return _URL; }
            set { SetProperty(ref _URL, value); }
        }
    }
}
