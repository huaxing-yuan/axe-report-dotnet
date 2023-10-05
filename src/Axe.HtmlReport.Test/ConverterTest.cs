using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Axe.HtmlReport.Selenium;
using OpenQA.Selenium;
using System.Diagnostics;

namespace Axe.HtmlReport.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AuditAndExport()
        {
            //Execute mon test automatisé
            using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);
            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/");

            var sw = Stopwatch.StartNew();
            //Effectuer une analyse d'accessibilité
            var builder = new HtmlReportBuilder().WithSelenium(driver);
            var result = builder.Convert(builder.Analyze());
            var fileName = builder.Export(result);
            Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }
    }
}