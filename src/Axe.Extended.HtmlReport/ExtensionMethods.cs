using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public static class ExtensionMethods
    {
        public static string? GetImpact(this AxeResultItem item)
        {
            
            var impact = item.Impact;
            impact ??= item.Nodes.FirstOrDefault()?.Impact;
            impact ??= item.Nodes.FirstOrDefault()?.All.FirstOrDefault()?.Impact;
            impact ??= item.Nodes.FirstOrDefault()?.Any.FirstOrDefault()?.Impact;
            impact ??= item.Nodes.FirstOrDefault()?.None.FirstOrDefault()?.Impact;
            return impact;
        }

        internal static IEnumerable<string> GetTagsByRule(this string ruleId)
        {

            if (AxeRgaaMapping.Mapping.ContainsKey(ruleId))
            {
                return AxeRgaaMapping.Mapping[ruleId];
            }
            else
            {
                return Array.Empty<string>();
            }
        }


    }
}
