﻿using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{
    public class OrderStateOrderMapping : BaseCycleFlowEntity
    {
        public int NopStoreId { get; set; }
        public int PosUserId { get; set; }
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public int CustomerId { get; set; }
        public string? Note { get; set; }
        public bool? IsReturn { get; set; }
        public int? ReturnOrderStatusId { get; set; }
    }
}
