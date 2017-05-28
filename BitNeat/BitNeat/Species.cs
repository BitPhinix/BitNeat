using System.Collections.Generic;
using System.Linq;
using BitNeat.DataClasses;

namespace BitNeat
{
    public class Species
    {
        /// <summary>
        /// The Representative of the species
        /// </summary>
        public Genome Representative { get; set; }

        /// <summary>
        /// The members of the species
        /// </summary>
        public List<Genome> Members { get; set; }

        /// <summary>
        /// The count of the non improving generations
        /// </summary>
        public int NonImprovingGenerationsCount { get; set; }

        /// <summary>
        /// The average fitness of the species
        /// </summary>
        public double AverageFitness => Members.Average(m => m.Fitness);

        /// <summary>
        /// The maximum fitness of all members      
        /// </summary>
        public double MaxFitness => Members.Max(m => m.Fitness);

        /// <summary>
        /// The minimum fitness of all members
        /// </summary>
        public double MinFitness => Members.Min(m => m.Fitness);

        /// <summary>
        /// The highest fitness the species ever had
        /// </summary>
        public double AlltimeMaxFitness { get; set; }

        /// <summary>
        /// Orders all members after their fitness
        /// </summary>
        public void OrderMembers()
        {
            Members = Members.OrderByDescending(m => m.Fitness).ToList();
        }
    }
}