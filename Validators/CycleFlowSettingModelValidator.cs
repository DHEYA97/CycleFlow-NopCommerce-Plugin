using DocumentFormat.OpenXml.EMMA;
using FluentValidation;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.Accounting.Services;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Validators
{
    public partial class CycleFlowSettingModelValidator : BaseNopValidator<CycleFlowSettingModel> 
    {
        public CycleFlowSettingModelValidator(ILocalizationService localizationService, ICycleFlowSettingService cycleFlowSettingService)
        {
            RuleFor(x => x.StoreId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Store.Required"));

            RuleFor(x => x.CustomerId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Customer.Required"));

            RuleFor(x => x.PosUserId)
               .NotEqual(0).NotEmpty().GreaterThan(0)
               .WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.PosUser.Required"));

            RuleFor(x => x.CurrentOrderStatusId).MustAwait(async (model, cans) =>
            {
                if (model.CurrentOrderStatusId <= 0)
                {
                    return false;
                }

                if (model != null)
                {
                    if (model.PosUserId > 0)
                    {
                        var exsist = await cycleFlowSettingService.IsCurrentOrderStatesExsistInSortingAsync(model.CurrentOrderStatusId,model.PosUserId,model.Id > 0 ? model.Id : 0);
                        return !exsist;
                    }
                }
                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.CurrentOrderStatusId.Found"));

            RuleFor(x => x.NextOrderStatusId).MustAwait(async (model, cans) =>
            {
                if (model.NextOrderStatusId <= 0)
                {
                    return false;
                }

                if (model != null)
                {
                    if (model.PosUserId > 0)
                    {
                        var exsist = await cycleFlowSettingService.IsNextOrderStatesExsistInSortingAsync(model.NextOrderStatusId, model.PosUserId, model.Id > 0 ? model.Id : 0);
                        return !exsist;
                    }
                }
                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.CurrentOrderStatusId.Found"));


        }
    }
}
