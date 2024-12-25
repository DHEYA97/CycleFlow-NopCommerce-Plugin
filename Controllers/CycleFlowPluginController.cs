using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class CycleFlowPluginController : BaseAdminController
    {
        public async Task<IActionResult> Configure()
        {
            return View("~/Plugins/Misc.CycleFlow/Views/Configure.cshtml");
        }
    }
}
