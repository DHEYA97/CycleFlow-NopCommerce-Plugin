using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public partial record OrderStatusPictureSearchModel : BaseSearchModel
    {
        public int PosUserId { get; set; }
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
    }
}