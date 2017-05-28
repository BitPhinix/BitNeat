namespace BitNeat.DataClasses
{
    public struct NodeInformation
    {
        /// <summary>
        /// The id of the node. This is set by <see cref="Mutator.GetInovationNumber"/>
        /// </summary>
        public long Id;

        /// <summary>
        /// The type of the node
        /// </summary>
        public NodeType Type;
    }
}