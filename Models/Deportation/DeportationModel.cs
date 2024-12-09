using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public partial record DeportationModel : BaseNopEntityModel
    {
        #region Ctor
        public DeportationModel()
        {
            ProductOrderItem = new List<ProductOrderItemModel>();
            ImageType = new List<ImageTypeModel>();
            AllDeportation = new List<AllDeportationModel>();
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
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.OrderItemCount")]
        public int OrderItemCount { get; set; }

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
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ReturnStepName")]
        public string? ReturnStepName { get; set; }

        public string? Note { get; set; }

        public IList<ProductOrderItemModel> ProductOrderItem { get; set; }
        public IList<ImageTypeModel>? ImageType { get; set; }
        public IList<AllDeportationModel> AllDeportation { get; set; }
        #endregion
        #region Nested Classes
        public partial record ProductOrderItemModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string Sku { get; set; }

            public string PictureThumbnailUrl { get; set; }
            public int Quantity { get; set; }
        }
        public partial record ImageTypeModel : BaseNopEntityModel
        {

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ReturnStep.Picture")]
            public int? ImageTypeId { get; set; }
            [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ReturnStep.Name")]
            public string? ImageTypeName { get; set; }
            public string? ImageTypeUrl { get; set; }
        }
        #endregion
    }
}
