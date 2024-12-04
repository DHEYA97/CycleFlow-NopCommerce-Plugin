using Nop.Plugin.Misc.CycleFlow.Models.Deportation;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class DeportationModelFactory : IDeportationModelFactory
    {

       public Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel)
       {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            // Prepare parameters
            searchModel.SetGridPageSize();
            return Task.FromResult(searchModel);
       }
       public  Task<DeportationListModel> PrepareDeportationModelListModelAsync(DeportationSearchModel searchModel)
       {
            throw new NotImplementedException();
       }
    }
}
