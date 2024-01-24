using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    internal class AxeRgaaMapping
    {
        public Dictionary<string, IEnumerable<string>> Mapping { get; } = new Dictionary<string, IEnumerable<string>>()
        {
            {"area-alt",                new string[] { "1.1"} },
            {"aria-allowed-attr",       new string[] { "1.1" } },
            {"aria-hidden-body",        new string[] { "10.8" } },
            {"aria-hidden-focus",       new string[] { "10.8"} },
            {"aria-input-field-name",   new string[] { "11.1" } },
            {"aria-progressbar-name",   new string[] { "7.5" } },
            {"aria-required-attr",      new string[] { "9.1", "11.10" } },
            {"aria-roles",              new string[] { "1.1", "1.2", "1.3", "1.5", "1.6", "1.9", "5.3", "5.6", "5.7", "5.8", "7.5", "9.1", "9.3", "11.4", "11.5", "11.8" } },
            {"", new string[] { } },
            {"", new string[] { } },
            {"", new string[] { } },
            {"", new string[] { } },
            {"", new string[] { } },
            {"", new string[] { } },
        };
    }
}
