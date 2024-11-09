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
    public record CycleFlowSettingModel : BaseNopEntityModel
    {
        public CycleFlowSettingModel()
        {
            SelectedImageTypeIds = new List<int>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailablePosUsers = new List<SelectListItem>();
            AvailableOrderStatus = new List<SelectListItem>();
            AvailableImageTypes = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusId")]
        public int CurrentOrderStatusId { get; set; }
        public string CurrentOrderStatusName { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreId")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NopWarehouseId")]
        public int NopWarehouseId { get; set; }
        public string NopWarehouseName { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserId")]
        public int PosUserId { get; set; }
        public string PosUserName { get; set; }
        public IList<SelectListItem> AvailablePosUsers { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusId")]
        public int NextOrderStatusId { get; set; }
        public string NextOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableOrderStatus { get; set; }
        
        public IList<int> SelectedImageTypeIds { get; set; }
        public IList<SelectListItem> AvailableImageTypes { get; set; }
    }
}