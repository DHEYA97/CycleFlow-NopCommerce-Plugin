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
            AvailableClientSmsTemplates = new List<SelectListItem>();
            AvailableUserSmsTemplates = new List<SelectListItem>();
            AvailableReturnOrderStatus = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatus")]
        public int CurrentOrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName")]
        public string? CurrentOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableCurrentOrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Store")]
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Customer")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public IList<SelectListItem> AvailableCustomers { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUser")]
        public int PosUserId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName")]
        public string? PosUserName { get; set; }
        public IList<SelectListItem> AvailablePosUsers { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatus")]
        public int NextOrderStatusId { get; set; }
        public string? NextOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableNextOrderStatus { get; set; }
        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SelectedImageType")]
        public IList<int>? SelectedImageTypeIds { get; set; }
        public IList<SelectListItem> AvailableImageTypes { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsFirstStep")]
        public bool IsFirstStep { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsLastStep")]
        public bool IsLastStep { get; set; }
        public bool EnableIsFirstStep { get; set; }
        public bool EnableIsLastStep { get; set; }

        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ClientSmsTemplate")]
        public int? ClientSmsTemplateId { get; set; }
        public IList<SelectListItem> AvailableClientSmsTemplates { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToClient")]
        public bool IsEnableSendToClient { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.UserSmsTemplate")]
        public int? UserSmsTemplateId { get; set; }
        public IList<SelectListItem> AvailableUserSmsTemplates { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToUser")]
        public bool IsEnableSendToUser { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableReturn")]
        public bool IsEnableReturn { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ReturnStep")]
        public int? ReturnStepId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ReturnOrderStatusName")]
        public string? ReturnOrderStatusName { get; set; }
        public IList<SelectListItem> AvailableReturnOrderStatus { get; set; }

    }
}