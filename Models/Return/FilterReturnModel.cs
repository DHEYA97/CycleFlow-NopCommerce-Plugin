using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Return
{
    public partial class FilterReturnModel : BaseEntity
    {
        #region Ctor
        public FilterReturnModel()
        {
            OrderDetail = new List<OrderDetailModel>();
        }
        #endregion
        #region Properties
        public int PosUserId { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.PosUserName")]
        public string PosUserName { get; set; }
        
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.CustomerId")]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.year")]
        public int Year { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.Month")]
        public int Month { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.ReturnCount")]
        public int ReturnCount { get; set; }
        public IList<OrderDetailModel> OrderDetail { get; set; }
        #endregion
        #region Nested Classes
        public partial record OrderDetailModel : BaseNopEntityModel
        {

            [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.ReturnStep.OrderId")]
            public int OrderId { get; set; }
            [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.ReturnStep.OrderStatusId")]
            public int OrderStatusId { get; set; }
        }
        #endregion
    }
}
