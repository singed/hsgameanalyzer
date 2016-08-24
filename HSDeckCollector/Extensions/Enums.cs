namespace HSDeckCollector.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// 
        /// </summary>
        public enum FuelingMode
        {
            Auto,
            Manual,
            Reserved
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FpState
        {
            Off,
            Ready,
            Withdrawn,
            Filling,
            PaymentAwaiting
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Application
        {
            HOS,
            POS,
            I4C,
            WDM,
            TMM
        }

        /// <summary>
        /// Types of log records (enum)
        /// </summary>
        public enum LogType
        {
            Log,
            Screen
        }
    }
}
