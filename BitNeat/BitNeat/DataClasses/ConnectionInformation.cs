namespace BitNeat.DataClasses
{
    public struct ConnectionInformation
    {
        /// <summary>
        /// The node to connect from
        /// </summary>
        public long FromNode;

        /// <summary>
        /// The node to connect to
        /// </summary>
        public long ToNode;

        /// <summary>
        /// The weight of the connection
        /// </summary>
        public double Weight;

        /// <summary>
        /// Determine whether to connection is processed
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// The id of the connection
        /// </summary>
        public long Id;
    }
}