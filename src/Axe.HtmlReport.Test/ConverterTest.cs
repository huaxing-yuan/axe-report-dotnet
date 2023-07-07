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
        public void Test1()
        {
            //Execute mon test automatis�
            using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
            driver.Navigate().GoToUrl("https://www.axa.fr");
            var cookie = new WebElementDescription(driver)
            {
                Id = "footer_tc_privacy_button"
            };
            if(cookie.Exists(5))
            {
                cookie.Click();
            }

            var sw = Stopwatch.StartNew();
            //Effectuer une analyse d'accessibilit�
            var builder = new HtmlReportBuilder().WithSelenium(driver);
            var result = builder.Convert(builder.Analyze());
            var fileName = builder.Export(result);
            Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }
    }
}