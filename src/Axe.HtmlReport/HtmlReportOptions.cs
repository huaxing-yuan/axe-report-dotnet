using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    /// <summary>
    /// Options of generating HTML report
    /// </summary>
    public sealed class HtmlReportOptions
    {
        /// <summary>
        /// Gets or sets the output format of the HTML report: Zip Archive or a folder containing HTML report and all resources.
        /// </summary>
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Html;
    
        /// <summary>
        /// Gets or sets the output folder of the HTML report. If not set, the report will be generated in default temprary folder.
        /// </summary>
        public string? OutputFolder { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if advanced screenshot should be used. Default is true.
        /// </summary>
        /// <remarks>
        /// Advanced screenshot is a screenshot of the whole viewport, and the element in question is highlighted.
        /// Otherwise, a screenshot of the element in question is taken.
        /// Advanced screenshot marks the element in question more clearly, but it takes more space (images are larger).
        /// </remarks>
        public bool UseAdvancedScreenshot { get; set; } = true;
    }
}
