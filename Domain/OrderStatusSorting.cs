using Nop.Core;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{
    public class OrderStatusSorting : BaseEntity
    {
        public int NopStoreId { get; set; }
        public int PosUserId { get; set; }
        public int OrderStatusId { get; set; }
        public int NextStep { get; set; }
        public bool IsFirstStep { get; set; }
        public bool IsLastStep { get; set; }
    }
}
