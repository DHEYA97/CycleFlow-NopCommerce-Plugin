using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.OrderStatus
{
    public record OrderStatusSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.SearchName")]
        public string SearchName { get; set; }
        #endregion
    }
}
