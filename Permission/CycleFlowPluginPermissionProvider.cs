using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Permission
{
    public partial class CycleFlowPluginPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord AccessToCycleFlowPluginMenu = new()
        {
            Name = "Access To CycleFlow Plugin Menu",
            SystemName = "AccessToCycleFlowPluginMenu",
            Category = "CycleFlow"
        };
        public static readonly PermissionRecord ManageCycleFlowPlugin = new()
        {
            Name = "Manage CycleFlow Plugin Accounts",
            SystemName = "ManageCycleFlowPluginAccount",
            Category = "CycleFlow"
        };
        public static readonly PermissionRecord DeportationCycleFlowPlugin = new()
        {
            Name = "Deportation CycleFlow",
            SystemName = "DeportationCycleFlow",
            Category = "CycleFlow"
        };
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessToCycleFlowPluginMenu,
                ManageCycleFlowPlugin,
            };
        }
        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new() { (NopCustomerDefaults.AdministratorsRoleName, GetPermissions().ToArray()) };
        }
    }
}
