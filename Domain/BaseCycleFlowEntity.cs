using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.Accounting.Domain;
using Nop.Plugin.Misc.Accounting.Models;
using Nop.Plugin.Misc.Accounting.Services;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{
    public partial class BaseCycleFlowEntity : BaseEntity
    {
        public int InsertedByUser { set; get; }
        public DateTime? InsertionDate { set; get; }
        public int UpdatedByUser { set; get; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdatingDate { set; get; }
        public virtual async Task<SaveModelResultModel> SetBaseInfoAsync<T>(IWorkContext workContext) where T : BaseCycleFlowEntity
        {
           
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            int customer_id = (await workContext.GetCurrentCustomerAsync()).Id;
            DateTime dateTime = DateTime.UtcNow;

            if (Id == 0)
            {
                InsertedByUser = customer_id;
                InsertionDate = dateTime;
                UpdatedByUser = customer_id;
                UpdatingDate = dateTime;
            }
            else
            {
                var Repository = EngineContext.Current.Resolve<IRepository<T>>();
                T old_entity = await Repository.GetByIdAsync(Id);

                if (old_entity != null)
                {
                    InsertedByUser = old_entity.InsertedByUser;
                    InsertionDate = old_entity.InsertionDate;
                    UpdatedByUser = customer_id;
                    UpdatingDate = dateTime;
                }
                else
                {
                    return new SaveModelResultModel()
                    {
                        success = false,
                        message = await localizationService.GetResourceAsync("Nop.Plugin.CycleFlow.Common.EntityNotFound"),
                        hasError = false,
                        errors = new List<SaveModelErrorModel> { new SaveModelErrorModel() { Name = "None", Errors = new List<SaveModelErrorItem>() { new SaveModelErrorItem() { ErrorMessage = await localizationService.GetResourceAsync("Nop.Plugin.CycleFlow.Common.EntityNotFound") } } } }
                    };
                }
            }

            return new SaveModelResultModel()
            {
                success = true,
            };
        }
    }
}
