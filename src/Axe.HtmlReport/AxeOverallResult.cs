using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    public class AxeOverallResult : BaseResult
    {

        public List<AxePageResult> PageResults { get; set; } = new List<AxePageResult>();


        public string Title { get; internal set; }
        public DateTime TimeStamp { get; internal set; }



        /// <summary>
        /// Calculate the overall score of the application based on all tested pages.
        /// If a rule is failed on one page, It will be marked as Failed in final result.
        /// A rule is marked Passed if the result is passed on every tested pages.
        /// </summary>
        /// <returns></returns>
        protected override int GetScore()
        {
            Dictionary<string, ResultType> overallResults = new Dictionary<string, ResultType>();
            Dictionary<string, int> rulePoints = new Dictionary<string, int>();

            foreach(var page in PageResults)
            {
                foreach(var pass in page.Passes)
                {
                    var id = pass.Item.Id;
                    if (!overallResults.ContainsKey(id))
                    {
                        overallResults[id] = ResultType.Passes;
                        rulePoints[id] = AxePageResult.ScorePerImpact(pass.Item);
                    }
                }

                foreach(var violation in page.Violations)
                {
                    var id = violation.Item.Id;
                    overallResults[id] = ResultType.Violations;
                    rulePoints[id] = AxePageResult.ScorePerImpact(violation.Item);
                }
            }

            int passed = 0;
            int overall = 0;
            foreach(var key in overallResults.Keys)
            {
                int impact = rulePoints[key];
                if (overallResults[key] == ResultType.Passes)
                {
                    passed += impact;
                }
                overall += impact;
            }
            var score = passed * 100 / overall;
            Score = score;
            ScoreRotation = Score * 360 / 100;
            return score;
        }
    }
}
