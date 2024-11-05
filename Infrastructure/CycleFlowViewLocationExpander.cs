using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework;
using Nop.Plugin.Misc.CycleFlow.Constant;


namespace Nop.Plugin.Misc.CycleFlow.Infrastructure
{
    public class CycleFlowViewLocationExpander : IViewLocationExpander
    {
        private static string CycleFlow_System_OutputDir => SystemDefaults.PluginOutputDir;
        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
        }


        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName != "Admin" && context.AreaName != SystemDefaults.POS_AREA_NAME)
            {
                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    viewLocations = new[] {
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/Shared/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                        .Concat(viewLocations);
                }
            }
            else if (context.AreaName == SystemDefaults.POS_AREA_NAME)
            {
                viewLocations = new[] {
                        $"/Plugins/{CycleFlow_System_OutputDir}/Areas/Pos/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Areas/Pos/Views/Shared/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/Shared/{{0}}.cshtml",
                        $"/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                        $"/Areas/Admin/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }
            else if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                        $"/Plugins/{CycleFlow_System_OutputDir}/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Areas/Admin/Views/Shared/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/{CycleFlow_System_OutputDir}/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}
