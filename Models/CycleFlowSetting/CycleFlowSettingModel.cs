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
            AvailableCustomers = new List<SelectListItem>();
            AvailablePosUsers = new List<SelectListItem>();
            AvailableCurrentOrderStatus = new List<SelectListItem>();
            AvailableNextOrderStatus = new List<SelectListItem>();
            AvailableImageTypes = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusId")]
        public int CurrentOrderStatusId { get; set; }
        public string? CurrentOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableCurrentOrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreId")]
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CustomerId")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public IList<SelectListItem> AvailableCustomers { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserId")]
        public int PosUserId { get; set; }
        public string? PosUserName { get; set; }
        public IList<SelectListItem> AvailablePosUsers { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusId")]
        public int NextOrderStatusId { get; set; }
        public string? NextOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableNextOrderStatus { get; set; }
        
        public IList<int>? SelectedImageTypeIds { get; set; }
        public IList<SelectListItem> AvailableImageTypes { get; set; }
        public bool IsFirstStep { get; set; }
        public bool IsLastStep { get; set; }
        public bool EnableIsFirstStep { get; set; }
        public bool EnableIsLastStep { get; set; }
    }
}