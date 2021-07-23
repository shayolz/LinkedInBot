using LinkedInBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInBot.Services
{
    public class ActionService
    {
        private readonly AppSettings _config;


        public ActionService(AppSettings config)
        {
            _config = config;
        }

        public bool Login(LinkedinLogin login)
        {
            return false;
        }
    }
}
