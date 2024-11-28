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
    public record CheckPosOrderStatusSearchModel : BaseSearchModel
    {
        #region Ctor
        public CheckPosOrderStatusSearchModel()
        {
            SearchPosUsersIds = new List<int>();
            AvailablePosUsers = new List<SelectListItem>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchPosUser")]
        public IList<int> SearchPosUsersIds { get; set; }

        public IList<SelectListItem> AvailablePosUsers { get; set; }
        #endregion
    }
}
