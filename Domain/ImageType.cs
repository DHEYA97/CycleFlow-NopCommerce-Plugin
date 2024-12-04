using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{

    public class ImageType : BaseCycleFlowEntity, ILocalizedEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }
        public bool Deleted { get; set; }
    }
}