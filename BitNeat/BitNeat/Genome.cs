using System;
using System.Collections.Generic;
using System.Linq;

namespace BitNeat.DataClasses
{
    public class Genome
    {
        private static readonly Random Rnd = new Random();

        public double Fitness { get; set; }
        public List<NodeInformation> NodeGenes { get; set; } = new List<NodeInformation>();
        public List<ConnectionInformation> ConnectionGenes { get; set; } = new List<ConnectionInformation>();

        public Genome Clone()
        {
            return new Genome
            {
                Fitness = Fitness,
                NodeGenes = new List<NodeInformation>(NodeGenes),
                ConnectionGenes = new List<ConnectionInformation>(ConnectionGenes)
            };
        }

        public double GetSimilaritie(Genome genome, double excessGenesComparisonImpact, double disjointGenesComparisonImpact, double weightDifferenceComparisonImpact)
        {
            //Get Exess, Disjoint, Matching genes
            var genomeComparisonResult = GetCompatibility(genome);

            //Calcuate Exess, Disjoint values
            var excess = excessGenesComparisonImpact * genomeComparisonResult.OwnGene.Excess.Count / genome.NodeGenes.Count;
            var disjoint = disjointGenesComparisonImpact * genomeComparisonResult.OwnGene.Disjoined.Count / genome.NodeGenes.Count;

            var weightDifference = 0d;

            //Calculate weight differences
            for (var i = 0; i < genomeComparisonResult.OtherGene.Matching.Count; i++)
                weightDifference += Math.Abs(genomeComparisonResult.OtherGene.Matching[i].Weight -
                                             genomeComparisonResult.OwnGene.Matching[i].Weight);

            weightDifference = weightDifference / genomeComparisonResult.OtherGene.Matching.Count * weightDifferenceComparisonImpact;

            //Return result
            return excess + disjoint + weightDifference;
        }

        public GenomeComparisonResult GetCompatibility(Genome genome)
        {
            var result = new GenomeComparisonResult();
            var g2Index = 0;

            //Order connectiongenes
            genome.ConnectionGenes = genome.ConnectionGenes.OrderBy(cg => cg.Id).ToList();
            ConnectionGenes = ConnectionGenes.OrderBy(cg => cg.Id).ToList();

            for (int i = 0; i < genome.ConnectionGenes.Count; i++)
            {
                if (g2Index > ConnectionGenes.Count - 1)
                {
                    //Genome1 Excess
                    result.OwnGene.Excess.AddRange(genome.ConnectionGenes.GetRange(i, genome.ConnectionGenes.Count - i));
                    return result;
                }

                if (genome.ConnectionGenes[i].Id < ConnectionGenes[g2Index].Id)
                {
                    //Genome1 Disjoined
                    result.OwnGene.Disjoined.Add(genome.ConnectionGenes[i]);
                }
                else if (genome.ConnectionGenes[i].Id == ConnectionGenes[g2Index].Id)
                {
                    //Matching
                    result.OtherGene.Matching.Add(genome.ConnectionGenes[i]);
                    result.OwnGene.Matching.Add(ConnectionGenes[g2Index]);
                    g2Index++;
                }
                else
                {
                    while (genome.ConnectionGenes[i].Id > ConnectionGenes[g2Index].Id)
                    {
                        //Genome2 Disjoined
                        result.OtherGene.Disjoined.Add(ConnectionGenes[g2Index]);
                        g2Index++;

                        if (g2Index <= ConnectionGenes.Count - 1)
                            continue;

                        //Genome1 Excess
                        result.OwnGene.Excess.AddRange(genome.ConnectionGenes.GetRange(i, genome.ConnectionGenes.Count - i));
                        return result;
                    }
                }
            }

            //Genome2 Excess
            result.OtherGene.Excess.AddRange(ConnectionGenes.GetRange(g2Index, ConnectionGenes.Count - g2Index));
            return result;
        }

        public Genome Crossover(Genome genome)
        {
            var child = new Genome();

            //Fitness not equal
            var genomeComparisonResult = GetCompatibility(genome);
            var betterGene = genome.Fitness > Fitness ? genomeComparisonResult.OwnGene : genomeComparisonResult.OtherGene;
            var worseGene = genome.Fitness > Fitness ? genomeComparisonResult.OtherGene : genomeComparisonResult.OwnGene;

            //Process matching genes
            for (var i = 0; i < betterGene.Matching.Count; i++)
                //50% chance
                child.ConnectionGenes.Add(Rnd.Next(2) == 1
                    ? betterGene.Matching[i]
                    : worseGene.Matching[i]);

            //Add disjoined and excess genes of better parent
            child.ConnectionGenes.AddRange(betterGene.Disjoined);
            child.ConnectionGenes.AddRange(betterGene.Excess);

            //Add nodeGenes of better parent
            child.NodeGenes = new List<NodeInformation>((genome.Fitness > Fitness ? genome : this).NodeGenes);

            return child;
        }

        public static Genome Generate(int inputNeurons, int outputNeurons, bool connect)
        {
            var genome = new Genome
            {
                Fitness = 0
            };

            genome.NodeGenes.Add(new NodeInformation
            {
                Id = -1,
                Type = NodeType.Bias
            });

            for (var i = 0; i < inputNeurons; i++)
            {
                genome.NodeGenes.Add(new NodeInformation
                {
                    Id = i - inputNeurons - outputNeurons - 1,
                    Type = NodeType.Input
                });
            }

            for (var i = 0; i < outputNeurons; i++)
            {
                genome.NodeGenes.Add(new NodeInformation
                {
                    Id = i - outputNeurons - 1,
                    Type = NodeType.Output
                });

                if(!connect)
                    continue;

                for (var j = 0; j < inputNeurons; j++)
                {
                    genome.ConnectionGenes.Add(new ConnectionInformation
                    {
                        Id = -1,
                        FromNode = j - inputNeurons - outputNeurons - 1,
                        ToNode = i - outputNeurons - 1,
                        Weight = 1,
                        Enabled = true
                    });
                }
            }

            return genome;
        }
    }
}