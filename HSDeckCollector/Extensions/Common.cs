using System;

namespace HSDeckCollector.Extensions
{
    /// <summary>
    /// Common settings
    /// </summary>
    public static class Common
    {
        static Random rnd = new Random();

        /// <summary>
        /// Generate random number
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        public static int RandomNumber(int lowerLimit = 10000000, int upperLimit = 99999999)
        {
            return rnd.Next(lowerLimit, upperLimit);
        }

        /// <summary>
        /// Implicity wait for functions
        /// </summary>
        /// <param name="methodCall">Use as "()=>[condition]"</param>
        /// <param name="time">Use as TimeSpan.FromSeconds(XXX)</param>
        /// <param name="errormsg">Error message to log if condition isn't reached</param>
        public static void Wait(Func<bool> methodCall, TimeSpan time, string errormsg) 
        {
            DateTime dif = DateTime.Now + time;
            while (DateTime.Now < dif)
            {               
                try
                {
                    if (methodCall())
                    {
                        return;
                    }                   
                }
                catch (Exception) {
                    throw;
                }           
            }        
            throw new Exception(String.Format("{0}",errormsg));      
        }

        /// <summary>
        /// Implicity wait for bool conditions
        /// </summary>
        /// <param name="conditionCall">Condition call</param>
        /// <param name="time">Use as TimeSpan.FromSeconds(XXX)</param>
        /// <param name="errormsg">Error message to log if condition isn't reached</param>
        public static void Wait(bool conditionCall, TimeSpan time, string errormsg)
        {
            DateTime dif = DateTime.Now + time;
            while (DateTime.Now < dif)
            {
                try
                {
                    if (conditionCall)
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            throw new Exception(String.Format("{0}", errormsg));
        }
    }
}
