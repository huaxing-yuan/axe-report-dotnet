using Deque.AxeCore.Commons;
using OpenQA.Selenium;
using SkiaSharp;
using System.Drawing;
using System.Xml.Linq;

namespace Axe.HtmlReport.Selenium
{
    public static class SeleniumHelper
    {
        /// <summary>
        /// Initializes the builder contexte with a Selenium WebDriver.
        /// WebDriver is used to take screenshots and other necessary manipulations to enrich report content.
        /// </summary>
        /// <param name="builder">HtmlReportBuilder object</param>
        /// <param name="driver">WebDriver object</param>
        /// <returns>The HtmlReportBuilder</returns>
        public static HtmlReportBuilder WithSelenium(this HtmlReportBuilder builder, WebDriver driver)
        {
            builder.GetScreenshot = (node, options) => ScreenShot(driver, node, options);
            builder.Analyze = () => Analyze(builder, driver);
            return builder;
        }

        private static AxeResult Analyze(HtmlReportBuilder builder, WebDriver driver)
        {
            Deque.AxeCore.Selenium.AxeBuilder axeBuilder = new Deque.AxeCore.Selenium.AxeBuilder(driver);
            if (builder.Options.Tags.Any())
            {
                axeBuilder.WithTags(builder.Options.Tags.ToArray());
            }
            var result = axeBuilder.Analyze();
            return result;
        }

        /// <summary>
        /// Takes the screenshot. this function will be called by <see cref="HtmlReportBuilder.GetScreenshot" delegation/>
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="node">Node in AxeResult</param>
        /// <param name="options"><see cref="HtmlReportOptions"/> providing options for screenshot.</param>
        /// <returns></returns>
        private static byte[] ScreenShot(WebDriver driver, AxeResultNode node, HtmlReportOptions options)
        {
            var selectors = node.Target.FrameShadowSelectors;
            IWebElement? element = null;
            if (selectors.Count > 1)
            {
                if (selectors.Any((IList<string> shadowSelectors) => shadowSelectors.Count > 1))
                {
                    //it contains shadow dom iFrame
                    throw new NotImplementedException("Shadow Dom element not yet implemented.");
                }
                else
                {
                    //it contains iFrame
                    var iFrameSelector = node.Target.FrameSelectors.ToArray();

                    //TODO: Unable to locate and screenshot elements inside frames.
                    //At current state, 
                    element = driver.FindElement(By.CssSelector(iFrameSelector[0]));
                    /*
                    foreach(var s in iFrameSelector)
                    {
                        element = driver.FindElement(By.CssSelector(s));
                        if(element.TagName == "iframe")
                        {
                            driver.SwitchTo().Frame(element);
                        }
                        else
                        {
                            element = driver.FindElement(By.CssSelector(s));
                        }
                    }*/
                }

            }
            else
            {
                var cssSelector = node.Target?.Selector;
                var xPath = node.XPath?.Selector;

                //find given element
                element = driver.FindElement(By.CssSelector(cssSelector));
                if (element == null)
                {
                    element = driver.FindElement(By.XPath(xPath));
                }
            }

            if (element != null && element is WebElement we)
            {
                try
                {
                    var screenshot = options.UseAdvancedScreenshot ? AdvancedScreenshot(driver, we, options) : we.GetScreenshot().AsByteArray;
                    driver.SwitchTo().DefaultContent(); //goes back to default context (leaving iframes)
                    return screenshot;
                }
                catch
                {
                    //in some cases (hidden element, 0 height element, etc. the screeshot is not possible, leave these cases behind.
                    return new byte[0];
                }
            }
            else
            {
                driver.SwitchTo().DefaultContent();
                throw new WebDriverException("The element can not be converted to type WebElement for screenshot.");
            }


        }

        private static byte[] AdvancedScreenshot(WebDriver driver, WebElement element, HtmlReportOptions options)
        {
            BringToView(element, driver);
            var imageViewPort = driver.GetScreenshot();
            var windowSize = driver.Manage().Window.Size;
            var locatable = (ILocatable)element;

            var location = locatable.Coordinates.LocationInViewport; //location and size are for 100% dpi
            var size = element.Size;
            if (size.Height != 0 && size.Width != 0)
            {
                var screenshot = MarkOnImage(imageViewPort, location, size, options);
                return screenshot;
            }
            return new byte[0];
        }

        /// <summary>
        /// Bring the element to viewport so that it can be captured by screenshot.
        /// </summary>
        /// <param name="element">The element should be taken into viewport</param>
        /// <param name="driver">WebDriver instance</param>
        private static void BringToView(WebElement element, WebDriver driver)
        {
            driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        //When using advanced screenshot, Use SkiaSharp to draw the element in question on the screenshot of containing view port.
        private static byte[] MarkOnImage(Screenshot imageViewPort, Point location, Size size, HtmlReportOptions options)
        {
            SKColor color = new SKColor(options.HighlightColor.R, options.HighlightColor.G, options.HighlightColor.B);
            using SKBitmap bitmap = SKBitmap.Decode(imageViewPort.AsByteArray);
            using SKCanvas canvas = new SKCanvas(bitmap);
            canvas.DrawRect(location.X, location.Y,
                size.Width, size.Height,
                new SKPaint()
                {
                    Color = color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = options.HighlightThickness
                });
            using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}