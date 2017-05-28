using System;
using System.Collections.Generic;
using System.Linq;
using BitNeat.DataClasses;

namespace BitNeat
{
    public class Mutator
    {
        private static readonly Random Rnd = new Random();

        public List<Inovation> Inovations { get; } = new List<Inovation>();

        public double AdjustWeightMutationProbability { get; set; } = 70;
        public double SetRandomWeightMutationProbability { get; set; } = 20;
        public double ToggleGeneMutationProbability { get; set; } = 8;
        public double AddConnectionGeneProbability { get; set; } = 15;
        public double AddNodeGeneProbability { get; set; } = 5;

        public double MaxAdjustWeightMultiplier { get; set; } = 0.1;
        public double MaxChangeWeightValue { get; set; } = 2;
        public double MaxNewGeneWeightValue { get; set; } = 1;

        /// <summary>
        /// Mutates the given genome
        /// </summary>
        /// <param name="genome">The genome to mutate</param>
        /// <returns>The mutated genome</returns>
        public Genome Mutate(Genome genome)
        {
            if (Rnd.NextDouble() * 100 <= AdjustWeightMutationProbability)
            {
                //Multiply weight by a value between 1 +/- MaxAdjustWeightMultiplier
                var i = Rnd.Next(genome.ConnectionGenes.Count);
                var toEdit = genome.ConnectionGenes[i];
                toEdit.Weight *= 1 - MaxAdjustWeightMultiplier + Rnd.NextDouble() * 2 * MaxAdjustWeightMultiplier;
                genome.ConnectionGenes[i] = toEdit;
            }

            if (Rnd.NextDouble() * 100 <= SetRandomWeightMutationProbability)
            {
                //Set weight to a value between 0 +/- MaxChangeWeightValue
                var i = Rnd.Next(genome.ConnectionGenes.Count);
                var toEdit = genome.ConnectionGenes[i];
                toEdit.Weight *= MaxChangeWeightValue * 2 * Rnd.NextDouble() - MaxChangeWeightValue;
                genome.ConnectionGenes[i] = toEdit;
            }

            if (Rnd.NextDouble() * 100 <= ToggleGeneMutationProbability)
            {
                //Select random connection
                var index = Rnd.Next(genome.ConnectionGenes.Count);
                var toEdit = genome.ConnectionGenes[index];

                //Toggle it
                toEdit.Enabled = !toEdit.Enabled;
                genome.ConnectionGenes[index] = toEdit;
            }

            if (Rnd.NextDouble() * 100 <= AddConnectionGeneProbability)
            {
                //Select 2 nodegenes
                var geneIndices = new []{Rnd.Next(genome.NodeGenes.Count), Rnd.Next(genome.NodeGenes.Count)};
                
                //If they are not equal
                if (geneIndices[0] != geneIndices[1])
                {
                    //Get node in from of the other
                    var fromNode = genome.NodeGenes[geneIndices.Min(i => i)];

                    //Get node in the back of the other
                    var toNode = genome.NodeGenes[geneIndices.Max(i => i)];

                    //Check if connection exists
                    if (!genome.ConnectionGenes.Any(cg => cg.FromNode.Equals(fromNode.Id) && cg.ToNode.Equals(toNode.Id)) && 
                        //Check if connection is allowed
                        (fromNode.Type != toNode.Type || fromNode.Type == NodeType.Hidden || toNode.Type == NodeType.Hidden))
                    {
                        //Create connection
                        genome.ConnectionGenes.Add(new ConnectionInformation
                        {
                            Weight = Rnd.NextDouble() * 2 * MaxNewGeneWeightValue - MaxNewGeneWeightValue,
                            Enabled = true,
                            FromNode = fromNode.Id,
                            ToNode = toNode.Id,
                            Id = GetInovationNumber(fromNode.Id, toNode.Id, InovationType.Connect)
                        });
                    }
                }
            }
            
            if (Rnd.NextDouble() <= AddNodeGeneProbability)
            {
                //Get connection to intercept
                var interceptIndex = Rnd.Next(genome.ConnectionGenes.Count);
                var toIntercept = genome.ConnectionGenes[interceptIndex];

                //If connection is Enabled
                if (toIntercept.Enabled)
                {
                    //Get new node id
                    var id = GetInovationNumber(toIntercept.FromNode, toIntercept.ToNode, InovationType.Add);

                    //If does not already exsists
                    if (genome.NodeGenes.All(ng => ng.Id != id))
                    {
                        //Deactivate old connection
                        toIntercept.Enabled = false;
                        genome.ConnectionGenes[interceptIndex] = toIntercept;

                        //Add connection from the old connection start point to the new node
                        genome.ConnectionGenes.Add(new ConnectionInformation
                        {
                            Enabled = true,
                            FromNode = toIntercept.FromNode,
                            ToNode = id,
                            Id = id,
                            Weight = Rnd.NextDouble() * 2 * MaxNewGeneWeightValue - MaxNewGeneWeightValue
                        });

                        //Add connection from the new node to the old connection end point
                        genome.ConnectionGenes.Add(new ConnectionInformation
                        {
                            Enabled = true,
                            FromNode = id,
                            ToNode = toIntercept.ToNode,
                            Id = id,
                            Weight = Rnd.NextDouble() * 2 * MaxNewGeneWeightValue - MaxNewGeneWeightValue
                        });

                        //Add node to genomes node genes
                        genome.NodeGenes.Insert(genome.NodeGenes.Select((ng, index) => new { ng, index }).First(p => p.ng.Id == toIntercept.FromNode).index + 1, new NodeInformation
                        {
                            Type = NodeType.Hidden,
                            Id = id
                        });
                    }
                }
            }

            return genome;
        }

        /// <summary>
        /// Returns the inovation number for the given inovation
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <param name="type"></param>
        /// <returns>The innovation number</returns>
        public int GetInovationNumber(long fromNode, long toNode, InovationType type)
        {
            //If there are no inovations jet
            if (!Inovations.Any())
            {
                //Add inovation
                Inovations.Add(new Inovation { FromNode = fromNode, ToNode = toNode, Type = type, Id = 0 });

                //Return id
                return 0;
            }

            //Get matching inovations
            var inovations = Inovations.Where(inv => inv.Type == type && inv.FromNode.Equals(fromNode) && inv.ToNode.Equals(toNode)).ToList();

            //If inovation already exsists
            if (inovations.Any())
                //Return id od of the inovation
                return inovations.First().Id;

            //Add inovations to known inovations
            var id = Inovations.Last().Id + 1;
            Inovations.Add(new Inovation { FromNode = fromNode, ToNode = toNode, Type = type, Id = id });

            //Return id
            return id;
        }
    }
}