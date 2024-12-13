using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
namespace Nop.Plugin.Misc.CycleFlow.Validators
{
    public partial class DeportationModelValidator : BaseNopValidator<DeportationModel>
    {
        public DeportationModelValidator(ILocalizationService localizationService, IOrderStatusService orderStatusService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.NopStoreId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.Store.Required"));

            RuleFor(x => x.CustomerId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.Customer.Required"));

            RuleFor(x => x.PosUserId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.PosUser.Required"));
            
            RuleFor(x => x.OrderId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.Order.Required"));
            
            RuleFor(x => x.OrderStatusId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.OrderStatus.Required"));

            RuleFor(x => x.ClientSmsTemplateId)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.ClientSmsTemplateId.MustSelect"))
                .When(x => x.IsEnableSendToClient);

            RuleFor(x => x.UserSmsTemplateId)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.UserSmsTemplateId.MustSelect"))
                .When(x => x.IsEnableSendToUser);

            RuleForEach(x => x.ImageType)
                .Must(image => image.PictureId.HasValue && image.PictureId > 0)
                .When(x => x.ImageType != null && x.ImageType.Count > 0)
                .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.Picture.MustNotEmpty"));

            RuleFor(x => x.ImageType)
                .Must(imageList => imageList != null && imageList.All(img => img.PictureId.HasValue && img.PictureId > 0))
                .When(x => x.ImageType?.Count > 0)
                .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.ImageType.MustNotEmpty"));

        }
    }
}
