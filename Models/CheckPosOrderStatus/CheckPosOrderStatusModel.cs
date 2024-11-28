using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Models.CheckPosOrderStatus
{
    public record CheckPosOrderStatusModel : BaseNopEntityModel
    {
        public CheckPosOrderStatusModel()
        {
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName")]
        public string PosUserName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CheckResult")]
        public string CheckResult { get; set; }
    }
}