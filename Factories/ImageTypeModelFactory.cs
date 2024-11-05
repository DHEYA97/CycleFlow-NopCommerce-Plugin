using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Services;

namespace Nop.Plugin.Misc.POSSystem.Areas.Admin.Factories
{
    public class ImageTypeModelFactory : IImageTypeModelFactory
    {
        #region Fields
        private readonly IImageTypeService _imageTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IHtmlFormatter _htmlFormatter;
        #endregion

        #region Ctor
        public ImageTypeModelFactory(
            IImageTypeService imageTypeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IHtmlFormatter htmlFormatter)
        {
            _imageTypeService = imageTypeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _htmlFormatter = htmlFormatter;
        }
        #endregion

        #region Methods
        public async Task<ImageTypeListModel> PrepareImageTypeListModelAsync(ImageTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var imageTypes = await _imageTypeService.GetAllImageTypesAsync(showHidden: true, name: searchModel.SearchName, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            var model = await new ImageTypeListModel().PrepareToGridAsync(searchModel, imageTypes, () =>
            {
                return imageTypes.SelectAwait(async imageType =>
                {
                    var imageTypeModel = imageType.ToModel<ImageTypeModel>();
                    return imageTypeModel;
                });
            });
            return model;
        }

        public async Task<ImageTypeModel> PrepareImageTypeModelAsync(ImageTypeModel model, ImageType imageType, bool excludeProperties = false)
        {
            Func<ImageTypeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (imageType != null)
            {
                if (model == null)
                {
                    model = imageType.ToModel<ImageTypeModel>();
                }

                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(imageType, entity => entity.Name, languageId, false, false);
                };
            }

            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        public Task<ImageTypeSearchModel> PrepareImageTypeSearchModelAsync(ImageTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            // Prepare parameters
            searchModel.SetGridPageSize();
            return Task.FromResult(searchModel);
        }
        #endregion
    }
}
