using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.POSSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface ICycleFlowSettingService
    {
        Task<IPagedList<OrderStatusSorting>> SearchCycleFlowSettingAsync(
             string orderStatusName = null,
              IList<int> posUserIds = null,
             IList<int> storeIds = null,
             int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertCycleFlowSettingAsync(CycleFlowSettingModel model);
        Task<OrderStatusSorting> GetOrderStatusSortingByIdAsync(int orderStatusSortingId);
        Task UpdateCycleFlowSettingAsync(CycleFlowSettingModel model);
        Task DeleteCycleFlowSettingAsync(OrderStatusSorting model);
        Task<IList<(string, string)>> GetFirstOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false);
        Task<IList<(string, string, int)>> GetNextOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false);
        Task<IList<(string, string, int)>> GetReturnOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false);
        Task<IList<int>> GetAllOrderCurrentSelectedImageTypeAsync(int posUserId, int orderStatusId);
        Task<bool> EnableIsFirstStepAsync(int posUserId, int currentId = 0);
        Task<bool> EnableIsLastStepAsync(int posUserId, int currentId = 0);
        Task<int?> GetNextStepByFirstStepAsync(int firstStep,int posUserId);
        Task<bool> IsCurrentOrderStatesExsistInSortingAsync(int orderStateId, int posUserId, int currentId = 0);
        Task<bool> IsNextOrderStatesExsistInSortingAsync(int orderStateId, int posUserId, bool IsLastStep = false, int currentId = 0);
        Task<(string, bool)> CheckOrderStatusSequenceAsync(int posUserId);
        Task<Customer> GetCustomerByOrderStatusIdAsync(int posUserId,int orderStateId);
        Task<OrderStatusSorting> GetFirstStepInPosUserAsync(int posUserId);
        Task NotificationPosUserAsync();
        Task<int?> GetReturnStepByCurentStepAsync(int statusId, int posUserId);
        Task<bool> IsFirstStepInSortingByStatusIdAsync(int statusId, int posUserId);
        Task<bool> IsLastStepInSortingByStatusIdAsync(int statusId, int posUserId);
    }
}
