using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    public class OverallReportBuilder
    {
        private PageReportOptions options;

        public OverallReportBuilder() {
            options = new PageReportOptions();
            PageBuilders = new List<PageReportBuilder>();
        }

        public OverallReportBuilder(PageReportOptions options)
        {
            this.options = options;
            PageBuilders = new List<PageReportBuilder>();
        }

        public OverallReportBuilder WithDefaultOptions(PageReportOptions options)
        {
            this.options = options;
            return this;
        }

        //Build OverallResult
        public OverallReportBuilder Build()
        {
            AxeOverallResult overallResult = new AxeOverallResult()
            {
                Title = options.Title,
                TimeStamp = DateTime.Now
            };
            foreach(var pageBuilder in PageBuilders)
            {
                if (pageBuilder.Result != null)
                {
                    overallResult.PageResults.Add(pageBuilder.Result);
                }
            }
            this.Result = overallResult;
            return this;
        }

        
        public string Export(string? fileName = null)
        {
            string path = Options.OutputFolder ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            int sequence = 1;

            //export the overallpage
            var html = PageReportBuilder.GetHtmlTemplate("overall-result.html");
            var rowTemplate = PageReportBuilder.GetHtmlTemplate("overall-tablerow.html");
            var TableRowPageResult = new StringBuilder();
            
            //export each subpages and generation of 
            foreach (var pageBuilder in PageBuilders)
            {
                if (pageBuilder.Result != null)
                {
                    string subFolder = "page" + sequence++;
                    string filename = "page-report.html";
                    pageBuilder.Options.OutputFolder = Path.Combine(path, subFolder);
                    pageBuilder.Options.OutputFormat = OutputFormat.Html;
                    pageBuilder.Export(filename);
                    TableRowPageResult.AppendLine(rowTemplate.Replace("{{PageTitle}}", pageBuilder.Options.Title)
                        .Replace("{{PageUrl}}", pageBuilder.Result.Url)
                        .Replace("{{Score}}", pageBuilder.Result.Score.ToString())
                        .Replace("{{ReportLink}}", subFolder + "/" + filename));
                }
            }

            html = html.Replace("{{Title}}", Options.Title)
                    .Replace("{{TimeStamp}}", this.Result.TimeStamp.ToString())
                    .Replace("{{Score}}", this.Result.Score.ToString())
                    .Replace("{{TableRowPageResult}}", TableRowPageResult.ToString())
                    .Replace("{{ScoreBackgroundColor}}", this.Result.ScoreBackgroundColor)
                    .Replace("{{ScoreColor}}", this.Result.ScoreForegroundColor)
                    .Replace("{{ScoreRotation}}", this.Result.ScoreRotation.ToString())
                    .Replace("{{TableRowPageResult}}", TableRowPageResult.ToString());


            string fullname = Path.Combine(path, fileName ?? "index.html");
            File.WriteAllText(fullname, html);

            switch (Options.OutputFormat)
            {
                case OutputFormat.Html:
                    return fullname;
                case OutputFormat.Zip:
                    var file = Path.GetTempFileName();
                    var zipName = Path.Combine(path, "report.zip");
                    File.Delete(file);
                    ZipFile.CreateFromDirectory(path, file);
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                    File.Copy(file, zipName, true);
                    File.Delete(file);
                    return zipName;
                default:
                    // it will be a bug if new output format is not yet implemented.
                    throw new NotImplementedException($"Output format is not yet supported {Options.OutputFormat}");
            }
        }

        public PageReportOptions Options
        {
            get { return this.options; }
        }

        public List<PageReportBuilder> PageBuilders { get; internal set; }
        public AxeOverallResult Result { get; private set; }
    }
}
