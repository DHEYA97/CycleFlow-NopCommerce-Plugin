using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.ImageType
{
    public record ImageTypeModel : BaseNopEntityModel, ILocalizedModel<ImageTypeLocalizedModel>
    {
        public ImageTypeModel()
        {
            Locales = new List<ImageTypeLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.ImageType.Fields.Name")]
        public string Name { get; set; }

        public IList<ImageTypeLocalizedModel> Locales { get; set; }
    }

    public record ImageTypeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.ImageType.Fields.Name")]
        public string? Name { get; set; }
    }
}
