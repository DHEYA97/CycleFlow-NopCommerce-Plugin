using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting
{
    public record CycleFlowSettingSearchModel : BaseSearchModel
    {
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchName")]
        public string SearchName { get; set; }
        #endregion
    }
}
