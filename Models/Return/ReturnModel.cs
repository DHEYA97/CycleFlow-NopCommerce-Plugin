using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CycleFlow.Models.Return
{
    public partial record ReturnModel : BaseNopEntityModel
    {
        #region Ctor
        public ReturnModel()
        {
            AllReturn = new List<AllReturnModel>();
        }
        #endregion
        #region Properties
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.CustomerId")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.year")]
        public int Year { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.Month")]
        public int Month { get; set; }
        [NopResourceDisplayName("Admin.Plugin.Misc.CycleFlow.Return.Fields.ReturnCount")]
        public int ReturnCount { get; set; }
        public IList<AllReturnModel> AllReturn { get; set; }
        #endregion
        #region Nested Classes
        public partial record AllReturnModel : BaseNopEntityModel
        {
            #region Ctor
            public AllReturnModel()
            {
                ImageType = new List<(string, string)?>();
            }
            #endregion
            #region Properties
            public int OrderStateOrderMappingId { get; set; }
            public int OrderId { get; set; }
            public string PosUserName { get; set; }
            public string PosStoreName { get; set; }
            public string ReturnStatusName { get; set; }
            public string ReturnFromStatusName { get; set; }
            public string CustomerReturnName { get; set; }
            public string CustomerReturnFromName { get; set; }
            public DateTime ReturnDate { get; set; }
            public string? Note { get; set; }
            public IList<(string, string)?> ImageType { get; set; }
            #endregion

        }
        #endregion
    }
}
