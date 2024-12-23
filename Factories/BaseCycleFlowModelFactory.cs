using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class BaseCycleFlowModelFactory : IBaseCycleFlowModelFactory
    {
        #region Felid
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IPosUserService _posUserService;
        private readonly IPosStoreService _posStoreService;
        private readonly IStoreService _storeService;
        #endregion
        #region Ctor
        public BaseCycleFlowModelFactory(
            ICustomerService customerService,
            ILocalizationService localizationService,
            IPosUserService posUserService,
            IPosStoreService posStoreService,
            IStoreService storeService
            )
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _posUserService = posUserService;
            _posStoreService = posStoreService;
            _storeService = storeService;
        }
        #endregion
        #region Methods
        public async Task PreparePosUsersListAsync(IList<SelectListItem> items)
        {

            var availablePosUsers = await (await _posUserService.GetPosUserListAsync()).Where(ps => ps.Active).ToListAsync();
            foreach (var user in availablePosUsers)
            {
                items.Add(new SelectListItem { Value = _posUserService.GetPosUserByNopCustomerIdAsync(user.Id).Result.Id.ToString(), Text = _customerService.GetCustomerFullNameAsync(user).Result.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        public async Task PreparePosStoresAsync(IList<SelectListItem> items)
        {
            var availableStores = await (await _posStoreService.GetAllPosStoresAsync()).Where(ps => ps.StoreType != StoreTypes.Online).ToListAsync();
            foreach (var store in availableStores)
            {
                var nopStore = await _storeService.GetStoreByIdAsync(store.NopStoreId);
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = nopStore.Name });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        public async Task PrepareCustomerListAsync(IList<SelectListItem> items)
        {
            var cycleFowRole = await _customerService.GetCustomerRoleBySystemNameAsync(SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            var availableCustomer = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { cycleFowRole.Id }).Result.ToListAsync();
            foreach (var customer in availableCustomer)
            {
                items.Add(new SelectListItem { Value = customer.Id.ToString(), Text = await _customerService.GetCustomerFullNameAsync(customer) });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        #endregion
    }
}
