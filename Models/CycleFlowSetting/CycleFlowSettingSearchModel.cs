using Microsoft.AspNetCore.Mvc.Rendering;
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
        #region Ctor
        public CycleFlowSettingSearchModel()
        {
            SearchPosUsersIds = new List<int>();
            SearchStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailablePosUsers = new List<SelectListItem>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchName")]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchPosUser")]
        public IList<int> SearchPosUsersIds { get; set; }

        [NopResourceDisplayName("Admin.Plugin.CycleFlow.CycleFlowSetting.Fields.SearchStore")]
        public IList<int> SearchStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailablePosUsers { get; set; }
        #endregion
    }
}
