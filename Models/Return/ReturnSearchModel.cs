using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
namespace Nop.Plugin.Misc.CycleFlow.Models.Return
{
    public record ReturnSearchModel : BaseSearchModel
    {
        #region Ctor
        public ReturnSearchModel()
        {
            SearchCustomerIds = new List<int>();
            AvailableCustomers = new List<SelectListItem>();
            SearchYearIds = new List<int>();
            AvailableYears = new List<SelectListItem>();
            SearchMonthIds = new List<int>();
            AvailableMonths = new List<SelectListItem>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchCustomer")]
        public IList<int> SearchCustomerIds { get; set; }
        public IList<SelectListItem> AvailableCustomers { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchCustomer")]
        public IList<int> SearchYearIds { get; set; }
        public IList<SelectListItem> AvailableYears { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchCustomer")]
        public IList<int> SearchMonthIds { get; set; }
        public IList<SelectListItem> AvailableMonths { get; set; }
        #endregion
    }
}