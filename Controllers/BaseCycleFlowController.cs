using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class BaseCycleFlowController : BaseAdminController
    {
        #region Methods
        public async Task<IActionResult> CheckPermissionAndRoleAsync(PermissionRecord permission, string role,bool isDataTables = false)
        {
            var permissionService  = EngineContext.Current.Resolve<IPermissionService>();
            var customerService  = EngineContext.Current.Resolve<ICustomerService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var currentCustomer = await workContext.GetCurrentCustomerAsync();
            if (currentCustomer == null)
            {
                return !isDataTables ? AccessDeniedView() : await AccessDeniedDataTablesJson();
            }
            if (!await permissionService.AuthorizeAsync(permission,currentCustomer))
            {
                return !isDataTables ? AccessDeniedView() : await AccessDeniedDataTablesJson();
            }
            if (!await customerService.IsInCustomerRoleAsync(currentCustomer, role))
            {
                return !isDataTables ? AccessDeniedView() : await AccessDeniedDataTablesJson();
            }

            return null;
        }
        #endregion
        #region Utilities

        protected async Task<JsonResult> AccessDeniedDataTablesJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            return ErrorJson(await localizationService.GetResourceAsync("Admin.AccessDenied.Description"));
        }
        #endregion

    }
}
