using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.OrderStatus
{
    public record OrderStatusModel : BaseNopEntityModel, ILocalizedModel<OrderStatusLocalizedModel>
    {
        public OrderStatusModel()
        {
            Locales = new List<OrderStatusLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Description")]
        public string? Description { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.IsActive")]
        public bool IsActive { get; set; }
        public IList<OrderStatusLocalizedModel> Locales { get; set; }
    }

    public record OrderStatusLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Name")]
        public string? Name { get; set; }

        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Description")]
        public string? Description { get; set; }
    }
}
