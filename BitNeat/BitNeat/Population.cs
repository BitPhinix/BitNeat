using System;
using System.Collections.Generic;
using System.Linq;
using BitNeat.DataClasses;

namespace BitNeat
{
    public class Population
    {
        /// <summary>
        /// The average fitness of all species
        /// </summary>
        public double AverageFitness => Species.Count == 0 ? 0 : Species.Average(s => s.AverageFitness);

        /// <summary>
        /// The max fitness of all species
        /// </summary>
        public double MaxFitness => Species.Count == 0 ? 0 : Species.Max(s => s.MaxFitness);

        /// <summary>
        /// The min fitness of all species
        /// </summary>
        public double MinFitness => Species.Count == 0 ? 0 : Species.Min(s => s.MinFitness);

        /// <summary>
        /// All species in the population
        /// </summary>
        public List<Species> Species { get; set; } = new List<Species>();

        /// <summary>
        /// Evaluates all members in the population
        /// </summary>
        /// <param name="evaluationFunc">The evaluation function</param>
        public void EvaluateMembers(Func<Network, double> evaluationFunc)
        {
            if(evaluationFunc == null)
                throw new ArgumentException("EvaluationFunc cant be null !");

            //Iderate over each species
            foreach (var species in Species)
            {
                //Nothing to do
                if (species.Members.Count < 1)
                    continue;

                //Evaluate each member in the species
                foreach (var member in species.Members)
                    member.Fitness = evaluationFunc(new Network(member));

                //Check if AlltimeMaxFitness is smaler then MaxFitness
                if (species.MaxFitness > species.AlltimeMaxFitness)
                {
                    //Update AlltimeMaxFitness
                    species.AlltimeMaxFitness = species.MaxFitness;

                    //Set NonImprovingGenerationsCount to 0
                    species.NonImprovingGenerationsCount = 0;
                }
                else
                    species.NonImprovingGenerationsCount++;
            }
        }

        /// <summary>
        /// Returns the fittest member of the population
        /// </summary>
        /// <returns>The fittest member</returns>
        public Genome GetFittestMember()
        {
            return Species.FirstOrDefault(s => s.MaxFitness == MaxFitness).Members.FirstOrDefault(m => m.Fitness == MaxFitness);
        }
    }
}
