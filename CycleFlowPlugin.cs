using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.POSSystem;
using Nop.Services.Cms;
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
        #endregion
        #region Ctor
        public CycleFlowPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionService,
            ICustomerService customerService
            )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _permissionService = permissionService;
            _customerService = customerService;
        }
        #endregion
        #region Methods
        public bool HideInWidgetList => false;
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult(new List<string>()
            {

            } as IList<string>);
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CycleFlowPlugin/Configure";
        }
        public override async Task InstallAsync()
        {
            await _permissionService.InstallPermissionsAsync(new CycleFlowPluginPermissionProvider());
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
                SystemName = "CycleFlow",
                Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlow").Result,
                ControllerName = null,
                ActionName = null,
                IconClass = "fas fa-stream",
                Visible = true,
                ChildNodes = new List<SiteMapNode>
                {
                    new SiteMapNode()
                    {
                        SystemName = "OrderStatus",
                        Title = _localizationService.GetResourceAsync( "Nop.Plugin.Misc.CycleFlow.OrderStatus").Result,
                        ControllerName = "OrderStatus",
                        ActionName = "List",
                        IconClass = "far fa-dot-circle",
                        Visible = true,
                    },
                    new SiteMapNode()
                    {
                        SystemName = "ImageType",
                        Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.ImageType").Result,
                        ControllerName = "ImageType",
                        ActionName = "List",
                        IconClass = "far fa-dot-circle",
                        Visible = true,  
                    },
                    new SiteMapNode()
                    {
                        SystemName = "CycleFlowSetting",
                        Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CycleFlowSetting").Result,
                        ControllerName = "CycleFlowSetting",
                        ActionName = "List",
                        IconClass = "far fa-dot-circle",
                        Visible = true,
                    },
                     new SiteMapNode()
                    {
                        SystemName = "CheckPosOrderStatus",
                        Title = _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.CheckPosOrderStatus").Result,
                        ControllerName = "CheckPosOrderStatus",
                        ActionName = "List",
                        IconClass = "far fa-dot-circle",
                        Visible = true,
                    }

                }
            };
            if (await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.AccessToCycleFlowPluginMenu))
            {
                rootNode.ChildNodes.Add(menueItem);
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
            var lang_list = await _languageService.GetAllLanguagesAsync();
            foreach (var lang in lang_list)
            {
                await _localizationService.AddOrUpdateLocaleResourceAsync(PluginResources(lang.UniqueSeoCode), languageId: lang.Id);
            }

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
        protected IDictionary<string, string> PluginResources(string LangCode)
        {
            switch (LangCode.ToLower())
            {
                case "en":
                    return PluginEnglishResources();
                case "ar":
                    return PluginArabicResources();

                default: return PluginEnglishResources();
            }
        }
        protected IDictionary<string, string> PluginEnglishResources()
        {
            return new Dictionary<string, string>
            {
                #region English
                ["Nop.Plugin.Misc.CycleFlow.CycleFlow"] = "CycleFlow",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Name"] = "Name",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Description"] = "Description",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.IsActive"] = "Is Active",
                ["Nop.Plugin.Misc.CycleFlow.OrderStatus.Name.Unique"] = "Name Must Be Unique",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.SearchName"] = "Search Name",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Title"] = "Order Status",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.EditOrderStatusDetails"] = "Edit Order Status Details",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.BackToList"] = "Back To List",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.AddNew"] = "Add New Order Status",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Info"] = "Order Status Info",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Deleted"] = "Order Status Deleted",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Updated"] = "Order Status Updated",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Added"] = "Order Status Added",
                ["Nop.Plugin.Misc.CycleFlow.OrderStatus"] = "Order Status",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Fields.Name"] = "Name",
                ["Nop.Plugin.Misc.CycleFlow.ImageType.Name.Unique"] = "Name Must Be Unique",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Fields.SearchName"] = "Search Name",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Title"] = "Image Type",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.EditImageTypeDetails"] = "Edit Image Type Details",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.BackToList"] = "Back To List",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.AddNew"] = "Add New Image Type",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Info"] = "Image Type Info",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Deleted"] = "Image Type Deleted",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Updated"] = "Image Type Updated",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Added"] = "Image Type Added",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Manage"] = "Manage Cycle Flow",
                ["Nop.Plugin.Misc.CycleFlow.ImageType"] = "Image Type",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Title"] = "Cycle Flow Setting",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "Current Order Status Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreName"] = "Store Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "Pos User Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusName"] = "Next Order Status Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusId"] = "Current Order Status Id",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreId"] = "Search Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NopWarehouseId"] = "Store Id",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserId"] = "Pos User Id",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusId"] = "Next Order Status Id",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NopWarehouseName"] = "Warehouse Name",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting"] = "Cycle Flow Setting",
                ["Admin.Plugin.Misc.CycleFlow.Common.Select"] = "Select",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CheckResult"] = "CheckResult",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ClientSmsTemplateId"] = "Client Sms Template",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.UserSmsTemplateId"] = "User Sms Template",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Store.Required"] = "Store is required",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Customer.Required"] = "Customer is required",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.PosUser.Required"] = "POS User is required",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.CurrentOrderStatusId.Found"] = "Current Order Status already exists in the sorting list",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.NextOrderStatusId.Found"] = "Next Order Status already exists in the sorting list",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsFirstStep.Found"] = "The first step cannot be enabled for more than one status",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsLastStep.Found"] = "The last step cannot be enabled for more than one status",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.ClientSmsTemplateId.MustSelect"] = "Client SMS Template must be selected",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.UserSmsTemplateId.MustSelect"] = "User SMS Template must be selected",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsEnableSendToUser.MustTrue"] = "IsEnableSendToUser must be true if User Sms Template Id is set",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsEnableSendToClient.MustTrue"] = "IsEnableSendToClient must be true if Client Sms Template Id is set",
                ["Admin.Plugin.Misc.CycleFlow.CheckPosOrderStatus.Title"] = "Check POS Order Status",
                ["Admin.Plugin.Misc.CycleFlow.CheckPosOrderStatusModel.Fields.PosUserName"] = "POS User Name",
                ["Admin.Plugin.Misc.CycleFlow.ResultOfCheck"] = "Result Of Check",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Title"] = "Cycle Flow Setting",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreName"] = "Store Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "POS User Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "Current Order Status Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusName"] = "Next Order Status Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.EditOrderStatusDetails"] = "Edit Order Status Details",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.EditOrderStatusDetails"] = "Edit Order Status Details",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "Back to List",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.AddNew"] = "Add New",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "Back to List",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.EditOrderStatusDetails"] = "Edit Order Status Details",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "Back to List",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Info"] = "Info",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Send"] = "Send",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchPosUser"] = "SearchPosUser",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatus"] = "Current Order Status",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Store"] = "Store",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Customer"] = "Customer",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUser"] = "POS User",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatus"] = "Next Order Status",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SelectedImageType"] = "Selected Image Type",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsFirstStep"] = "Is First Step",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsLastStep"] = "Is Last Step",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ClientSmsTemplate"] = "Client Sms Template",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToClient"] = "Is Enable Send To Client",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.UserSmsTemplate"] = "User Sms Template",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToUser"] = "Is Enable Send To User",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchName"] = "Search by Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchPosUser"] = "Search POS User",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchStore"] = "Search Store",
                ["Nop.Plugin.Misc.CycleFlow.CheckPosOrderStatus"] = "Check Order Status ٍSorting Setting",
                ["Admin.Plugin.CycleFlow.CycleFlowSetting.Fields.SearchStore"] = "Search Store",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "Current Order Status Name",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "Pos User Name",

                #endregion
            };
        }

        protected IDictionary<string, string> PluginArabicResources()
        {
            return new Dictionary<string, string>
            {
                #region Arabic
                ["Nop.Plugin.Misc.CycleFlow.CycleFlow"] = "الدوره المستنديه",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Name"] = "اسم",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.Description"] = "الوصف",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.IsActive"] = "نشط",
                ["Nop.Plugin.Misc.CycleFlow.OrderStatus.Name.Unique"] = "يجب أن يكون الاسم فريدًا",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Fields.SearchName"] = "اسم البحث",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Title"] = "حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.EditOrderStatusDetails"] = "تعديل تفاصيل حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.BackToList"] = "العودة إلى القائمة",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.AddNew"] = "إضافة حالة طلب جديدة",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Info"] = "معلومات حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Deleted"] = "تم حذف حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Updated"] = "تم تحديث حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Added"] = "تم إضافة حالة الطلب",
                ["Nop.Plugin.Misc.CycleFlow.OrderStatus"] = "حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Fields.Name"] = "اسم",
                ["Nop.Plugin.Misc.CycleFlow.ImageType.Name.Unique"] = "يجب أن يكون الاسم فريدًا",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Fields.SearchName"] = "اسم البحث",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Title"] = "نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.EditImageTypeDetails"] = "تعديل تفاصيل نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.BackToList"] = "العودة إلى القائمة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.AddNew"] = "إضافة نوع صورة جديدة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Info"] = "معلومات نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Deleted"] = "تم حذف نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Updated"] = "تم تحديث نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Added"] = "تم إضافة نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Manage"] = "إدارة الدورة المستندية",
                ["Nop.Plugin.Misc.CycleFlow.ImageType"] = "نوع الصورة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Title"] = "إعدادات الدورة المستندية ",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "اسم حالة الطلب الحالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreName"] = "اسم المتجر",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "اسم مستخدم الـ POS",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusName"] = "اسم حالة الطلب التالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusId"] = "معرف حالة الطلب الحالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreId"] = "اسم البحث",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NopWarehouseId"] = "معرف المتجر",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserId"] = "معرف مستخدم الـ POS",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusId"] = "معرف حالة الطلب التالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NopWarehouseName"] = "اسم المستودع",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting"] = "إعدادات الدورة المستندية ",
                ["Admin.Plugin.Misc.CycleFlow.Common.Select"] = "اختيار",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CheckResult"] = "نتيجة الفحص",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ClientSmsTemplateId"] = "قالب الرسالة النصية للعميل",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.UserSmsTemplateId"] = "قالب الرسالة النصية للمستخدم",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Store.Required"] = "المتجر مطلوب",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.Customer.Required"] = "العميل مطلوب",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.PosUser.Required"] = "مستخدم الـ POS مطلوب",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.CurrentOrderStatusId.Found"] = "حالة الطلب الحالية موجودة بالفعل في قائمة التصفية",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.NextOrderStatusId.Found"] = "حالة الطلب التالية موجودة بالفعل في قائمة التصفية",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsFirstStep.Found"] = "لا يمكن تمكين الخطوة الأولى لأكثر من حالة واحدة",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsLastStep.Found"] = "لا يمكن تمكين الخطوة الأخيرة لأكثر من حالة واحدة",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.ClientSmsTemplateId.MustSelect"] = "يجب تحديد قالب الرسالة النصية للعميل",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.UserSmsTemplateId.MustSelect"] = "يجب تحديد قالب الرسالة النصية للمستخدم",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsEnableSendToUser.MustTrue"] = "يجب أن يكون IsEnableSendToUser صحيحًا إذا تم تعيين قالب الرسالة النصية للمستخدم",
                ["Nop.Plugin.Misc.CycleFlow.CycleFlowSetting.IsEnableSendToClient.MustTrue"] = "يجب أن يكون IsEnableSendToClient صحيحًا إذا تم تعيين قالب الرسالة النصية للعميل",
                ["Admin.Plugin.Misc.CycleFlow.CheckPosOrderStatus.Title"] = "تحقق من حالة الطلب في الـ POS",
                ["Admin.Plugin.Misc.CycleFlow.CheckPosOrderStatusModel.Fields.PosUserName"] = "اسم مستخدم الـ POS",
                ["Admin.Plugin.Misc.CycleFlow.ResultOfCheck"] = "نتيجة الفحص",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Title"] = "إعدادات الدورة المستندية ",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.StoreName"] = "اسم المتجر",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "اسم مستخدم الـ POS",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "اسم حالة الطلب الحالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatusName"] = "اسم حالة الطلب التالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.EditOrderStatusDetails"] = "تعديل تفاصيل حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.OrderStatus.EditOrderStatusDetails"] = "تعديل تفاصيل حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "العودة إلى القائمة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.AddNew"] = "إضافة جديدة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "العودة إلى القائمة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.EditOrderStatusDetails"] = "تعديل تفاصيل حالة الطلب",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.BackToList"] = "العودة إلى القائمة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Info"] = "معلومات",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Send"] = "إرسال",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatus"] = "حالة الطلب الحالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Store"] = "المتجر",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.Customer"] = "العميل",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUser"] = "مستخدم نقاط البيع",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.NextOrderStatus"] = "حالة الطلب التالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SelectedImageType"] = "نوع الصورة المحدد",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsFirstStep"] = "هل هذه هي الخطوة الأولى",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsLastStep"] = "هل هذه هي الخطوة الأخيرة",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.ClientSmsTemplate"] = "قالب رسالة العميل",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToClient"] = "تمكين إرسال إلى العميل",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.UserSmsTemplate"] = "قالب رسالة المستخدم",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.IsEnableSendToUser"] = "تمكين إرسال إلى المستخدم",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchName"] = "البحث باسم الحاله",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchPosUser"] = "بحث عن مستخدم نقاط البيع",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.SearchStore"] = "بحث عن المتجر",
                ["Nop.Plugin.Misc.CycleFlow.CheckPosOrderStatus"] = "تحقق من اعدادات الدوره المستنديه",
                ["Admin.Plugin.CycleFlow.CycleFlowSetting.Fields.SearchStore"] = "بحث عن المتجر",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.CurrentOrderStatusName"] = "حالة الطلب الحالية",
                ["Admin.Plugin.Misc.CycleFlow.CycleFlowSetting.Fields.PosUserName"] = "اسم نقطة البيع",

                #endregion
            };
        }
        #endregion
    }
}
