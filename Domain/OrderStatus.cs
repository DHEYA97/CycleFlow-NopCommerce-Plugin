using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{

    public class OrderStatus : BaseCycleFlowEntity, ILocalizedEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}