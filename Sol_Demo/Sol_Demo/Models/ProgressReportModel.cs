using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Demo.Models
{
    public class ProgressReportModel
    {
        public int PrecentageComplete { get; set; }

        public WebSiteModel SiteDownloaded { get; set; }
    }
}