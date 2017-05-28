using System;
using System.Linq;
using BitNeat.DataClasses;
using Newtonsoft.Json;

namespace BitNeat
{
    public class Network
    {
        public int InputSize { get; set; }
        public int OutputSize { get; set; }

        public Neuron[] Neurons { get; set; }

        /// <summary>
        /// Json.net needs it :/
        /// </summary>
        public Network()
        {
            
        }

        /// <summary>
        /// Serializes the network to a json string
        /// </summary>
        /// <returns>The serialized string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        }

        /// <summary>
        /// Deserializes a json string to a network
        /// </summary>
        /// <param name="s">The string to deserialize</param>
        /// <returns>The deserialized network</returns>
        public static Network FromString(string s)
        {
            try
            {
                return JsonConvert.DeserializeObject<Network>(s, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
            }
            catch
            {
                throw new ArgumentException("Invalid json !");
            }
        }

        /// <summary>
        /// Creates a network from a <see cref="Genome"/>
        /// </summary>
        /// <param name="genome">The genome to create the network from</param>
        public Network(Genome genome)
        {
            //Set input, output size
            InputSize = genome.NodeGenes.Count(ng => ng.Type == NodeType.Input);
            OutputSize = genome.NodeGenes.Count(ng => ng.Type == NodeType.Output);

            //Set neurons
            Neurons = genome.NodeGenes.Select(ng => new Neuron {Type = ng.Type}).ToArray();

            //Create dictionary with a mapping from the node id to it's position in the _neurons list
            var dict = genome.NodeGenes.Select((ng, index) => new {ng.Id, index}).ToDictionary(p => p.Id, p => p.index);

            //Add connections to neurons
            foreach (var connectionGene in genome.ConnectionGenes)
                if(connectionGene.Enabled)
                    Neurons[dict[connectionGene.ToNode]].ConnectedNeurons.Add(new Synapse
                    {
                        Weight = connectionGene.Weight,
                        ConnectedNeuron = Neurons[dict[connectionGene.FromNode]]
                    });
        }

        /// <summary>
        /// Calculates the networks output from the given input
        /// </summary>
        /// <param name="input">The input</param>
        /// <returns>The result</returns>
        public double[] Calculate(double[] input)
        {
            SetInput(input);
            Update();
            return GetResult();
        }


        /// <summary>
        /// Sets the values of all input neurons
        /// </summary>
        /// <param name="input"></param>
        private void SetInput(double[] input)
        {
            if(input.Length != InputSize)
                throw new ArgumentException("Invalid input size !");

            var i = 0;
            var c = 0;

            //While not all input neurons are set
            while (c < InputSize)
            {
                //Check if current neuron is a input neuron
                if (Neurons[i].Type == NodeType.Input)
                {
                    //Set its value
                    Neurons[i].Value = input[c];
                    c++;
                }

                i++;
            }
        }

        /// <summary>
        /// Gets the result of the network
        /// </summary>
        /// <returns>The result</returns>
        private double[] GetResult()
        {
            var result = new double[OutputSize];

            var i = 0;
            var c = 0;

            //While not all output neurons are read
            while (c < OutputSize)
            {
                //Check if current neuron is a output neuron
                if (Neurons[i].Type == NodeType.Output)
                {
                    //Get its value and append it to the result array
                    result[c] = Neurons[i].Value;
                    c++;
                }

                i++;
            }

            return result;
        }

        /// <summary>
        /// Updates all neurons in the network
        /// </summary>
        private void Update()
        {
            foreach (var neuron in Neurons)
                neuron.Update();
        }
    }
}
