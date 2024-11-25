﻿using Nop.Core;
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
        Task<IList<(string, string)>> GetNextOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false);
        Task<IList<int>> GetAllOrderCurrentSelectedImageTypeAsync(int orderStatusId);
        Task<PosUser> GetPosUser(int orderStatusId);
        Task<bool> EnableIsFirstStepAsync();
        Task<bool> EnableIsLastStepAsync();
        Task<int> GetNextStepByFirstStep(int firstStep);
    }
}
