using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType; // تعديل النموذج ليكون متعلقًا بـ ImageType
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.CycleFlow.Validators
{
    public partial class ImageTypeModelValidator : BaseNopValidator<ImageTypeModel> // تغيير الكلاس إلى ImageTypeModelValidator
    {
        public ImageTypeModelValidator(ILocalizationService localizationService, IImageTypeService imageTypeService, IMappingEntityAccessor mappingEntityAccessor)
        {
            // إضافة قاعدة التحقق من أن الاسم فريد بالنسبة لـ ImageType
            RuleFor(x => x.Name).Must((model, cancellation) =>
            {
                bool exists = imageTypeService.IsImageTypeNameFoundAsync(model.Name, model.Id).Result;
                return !exists;
            }).WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.ImageType.Name.Unique"));

            // إعداد قواعد التحقق من القيم في قاعدة البيانات
            SetDatabaseValidationRules<ImageType>(mappingEntityAccessor);
        }
    }
}
