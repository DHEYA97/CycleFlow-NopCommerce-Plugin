using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public record DeportationSearchModel : BaseSearchModel
    {
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.DeportationModel.Fields.OrderNumber")]
        public int OrderNumber { get; set; }
        public bool JustShowByCustomer { get; set; }
        public bool JustLastStepOrder { get; set; }
        #endregion
    }
}