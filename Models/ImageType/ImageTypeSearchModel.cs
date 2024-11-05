using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.ImageType
{
    public record ImageTypeSearchModel : BaseSearchModel
    {
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.ImageType.Fields.SearchName")]
        public string SearchName { get; set; }
        #endregion
    }
}