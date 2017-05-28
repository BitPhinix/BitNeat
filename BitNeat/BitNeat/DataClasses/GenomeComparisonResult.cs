namespace BitNeat.DataClasses
{
    public class GenomeComparisonResult
    {
        /// <summary>
        /// Genome 1 results
        /// </summary>
        public GenomeComparisonInfo OtherGene { get; set; } = new GenomeComparisonInfo();

        /// <summary>
        /// Genome 2 results
        /// </summary>
        public GenomeComparisonInfo OwnGene { get; set; } = new GenomeComparisonInfo();
    }
}