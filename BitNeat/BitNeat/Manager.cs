using System;
using System.Collections.Generic;
using System.Linq;
using BitNeat.DataClasses;

namespace BitNeat
{
    public class Manager
    {
        public Mutator Mutator { get; set; }
        public Genome BaseGenome { get; set; }
        public Func<Network, double> EvaluationFunction { get; set; }
        public Population CurrentPopulation { get; } = new Population();

        public double ExcessGenesComparisonImpact { get; set; } = 1;
        public double DisjointGenesComparisonImpact { get; set; } = 1;
        public double WeightDifferenceComparisonImpact { get; set; } = 1;
        public double SpeciesThreshold { get; set; } = 1;

        public double CrossoverProbability { get; set; } = 75;
        public int PopulationSize { get; set; } = 200;

        public int MaxEqualFitnessGenerations { get; set; } = 20;

        private static readonly Random Rnd = new Random();

        public delegate void LifecycleFinishedEventDelagate(Population currentPopulation);
        public event LifecycleFinishedEventDelagate LifecycleFinishedEvent;

        /// <summary>
        /// Populates the population
        /// </summary>
        public void Populate()
        {
            if(BaseGenome == null)
                throw new Exception("BaseGenome has to be set first!");

            if(Mutator == null)
                throw new Exception("Mutator has to be set first!");

            //Add mutated versions of the base genome
            for (var i = 0; i < PopulationSize; i++)
                FindSpecies(CurrentPopulation, Mutator.Mutate(BaseGenome.Clone()));
        }

        /// <summary>
        /// Performs lifecycles untill the fitness is reached
        /// </summary>
        /// <param name="fitness">The fitness to be reached</param>
        public Network TrainUntil(double fitness)
        {
            if (EvaluationFunction == null)
                throw new Exception("EvaluationFuntion has to be set first!");

            if (Mutator == null)
                throw new Exception("Mutator has to be set first!");

            //Perform lifecycles untill the fitness is reached
            while (CurrentPopulation.MaxFitness < fitness)
                Next();

            return new Network(CurrentPopulation.GetFittestMember());
        }

        /// <summary>
        /// Performs a lifecycle on the current population
        /// </summary>
        public void Next()
        {
            if (CurrentPopulation.Species.Count == 0)
                Populate();

            if(EvaluationFunction == null)
                throw new Exception("EvaluationFuntion has to be set first!");

            if (Mutator == null)
                throw new Exception("Mutator has to be set first!");

            //Evaluate Members
            CurrentPopulation.EvaluateMembers(EvaluationFunction);

            //Perform lifecycle
            DoSelection();
            Reproduce();

            //Fire event
            LifecycleFinishedEvent?.Invoke(CurrentPopulation);
        }

        /// <summary>
        /// Performs selection on the generation
        /// </summary>
        private void DoSelection()
        {
            for (int i = 0; i < CurrentPopulation.Species.Count; i++)
            {
                var species = CurrentPopulation.Species[i];

                //Sort out species that have not improved in the last MaxEqualFitnessGenerations generation and are not the best current species or species that are to smal
                if (species.NonImprovingGenerationsCount > MaxEqualFitnessGenerations && species.MaxFitness < CurrentPopulation.MaxFitness || species.Members.Count <= 2)
                {
                    CurrentPopulation.Species.Remove(species);
                    i--;
                    continue;
                }

                //Kill worse half of each species
                species.OrderMembers();
                species.Members = species.Members.GetRange(0, species.Members.Count / 2);
            }

            var fitnessSum = CurrentPopulation.Species.Sum(s => s.AverageFitness);

            for (int i = 0; i < CurrentPopulation.Species.Count; i++)
            {
                var species = CurrentPopulation.Species[i];

                //Sort out weak species
                if (species.AverageFitness / fitnessSum * PopulationSize < 1)
                {
                    CurrentPopulation.Species.Remove(species);
                    i--;
                }
            }
        }

        /// <summary>
        /// Reproduces the generation
        /// </summary>
        private void Reproduce()
        {
            var fitnessSum = CurrentPopulation.Species.Sum(s => s.AverageFitness);
            var children = new List<Genome>();

            //Get children form each species (the ammout is detainment by the fitness of the species)
            foreach (var species in CurrentPopulation.Species)
                for (int i = 0; i < species.AverageFitness / fitnessSum * PopulationSize - 1; i++)
                    children.Add(GetChild(species));

            //Fill up population
            while (CurrentPopulation.Species.Count + children.Count < PopulationSize)
                children.Add(GetChild(CurrentPopulation.Species[Rnd.Next(CurrentPopulation.Species.Count)]));

            //Only keep the best genomes out of each species
            foreach (var species in CurrentPopulation.Species)
                species.Members = species.Members.GetRange(0, 1);

            //Find species for children
            foreach (var child in children)
                FindSpecies(CurrentPopulation, child);
        }

        /// <summary>
        /// Generates a child for the given species
        /// </summary>
        /// <param name="species">The species to get the child from</param>
        /// <returns>The child</returns>
        private Genome GetChild(Species species)
        {
            return Rnd.NextDouble() * 100 >= CrossoverProbability
                //Crossover child
                ? Mutator.Mutate(species.Members[Rnd.Next(species.Members.Count)]
                    .Crossover(species.Members[Rnd.Next(species.Members.Count)]))

                //Clone Child
                : Mutator.Mutate(species.Members[Rnd.Next(species.Members.Count)].Clone());
        }

        /// <summary>
        /// Finds/Adds a species for the given genome in the given population
        /// </summary>
        /// <param name="population">The population to insert the gemone</param>
        /// <param name="genome">The geome to insert</param>
        private void FindSpecies(Population population, Genome genome)
        {
            //Check all species
            foreach (var speciese in population.Species)
            {
                //If Representative and genome are not similar, skip
                if (speciese.Representative.GetSimilaritie(genome, ExcessGenesComparisonImpact, DisjointGenesComparisonImpact, WeightDifferenceComparisonImpact) >= SpeciesThreshold)
                    continue;

                //Add member to species and return
                speciese.Members.Add(genome);
                return;
            }

            //If no matching species is found, add a new one
            population.Species.Add(new Species
            {
                Members = new List<Genome> { genome },
                Representative = genome
            });
        }
    }
}
