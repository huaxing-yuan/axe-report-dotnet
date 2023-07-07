# Axe HtmlReport
Runs accessibility test on the given page and coverts axe-core report to comprehensive html reports


## Example: Runs Analyze and Generate Report
This example creates a HtmlReportBuilder, runs Analyze using Axe.Core.Selenium and Generate a HtmlReport use default Settings:

```csharp
var builder = new HtmlReportBuilder().WithSelenium(driver);
var result = builder.Convert(builder.Analyze());
var fileName = builder.Export(result);
```
