using Deque.AxeCore.Commons;
using Microsoft.Playwright;
using OpenQA.Selenium;

namespace Axe.HtmlReport
{
    /// <summary>
    /// Classes for generating HTML report from given Axe-Core results
    /// </summary>
    public class HtmlReportBuilder
    {
        public HtmlReportOptions Options { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder"/> class using default options.
        /// </summary>
        public HtmlReportBuilder()
        {
            Options = new HtmlReportOptions()
            {
                OutputFolder = null,
                OutputFormat = OutputFormat.Html,
                UseAdvancedScreenshot = true
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder"/> class using the specified options.
        /// </summary>
        /// <param name="options"></param>
        public HtmlReportBuilder(HtmlReportOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// The object holding the WebDriver instance.
        /// </summary>
        private object? driver;

        /// <summary>
        /// Use Selenium WebDriver
        /// </summary>
        /// <param name="driver">WebDriver object</param>
        /// <returns></returns>
        public HtmlReportBuilder WithDriver(IWebDriver driver)
        {
            this.driver = driver;
            return this;
        }

        /// <summary>
        /// Run Axe-Core analysis and returns analysis results
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public AxeResult Analyze()
        {
            if(this.driver is IWebDriver webDriver)
            {
               Deque.AxeCore.Selenium.AxeBuilder axeBuilder = new Deque.AxeCore.Selenium.AxeBuilder(webDriver);
               var results = axeBuilder.Analyze();
               return results;
            }
            else
            {
               
               throw new System.Exception("No WebDriver is provided found.");
            }
        }

        public string ConvertToHtml(AxeResult result)
        {
        }
    }
}