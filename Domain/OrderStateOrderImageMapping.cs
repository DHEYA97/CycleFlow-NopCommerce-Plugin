﻿using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Domain
{
    public class OrderStateOrderImageMapping : BaseCycleFlowEntity
    {
        public int NopStoreId { get; set; }
        public int PosUserId { get; set; }
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public int CustomerId { get; set; }
        public int ImageTypeId { get; set; }
        public int PictureId { get; set; }
        public int OrderStateOrderMappingId { get; set; }
    }
}
