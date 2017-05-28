namespace BitNeat.DataClasses
{
    public class Synapse
    {
        /// <summary>
        /// The connected neuron
        /// </summary>
        public Neuron ConnectedNeuron { get; set; }

        /// <summary>
        /// The weight of the connection
        /// </summary>
        public double Weight { get; set; }
    }
}