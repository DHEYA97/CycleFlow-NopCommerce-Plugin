using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CheckPosOrderStatus;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class CheckPosOrderStatusModelFactory : ICheckPosOrderStatusModelFactory
    {
        #region Felid
        private readonly ILocalizationService _localizationService;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
        #endregion
        #region Ctor
        public CheckPosOrderStatusModelFactory(ILocalizationService localizationService,
            ICycleFlowSettingService cycleFlowSettingService,
            IPosUserService posUserService,
            ICustomerService customerService
            )
        {
            _localizationService = localizationService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _posUserService = posUserService;
            _customerService = customerService;
        }
        #endregion
        #region Methods
        public virtual async Task<CheckPosOrderStatusSearchModel> PrepareCheckPosOrderStatusSearchModelAsync(CheckPosOrderStatusSearchModel searchModel)
        {
            searchModel ??= new CheckPosOrderStatusSearchModel();

            await PreparePosUsersListAsync(searchModel.AvailablePosUsers);


            searchModel.SearchPosUsersIds
                = new List<int>() { 0 };
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public virtual async Task<CheckPosOrderStatusListModel> PrepareCheckPosOrderStatusListModelAsync(CheckPosOrderStatusSearchModel searchModel)
        {
            var posUsers = await _posUserService.SearchPosUserAsync(
               posUserIds: searchModel.SearchPosUsersIds
           );

            return await new CheckPosOrderStatusListModel().PrepareToGridAsync(searchModel, posUsers, () =>
            {
                return posUsers.SelectAwait(async checkPosOrderStatusModel =>
                {
                    return await PrepareCheckPosOrderStatusModelAsync(new CheckPosOrderStatusModel(), checkPosOrderStatusModel);
                });
            });
        }
        #endregion
        #region Utilite
        public async Task<CheckPosOrderStatusModel> PrepareCheckPosOrderStatusModelAsync(CheckPosOrderStatusModel model, PosUser posUser)
        {
            model.Id = posUser.Id;
            model.PosUserName = await _customerService.GetCustomerFullNameAsync(await _posUserService.GetUserByIdAsync(posUser.Id)) ?? string.Empty;
            //model.CheckResult = await _cycleFlowSettingService.CheckOrderStatusSequenceAsync(posUser.Id);
            return model;
        }
        public async Task PreparePosUsersListAsync(IList<SelectListItem> items)
        {

            var availablePosUsers = await (await _posUserService.GetPosUserListAsync()).Where(ps => ps.Active).ToListAsync();
            foreach (var user in availablePosUsers)
            {
                items.Add(new SelectListItem { Value = _posUserService.GetPosUserByNopCustomerIdAsync(user.Id).Result.Id.ToString(), Text = _customerService.GetCustomerFullNameAsync(user).Result.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        #endregion
    }
}
