using System.Collections.Generic;

namespace BitNeat.DataClasses
{
    public class GenomeComparisonInfo
    {
        /// <summary>
        /// Matching genes
        /// </summary>
        public List<ConnectionInformation> Matching { get; set; } = new List<ConnectionInformation>();

        /// <summary>
        /// Disjoined genes
        /// </summary>
        public List<ConnectionInformation> Disjoined { get; set; } = new List<ConnectionInformation>();

        /// <summary>
        /// Excess genes
        /// </summary>
        public List<ConnectionInformation> Excess { get; set; } = new List<ConnectionInformation>();
    }
}