namespace BitNeat.DataClasses
{
    public enum NodeType
    {
        /// <summary>
        /// Node used for inputing
        /// </summary>
        Input,

        /// <summary>
        /// Node used for processing
        /// </summary>
        Hidden,

        /// <summary>
        /// Node used as output
        /// </summary>
        Output,

        /// <summary>
        /// Bias node
        /// </summary>
        Bias
    }
}