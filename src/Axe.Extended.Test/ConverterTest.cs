using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Axe.Extended.HtmlReport;
using Axe.Extended.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using Platform = AxaFrance.WebEngine.Platform;

namespace Axe.Extended.Test
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
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch { }

                var sw = Stopwatch.StartNew();

                //Effectuer une analyse d'accessibilité
                var filename = new PageReportBuilder()
                    .WithOptions(new PageReportOptions()
                    {
                        HighlightColor = Color.LimeGreen,
                        HighlightThickness = 5,
                        ScoringMode = ScoringMode.Weighted,
                        Tags = Array.Empty<string>()
                    })
                    .WithSelenium(driver)
                    .Build()
                    .Export();
                Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
        }

        [Test]
        public void CustomRulesRgaa()
        {
            //Execute mon test automatisé
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                //driver.Navigate().GoToUrl("https://squizlabs.github.io/HTML_CodeSniffer/Standards/WCAG2/Examples/G141.Fail.html");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch { }

                var sw = Stopwatch.StartNew();

                //Effectuer une analyse d'accessibilité
                var filename = new PageReportBuilder()
                    .WithOptions(new PageReportOptions()
                    {
                        HighlightColor = Color.LimeGreen,
                        HighlightThickness = 5,
                        ScoringMode = ScoringMode.Weighted,
                        //Tags = new string[] { "rgaa*" },
                    })
                    .WithSelenium(driver)
                    .WithRgaaExtension()
                    .Build()
                    .Export();
                Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
        }

        [Test]
        public void MySuperUnitTest()
        {
            using (var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                /*
                 * Vos 200 lignes de script automatisé existant
                */
                var report = new PageReportBuilder()    //Instancier un builder
                    .WithSelenium(driver)               //passer le driver Selenium
                    .Build()                            //analyser le page
                    .Export();                          //générer le rapport

                Process.Start(new ProcessStartInfo(report) { UseShellExecute = true });
            }
        }

        [Test]
        public void AuditForApplication()
        {
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                var defaultOptions = new PageReportOptions()
                {
                    HighlightColor = Color.OrangeRed,
                    HighlightThickness = 5,
                    UseAdvancedScreenshot = true,
                    ScoringMode = ScoringMode.NonWeighted,
                    //Tags = Array.Empty<string>(),
                    Title = "AXA.FR",
                    OutputFormat = OutputFormat.Zip
                };
                var builder = new OverallReportBuilder().WithDefaultOptions(defaultOptions);
                //Analyze first page
                driver.Navigate().GoToUrl("https://www.axa.fr/");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch { }
                builder.WithSelenium(driver, "Main Page").Build();

                //Analyze second page
                driver.Navigate().GoToUrl("https://www.axa.fr/pro.html");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch { }
                builder.WithSelenium(driver, "Pro").Build();

                //Analyze third page
                driver.Navigate().GoToUrl("https://www.axa.fr/pro/services-assistance.html");
                builder.WithSelenium(driver, "Assistance").Build();

                //Demarche Inondation
                driver.Navigate().GoToUrl("https://www.axa.fr/assurance-habitation/demarches-inondation.html");
                builder.WithSelenium(driver, "Inondation").Build();

                driver.Navigate().GoToUrl("https://www.axa.fr/pro/devis-assurance-professionnelle.html");
                builder.WithSelenium(driver, "Devis Pro").Build();

                driver.Navigate().GoToUrl("https://www.axa.fr/compte-bancaire.html");
                builder.WithSelenium(driver, "Compte Bancaire").Build();

                string report = builder.Build().Export();
                Process.Start(new ProcessStartInfo(report) { UseShellExecute = true });
            }
        }

        [Test]
        public void AuditWCAG2ExportZip()
        {
            //Execute mon test automatisé
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr/");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch { }

                var zipReport = new PageReportBuilder()
                    .WithOptions(new PageReportOptions()
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
}