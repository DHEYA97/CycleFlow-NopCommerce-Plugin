using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public record DeportationSearchModel : BaseSearchModel
    {
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.DeportationModel.Fields.SearchName")]
        public string SearchName { get; set; }
        #endregion
    }
}