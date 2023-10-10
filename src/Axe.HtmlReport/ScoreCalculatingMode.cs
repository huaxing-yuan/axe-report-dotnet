﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.HtmlReport
{
    /// <summary>
    /// Mode to calculate the score
    /// </summary>
    public enum ScoringMode
    {
        /// <summary>
        /// Each passed and failed rule will be counted as 1, the number of occurances of each rule are not counted.
        /// </summary>
        NonWeighted,

        /// <summary>
        /// Each passed and failed rule will be counted according to its severity, the number of occurances of each rule are not counted.
        /// </summary>
        Weighted,

        /// <summary>
        /// Each passed and failed rule will be counted according to its severity and the number of occurance.
        /// </summary>
        WeightedOccurence,
    }
}
