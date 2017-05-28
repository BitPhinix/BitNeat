using System;
using System.Collections.Generic;
using System.Linq;
using BitNeat.DataClasses;

namespace BitNeat
{
    public class Neuron
    {
        /// <summary>
        /// The value of the neuron. This is set by <see cref="Update"/>
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The type of the neuron
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// A list of all connected neurons
        /// </summary>
        public List<Synapse> ConnectedNeurons { get; set; } = new List<Synapse>();

        /// <summary>
        /// Updates the neurons value
        /// </summary>
        public void Update()
        {
            if (Type == NodeType.Bias)
                //Bias is always 1
                Value = 1;
            else if (Type != NodeType.Input)
                //Sigmoid
                Value = 1 / (1 + Math.Pow(Math.E, -GetConnectedValue())); 
        }

        /// <summary>
        /// Returns the sum of all connections
        /// </summary>
        /// <returns></returns>
        private double GetConnectedValue()
        {
            // Sum off all connection values
            return ConnectedNeurons.Sum(s => s.Weight * s.ConnectedNeuron.Value);
        }
    }
}