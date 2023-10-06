using Deque.AxeCore.Commons;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Axe.HtmlReport.Selenium")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Axe.HtmlReport.Playwright")]
namespace Axe.HtmlReport
{
    /// <summary>
    /// Classes for generating HTML report from given Axe-Core results
    /// </summary>
    public class HtmlReportBuilder
    {
        public HtmlReportOptions Options { get; set; }

        public bool CanGetScreenshot => GetScreenshot != EmptyGetScreenshot && GetScreenshot != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder"/> class using default options.
        /// </summary>
        public HtmlReportBuilder()
        {
            Analyze = EmptyAnalyzeDelegate;
            GetScreenshot = EmptyGetScreenshot;
            Options = new HtmlReportOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder"/> class using the specified options.
        /// </summary>
        /// <param name="options"></param>
        public HtmlReportBuilder(HtmlReportOptions options)
        {
            Analyze = EmptyAnalyzeDelegate;
            GetScreenshot = EmptyGetScreenshot;
            Options = options;
        }

        public HtmlReportBuilder WithOptions(HtmlReportOptions options)
        {
            Options = options;
            return this;
        }

        private byte[]? EmptyGetScreenshot(AxeResultNode node, HtmlReportOptions options)
        {
            return null;
        }


        /// <summary>
        /// Converts the given Axe-Core results to HTML report using actual configuration.
        /// </summary>
        /// <param name="result">The AxeResult to be converted.</param>
        public AxeEnhancedResult Convert(AxeResult result)
        {
            //Add screenshots to AxeResult, then converted to HTML according to the option
            AxeEnhancedResult resultEnhanced = new AxeEnhancedResult(result, this);
            return resultEnhanced;
        }

        /// <summary>
        /// Export Enhanced AxeResult (with Screenshots) to HTML file.
        /// </summary>
        /// <param name="result">Processed AxeResult with screenshot</param>
        public string Export(AxeEnhancedResult result)
        {
            string path = Options.OutputFolder ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            string violations = GenerateSection(result.Violations, path);
            string passes = GenerateSection(result.Passes, path);
            string incomplete = GenerateSection(result.Incomplete, path);
            string inapplicable = GenerateSection(result.Inapplicable, path);
            string html = GetHtmlTemplate("index.html");
            html = html.Replace("{{Title}}", Options.Title)
                .Replace("{{PageUrl}}", result.Url)
                .Replace("{{TimeStamp}}", result.AxeResult.Timestamp.ToString())
                .Replace("{{Score}}", result.Score.ToString())
                .Replace("{{ScoreColor}}", result.ScoreForegroundColor)
                .Replace("{{ScoreBackgroundColor}}", result.ScoreBackgroundColor)
                .Replace("{{ScoreRotation}}", result.ScoreRotation.ToString())
                .Replace("{{Violations}}", violations)
                .Replace("{{Passed}}", passes)
                .Replace("{{Incomplete}}", incomplete)
                .Replace("{{ViolationRules}}", result.Violations.Count().ToString())
                .Replace("{{ViolationNodes}}", result.Violations.Sum(x => x.Nodes.Count()).ToString())
                .Replace("{{NonApplicable}}", inapplicable);
            string filename = Path.Combine(path, "index.html");
            File.WriteAllText(filename, html);
            //TODO: according to the option, make report as zip file or leave it as folder
            return filename;
        }

        private string GetHtmlTemplate(string filename)
        {
            //read content from Embeded Resource `Assets/index.html`
            var assembly = typeof(HtmlReportBuilder).Assembly;
            var resourceName = assembly.GetName().Name + ".Assets." + filename;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"Unable to find resource {resourceName} in assembly {assembly.FullName}");
            return new StreamReader(stream).ReadToEnd();
        }


        private string GenerateSection(AxeResultEnhancedItem[] items, string path)
        {
            StringBuilder overall = new StringBuilder();
            var template = GetHtmlTemplate("rule-part.html");
            foreach (var item in items)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var node in item.Nodes)
                {
                    var nodeTemplate = GetHtmlTemplate("node-part.html");
                    var cssSelector = node.Node.Target;
                    var xpath = node.Node.XPath;
                    var display = node.Screenshot != null ? "block" : "none";
                    string filename = string.Empty;
                    if (node.Screenshot != null)
                    {
                        Guid id = Guid.NewGuid();
                        filename = id.ToString() + ".png";
                        File.WriteAllBytes(Path.Combine(path, filename), node.Screenshot);
                    }

                    sb.Append(
                        nodeTemplate.Replace("{{Selector}}", cssSelector.ToString())
                        .Replace("{{HtmlCode}}", HttpUtility.HtmlEncode(node.Node.Html))
                        .Replace("{{Display}}", display)
                        .Replace("{{Filename}}", filename)
                        .Replace("{{AnyChecks}}", ToHtml(node.Node.Any, "Any Checks"))
                        .Replace("{{AllChecks}}", ToHtml(node.Node.Any, "All Checks"))
                        .Replace("{{NoneChecks}}", ToHtml(node.Node.Any, "None Checks"))
                        );
                }
                overall.Append(
                    template.Replace("{{RuleId}}", item.Item.Id)
                    .Replace("{{RuleTags}}", string.Join(' ', item.Item.Tags.Select(x => $"<div class='regularition'>{x}</div>")))
                    .Replace("{{Impact}}", item.Item.GetImpact())
                    .Replace("{{Description}}", HttpUtility.HtmlEncode(item.Item.Description))
                    .Replace("{{Help}}", HttpUtility.HtmlEncode(item.Item.Help))
                    .Replace("{{HelpUrl}}", item.Item.HelpUrl)
                    .Replace("{{RuleNodeCount}}", item.Nodes.Length.ToString())
                    .Replace("{{RuleNodes}}", sb.ToString()));
            }
            return overall.ToString();
        }

        private string ToHtml(AxeResultCheck[]? any, string label)
        {
            if (any != null)
            {
                var sb = new StringBuilder();
                foreach (AxeResultCheck item in any)
                {
                    sb.AppendLine("Check Id:" + item.Id);
                    sb.AppendLine("Impact:" + item.Impact);
                    sb.AppendLine("Message:" + item.Message);
                    if(item.Data != null)
                    {
                        sb.AppendLine("Data:" + item.Data.ToString());
                    }
                }
                return WebUtility.HtmlEncode(sb.ToString());
            }
            return "No rules audited for " + label;
        }

        public GetScreenshotDelegate GetScreenshot { get; internal set; }
        public AnalyzeDelegate Analyze { get; internal set; }

        public delegate byte[]? GetScreenshotDelegate(AxeResultNode node, HtmlReportOptions options);
        public delegate AxeResult AnalyzeDelegate();


        /// <summary>
        /// This is the default Emoty delegate for Analyze. without calling WithSelenium or WithPlaywright, analyze will be empty.
        /// </summary>
        /// <returns></returns>
        private AxeResult EmptyAnalyzeDelegate()
        {
            throw new System.NotImplementedException("Test Framework Not specified. Please pass .WithSelenium(driver) or .WithPlaywright(context) before ");
        }

    }
}