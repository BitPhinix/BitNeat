namespace BitNeat.DataClasses
{
    public struct Inovation
    {
        /// <summary>
        /// The id of the Inovation
        /// </summary>
        public int Id;

        /// <summary>
        /// The type of the inovation
        /// </summary>
        public InovationType Type;

        /// <summary>
        /// The node to connect from
        /// </summary>
        public long FromNode;

        /// <summary>
        /// The node to connect to
        /// </summary>
        public long ToNode;
    }
}