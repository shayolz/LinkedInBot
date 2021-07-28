using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInBot.Domain
{
    public class AppSettings
    {
        public List<LinkedinLogin> Accounts { get; set; }
        public int WaitTimeAfterEveryBehaviour { get; set; }
    }
}
