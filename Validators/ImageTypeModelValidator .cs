using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.CycleFlow.Validators
{
    public partial class ImageTypeModelValidator : BaseNopValidator<ImageTypeModel> 
    {
        public ImageTypeModelValidator(ILocalizationService localizationService, IImageTypeService imageTypeService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).Must((model, cancellation) =>
            {
                bool exists = imageTypeService.IsImageTypeNameFoundAsync(model.Name, model.Id).Result;
                return !exists;
            }).WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.ImageType.Name.Unique"));

            SetDatabaseValidationRules<ImageType>(mappingEntityAccessor);
        }
    }
}
