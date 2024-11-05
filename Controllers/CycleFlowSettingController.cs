using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CycleFlowSettingController : BasePluginController
    {
        [HttpGet]
        public async Task<IActionResult> CreateCycleFlow(int Id)
        {
            await Task.CompletedTask;
            return View(Id);
        }
    }
}
