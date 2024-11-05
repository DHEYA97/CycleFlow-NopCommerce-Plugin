using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
namespace Nop.Plugin.Misc.CycleFlow.Validators
{
    public partial class OrderStatusModelValidator : BaseNopValidator<OrderStatusModel>
    {
        public OrderStatusModelValidator(ILocalizationService localizationService, IOrderStatusService orderStatusService, IMappingEntityAccessor mappingEntityAccessor)
        {
           
            RuleFor(x => x.Name).Must((model, cancellation) =>
            {
                bool exists = orderStatusService.IsOrderStatesNameFoundAsync(model.Name,model.Id).Result;
                return !exists;
            }).WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatus.Name.Unique"));
            SetDatabaseValidationRules<OrderStatus>(mappingEntityAccessor);
        }
    }
}
