using Nop.Plugin.Misc.POSSystem.Domains;
namespace Nop.Plugin.Misc.CycleFlow.Event
{
    public partial class PosOrderEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="PosOrder">Order</param>
        public PosOrderEvent(PosOrder posOrder)
        {
            PosOrder = posOrder;
        }

        /// <summary>
        /// PosOrder
        /// </summary>
        public PosOrder PosOrder { get; }
    }
}
