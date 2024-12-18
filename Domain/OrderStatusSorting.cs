using Nop.Core;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{
    public class OrderStatusSorting : BaseCycleFlowEntity
    {
        public int NopStoreId { get; set; }
        public int PosUserId { get; set; }
        public int OrderStatusId { get; set; }
        public int? NextStep { get; set; }
        public bool IsFirstStep { get; set; }
        public bool IsLastStep { get; set; }
        public bool IsEnableSendToClient { get; set; }
        public int? ClientSmsTemplateId { get; set; }
        public bool IsEnableSendToUser { get; set; }
        public int? UserSmsTemplateId { get; set; }
        public bool IsEnableReturn { get; set; }
        public int? ReturnStepId { get; set; }
    }
}
