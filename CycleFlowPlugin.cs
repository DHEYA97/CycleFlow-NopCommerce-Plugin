using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.POSSystem;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.CycleFlow
{
    public class CycleFlowPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields
        protected readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        protected readonly IPermissionService _permissionService;
        protected readonly ICustomerService _customerService;
        protected readonly WidgetSettings _widgetSettings;
        protected readonly ISettingService _settingService;
        #endregion
        #region Ctor
        public CycleFlowPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionService,
            ICustomerService customerService,
            WidgetSettings widgetSettings,
            ISettingService settingService
            )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _permissionService = permissionService;
            _customerService = customerService;
            _widgetSettings = widgetSettings;
            _settingService = settingService;
        }
        #endregion
        #region Methods
        public bool HideInWidgetList => false;
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.OrderDetailsButtons)
                return SystemDefaults.ORDER_CYCLE_FLOW;

            return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult(new List<string>()
            {
                AdminWidgetZones.OrderDetailsButtons
            } as IList<string>);
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CycleFlowPlugin/Configure";
        }
        public override async Task InstallAsync()
        {
            await _permissionService.InstallPermissionsAsync(new CycleFlowPluginPermissionProvider());
            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(SystemDefaults.SYSTEM_NAME))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(SystemDefaults.SYSTEM_NAME);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await InsertInitialDataAsync();
            await base.InstallAsync();
        }
        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await InsertInitialDataAsync();
            await UpdatePermissionDataAsync();
        }
        public override async Task UninstallAsync()
        {
            CycleFlowPluginPermissionProvider permissionProvider = new CycleFlowPluginPermissionProvider();

            foreach (var permation in permissionProvider.GetPermissions())
            {
                await DeleteRoleDataAsync(permation);
            }

            await _localizationService.DeleteLocaleResourceAsync("Admin.Plugin.Misc.CycleFlow");
            await _localizationService.DeleteLocaleResourceAsync("Nop.Plugin.Misc.CycleFlow");
            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menueItem = new SiteMapNode()
            {
                SystemName = SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME,
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlow").Result,
                ControllerName = null,
                ActionName = null,
                IconClass = "fas fa-stream",
                Visible = true,
            };
            var DeportationChildNode = new SiteMapNode()
            {
                SystemName = "Deportation",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation").Result,
                ControllerName = "Deportation",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var AllOrderChildNode = new SiteMapNode()
            {
                SystemName = "Deportation",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.AllOrder").Result,
                ControllerName = "Deportation",
                ActionName = "AllOrder",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var LastStepChildNode = new SiteMapNode()
            {
                SystemName = "Deportation",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.LastStepOrder").Result,
                ControllerName = "Deportation",
                ActionName = "LastStepOrder",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var ReturnChildNode = new SiteMapNode()
            {
                SystemName = "Return",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Return").Result,
                ControllerName = "Return",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var OrderStatusChildNode = new SiteMapNode()
            {
                SystemName = "OrderStatus",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatus").Result,
                ControllerName = "OrderStatus",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var CycleFlowSettingChildNode = new SiteMapNode()
            {
                SystemName = "CycleFlowSetting",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting").Result,
                ControllerName = "CycleFlowSetting",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            var CheckPosOrderStatusChildNode = new SiteMapNode()
            {
                SystemName = "CheckPosOrderStatus",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CheckPosOrderStatus").Result,
                ControllerName = "CheckPosOrderStatus",
                ActionName = "List",
                IconClass = "far fa-dot-circle",
                Visible = true,
            };
            if (await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.AccessToCycleFlowPluginMenu))
            {
                rootNode.ChildNodes.Add(menueItem);
            }
            if (await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
            {
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(OrderStatusChildNode);
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(CycleFlowSettingChildNode);
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(CheckPosOrderStatusChildNode);
            }
            if (await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
            {
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(DeportationChildNode);
            }
            if (await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin))
            {
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(AllOrderChildNode);
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(LastStepChildNode);
                rootNode.ChildNodes.FirstOrDefault(s => s.SystemName.Equals(SystemDefaults.CYCLE_FLOW_SITE_MAP_NODE_SYSTEM_NAME))?.ChildNodes.Add(ReturnChildNode);
            }
        }
        #endregion
        #region Utilites
        protected async Task DeleteRoleDataAsync(PermissionRecord permission)
        {

            var manageAccountingAccountPermission = (await _permissionService.GetAllPermissionRecordsAsync())
                                                    .FirstOrDefault(x => x.SystemName == permission.SystemName);
            if (manageAccountingAccountPermission != null)
            {
                var listMappingCustomerRolePermissionRecord = await _permissionService.GetMappingByPermissionRecordIdAsync(manageAccountingAccountPermission.Id);
                foreach (var mappingCustomerPermissionRecord in listMappingCustomerRolePermissionRecord)
                    await _permissionService.DeletePermissionRecordCustomerRoleMappingAsync(
                        mappingCustomerPermissionRecord.PermissionRecordId,
                        mappingCustomerPermissionRecord.CustomerRoleId);

                await _permissionService.DeletePermissionRecordAsync(manageAccountingAccountPermission);
            }

        }
        public async Task UpdatePermissionDataAsync()
        {
            var permissionProvider = new CycleFlowPluginPermissionProvider();

            var installedPermissions = await _permissionService.GetAllPermissionRecordsAsync();
            var defaultPermissionMapping = permissionProvider.GetDefaultPermissions();
            var roles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
            foreach (var permission in permissionProvider.GetPermissions())
            {
                if (!installedPermissions.Select(t => t.SystemName)?.Contains(permission.SystemName) ?? false)
                {
                    await _permissionService.InsertPermissionRecordAsync(permission);

                    foreach (var role in roles)
                    {
                        if (
                            defaultPermissionMapping.Select(t => t.systemRoleName).Contains(role.SystemName)
                            &&
                            (defaultPermissionMapping.Where(t => t.systemRoleName == role.SystemName)?.SelectMany(t => t.permissions.Select(t => t.SystemName))?.Contains(permission.SystemName) ?? false)
                            )
                        {
                            await _permissionService.InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping { CustomerRoleId = role.Id, PermissionRecordId = permission.Id });
                        }
                    }
                }
            }
        }
       protected async Task InsertInitialDataAsync()
        {
            
            var cycleFlowRole = await _customerService.GetCustomerRoleBySystemNameAsync(SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (cycleFlowRole == null)
            {
                cycleFlowRole = new CustomerRole()
                {
                    IsSystemRole = true,
                    SystemName = SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,
                    Name = SystemDefaults.CYCLE_FLOW_ROLE_NAME,
                    Active = true,
                };
                await _customerService.InsertCustomerRoleAsync(cycleFlowRole);
            }
            
        }
        
        #endregion
    }
}
