using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    /// <summary>
    /// Represents the complete results of an axe scan, including results from all rules and nodes included in the scan. Enhanced with screenshots.
    /// </summary>
    public class AxeEnhancedResult
    {

        public AxeEnhancedResult(AxeResult result, HtmlReportBuilder htmlReportBuilder)
        {
            Violations = GetViolations(result, htmlReportBuilder);
            Passes = GetPasses(result, htmlReportBuilder);
            Incomplete = GetIncomplete(result, htmlReportBuilder);
            Inapplicable = GetInapplicable(result, htmlReportBuilder);
            TestEngine = result.TestEngine;
            TestRunner = result.TestRunner;
            Timestamp = result.Timestamp;
            Url = result.Url;
            TestEnvironment = result.TestEnvironment;
        }

        private AxeResultEnhancedItem[] GetInapplicable(AxeResult result, HtmlReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> inapplicable = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotInapplicable;
            foreach(var i in result.Inapplicable)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                inapplicable.Add(item);
            }
            return inapplicable.ToArray();
        }

        private AxeResultEnhancedItem[] GetIncomplete(AxeResult result, HtmlReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> incomplete = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotIncomplete;
            foreach(var i in result.Incomplete)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                incomplete.Add(item);
            }
            return incomplete.ToArray();
        }

        private AxeResultEnhancedItem[] GetPasses(AxeResult result, HtmlReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> passes = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotPasses;
            foreach(var i in result.Passes)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                passes.Add(item);
            }
            return passes.ToArray();
        }

        private AxeResultEnhancedItem[] GetViolations(AxeResult result, HtmlReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> violations = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotViolations;
            foreach(var i in result.Violations)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                violations.Add(item);
            }

            return violations.ToArray();
        }

        private AxeResultEnhancedNode[] GetEnhancedNodes(AxeResultNode[] nodes, HtmlReportBuilder htmlReportBuilder, bool takeScreenshot)
        {
            List<AxeResultEnhancedNode> axeResultNodeEnhanceds = new List<AxeResultEnhancedNode>();
            foreach(var n in nodes)
            {
                AxeResultEnhancedNode axeResultNodeEnhanced = new AxeResultEnhancedNode(n);
                if (takeScreenshot)
                {
                    axeResultNodeEnhanced.Screenshot = htmlReportBuilder.GetScreenshot(n, htmlReportBuilder.Options);
                }
                axeResultNodeEnhanceds.Add(axeResultNodeEnhanced);
            }
            return axeResultNodeEnhanceds.ToArray();
        }

        /// <summary>
        /// These results indicate what elements failed the rules.
        /// </summary>
        public AxeResultEnhancedItem[] Violations { get; internal set; }

        /// <summary>
        /// These results indicate what elements passed the rules.
        /// </summary>
        public AxeResultEnhancedItem[] Passes { get; internal set; }

        /// <summary>
        /// These results indicate which rules did not run because no matching content was found on the page. 
        /// For example, with no video, those rules won't run.
        /// </summary>
        public AxeResultEnhancedItem[] Inapplicable { get; internal set; }

        /// <summary>
        /// These results were aborted and require further testing. 
        /// This can happen either because of technical restrictions to what the rule can test, or because a javascript error occurred.
        /// </summary>
        public AxeResultEnhancedItem[] Incomplete { get; internal set; }

        /// <summary>
        /// The date and time that analysis was completed.
        /// </summary>
        public DateTimeOffset? Timestamp { get; private set; }

        /// <summary>
        /// Information about the current browser or node application that ran the audit.
        /// </summary>
        public AxeTestEnvironment TestEnvironment { get; private set; }

        /// <summary>
        /// The runner that ran the audit.
        /// </summary>
        public AxeTestRunner TestRunner { get; set; }

        /// <summary>
        /// The URL of the page that was tested.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The application that ran the audit.
        /// </summary>
        public AxeTestEngine TestEngine { get; private set; }

    }
}
