using Deque.AxeCore.Commons;
using OpenQA.Selenium;
using SkiaSharp;
using System.Drawing;

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
            var cssSelector = node.Target?.Selector;
            var xPath = node.XPath?.Selector;

            //find given element
            var element = driver.FindElement(By.CssSelector(cssSelector));
            if (element == null)
            {
                element = driver.FindElement(By.XPath(xPath));
            }
            if(element is WebElement we)
            {
                //advanced screenshot: take the screenshot of the whole viewport, and marks the location of the element.
                //standard screenshot: take the screenshot of the element only.
                return options.UseAdvancedScreenshot ? AdvancedScreenshot(driver, we, options) : we.GetScreenshot().AsByteArray;
            }
            else
            {
                throw new WebDriverException("The element can not be converted to type WebElement for screenshot.");
            }            
        }

        private static byte[] AdvancedScreenshot(WebDriver driver, WebElement element, HtmlReportOptions options)
        {
            BringToView(element, driver);
            var imageViewPort = driver.GetScreenshot();

            var locatable = (ILocatable)element;
            var location = locatable.Coordinates.LocationInViewport;
            var size = element.Size;
            var screenshot = MarkOnImage(imageViewPort, location, size, options);
            return screenshot;
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
                new SKPaint() {
                    Color = color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = options.HighlightThickness});
            using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}