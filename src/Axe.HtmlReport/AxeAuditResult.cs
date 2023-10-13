using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    public class AxeAuditResult
    {

        public List<AxeEnhancedResult> PageResults { get; set; } = new List<AxeEnhancedResult>();


        private int? _score;
        public int Score
        {
            get
            {
                _score ??= GetOverallScore();
                return _score.Value;
            }
        }

        private int GetOverallScore()
        {
            throw new NotImplementedException();
        }
    }
}
