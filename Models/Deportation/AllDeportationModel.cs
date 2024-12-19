using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.CycleFlow.Models.Deportation
{
    
    public partial record AllDeportationModel : BaseNopEntityModel
    {
        #region Ctor
        public AllDeportationModel()
        {
            ImageType = new List<(string,string)?>();
        }
        #endregion
        #region Properties
        public string StatusName { get; set; }
        public string? NextStatusName { get; set; }
        public DateTime? DeportationDate { get; set; }
        public string? Note { get; set; }
        public bool? IsReturn { get; set; }
        public IList<(string,string)?> ImageType { get; set; }
        #endregion

    }
}
