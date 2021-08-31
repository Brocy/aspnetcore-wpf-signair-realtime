using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using RealtimeSurvey.Hubs.Presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeSurvey.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PresenceTracker _presenceTracker;

        public IndexModel(ILogger<IndexModel> logger, PresenceTracker presenceTracker)
        {
            _logger = logger;
            _presenceTracker = presenceTracker;
        }

        public void OnGet()
        {

        }

        public PartialViewResult OnGetDeviceListViewPartial()
        {
            var users = _presenceTracker.GetOnlineUsers().Result?.ToList();
            if (null == users)
                users = new List<string>();
            return Partial("_DeviceListViewPartial", users);
        }
    }
}
