using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IImageTypeModelFactory
    {
        Task<ImageTypeSearchModel> PrepareImageTypeSearchModelAsync(ImageTypeSearchModel searchModel);

        Task<ImageTypeListModel> PrepareImageTypeListModelAsync(ImageTypeSearchModel searchModel);

        Task<ImageTypeModel> PrepareImageTypeModelAsync(ImageTypeModel model, ImageType imageType, bool excludeProperties = false);
    }
}
