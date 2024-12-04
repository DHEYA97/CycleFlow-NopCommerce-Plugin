using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public record DeportationModel : BaseNopEntityModel
    {
        #region Ctor
        public DeportationModel()
        {
            ImageTypeId = new List<int>();
            ImageTypeName = new List<string>();
            ProductOrderItem = new List<ProductOrderItemModel>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.NopStoreId")]
        public int NopStoreId { get; set; }
        public string? StoreName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.PosUserId")]
        public int PosUserId { get; set; }
        public string? PosUserName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.OrderDate")]
        public DateTime OrderDate { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.OrderStatusId")]
        public int OrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.CurrentOrderStatusName")]
        public string? CurrentOrderStatusName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.NextOrderStatus")]
        public int NextOrderStatusId { get; set; }
        public string? NextOrderStatusName { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.CustomerId")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public IList<int>? ImageTypeId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ImageTypeName")]
        public IList<string>? ImageTypeName { get; set; }
        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.IsEnableSendToClient")]
        public bool IsEnableSendToClient { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ClientSmsTemplate")]
        public int? ClientSmsTemplateId { get; set; }
        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.IsEnableSendToUser")]
        public bool IsEnableSendToUser { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.UserSmsTemplate")]
        public int? UserSmsTemplateId { get; set; }

        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.IsEnableReturn")]
        public bool IsEnableReturn { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ReturnStep")]
        public int? ReturnStepId { get; set; }

        public IList<ProductOrderItemModel> ProductOrderItem { get; set; }

        #endregion
        #region Nested Classes
        public partial record ProductOrderItemModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string Sku { get; set; }

            public string PictureThumbnailUrl { get; set; }
        }
        #endregion
    }
}
