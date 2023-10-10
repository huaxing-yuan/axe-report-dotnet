using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Axe.HtmlReport.Selenium;
using OpenQA.Selenium;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace Axe.HtmlReport.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AuditAndExportHtml()
        {
            //Execute mon test automatisé
            using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);
            driver.Navigate().GoToUrl("https://www.axa.fr/");
            try
            {
                driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
            }
            catch { }

            var sw = Stopwatch.StartNew();

            //Effectuer une analyse d'accessibilité
            var filename = new HtmlReportBuilder()
                .WithOptions(new HtmlReportOptions()
                {
                    HighlightColor = Color.LimeGreen,
                    HighlightThickness = 5,
                    ScoringMode = ScoringMode.Weighted
                })
                .WithSelenium(driver)
                .Build()
                .Export();
            Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
            Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
        }

        [Test]
        public void AuditWCAG2ExportZip()
        {
            //Execute mon test automatisé
            using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);
            driver.Navigate().GoToUrl("https://www.axa.fr/");
            try
            {
                driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
            }
            catch { }

            var zipReport = new HtmlReportBuilder()
                .WithOptions(new HtmlReportOptions()
                {
                    ScreenshotPasses = false,
                    UseAdvancedScreenshot = false,
                    OutputFormat = OutputFormat.Zip
                })
                .WithSelenium(driver)
                .Build()
                .Export();

        }
    }
}