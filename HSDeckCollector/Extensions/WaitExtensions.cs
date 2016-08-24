using System;
using OpenQA.Selenium;

namespace HSDeckCollector.Extensions
{
    /// <summary>
    /// Waiting methods: waits for conditions
    /// </summary>
    public static class WaitExtensions
    {
        
        /// <summary>
        /// Wait for boolean condition method
        /// </summary>
        /// <param name="driver">Used webdriver</param>
        /// <param name="waitCondition">Boolean condition</param>
        /// <param name="total">Total time to wait for condition</param>
        public static void Wait(this IWebDriver driver, Func<IWebDriver, bool> waitCondition, TimeSpan total)
        {
            driver.Wait(waitCondition, total, "");
        }
        
        /// <summary>
        /// Wait for boolean condition method
        /// </summary>
        /// <param name="driver">Used webdriver</param>
        /// <param name="waitCondition">Boolean condition</param>
        /// <param name="total">Total time to wait for condition</param>
        /// <param name="timeoutMessage">Timeout message to display if condition is not reached on time</param>
        private static void Wait(this IWebDriver driver, Func<IWebDriver, bool> waitCondition, TimeSpan total, string timeoutMessage)
        {
            var waitStart = DateTime.Now;
            var waitFinish = waitStart.Add(total);
            while (DateTime.Now <= waitFinish)
            {
                try
                {   // ignore any exceptions during specified period
                    if (waitCondition(driver))
                        return;
                }
                catch
                {
                    // ignored
                }
            }
            if (!string.IsNullOrEmpty(timeoutMessage))
                throw new Exception("Timeout exceeded. " + timeoutMessage);
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
                catch
                {
                    // ignored
                }
            }
            throw new Exception(string.Format("{0}", errormsg));
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
                    // ignored
                }
            }
            throw new Exception(string.Format("{0}", errormsg));
        }

        /// <summary>
        /// Identify if page is already loaded or not
        /// </summary>
        /// <param name="driver">Webdriver extension</param>
        /// <returns>Returns true if "page is loaded" condition is true</returns>
        public static bool WaitForPageLoaded(this IWebDriver driver)
        {
            Wait(() => 
                driver.JsGetElementsCount("#httpReady") > 0 && driver.JsGetElementsCount("#loading-bar") == 0, 
                    TimeSpan.FromSeconds(60), "Page not loaded - timed out");
            return true;
        }
    }
}