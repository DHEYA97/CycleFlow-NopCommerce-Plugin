using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{

    public class OrderStatusPermissionMapping : BaseEntity
    {
        public int NopStoreId { get; set; }
        public int PosUserId { get; set; }
        public int OrderStatusId { get; set; }
        public int CustomerId { get; set; }
    }
}