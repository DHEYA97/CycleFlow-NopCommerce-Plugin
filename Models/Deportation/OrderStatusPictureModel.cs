using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    public partial record OrderStatusPictureModel : BaseNopEntityModel
    {
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Deportation.Fields.ReturnStep.Picture")]
        public int? PictureId { get; set; }
        public string? PictureUrl { get; set; }
    }
}
