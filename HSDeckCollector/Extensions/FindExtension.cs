using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace HSDeckCollector.Extensions
{
    /// <summary>
    /// Contains Webdriver extensions which search elements on page and define page state
    /// </summary>
    public static class FindExtension
    {
        /// <summary>
        /// Finds web element by jQuery selector
        /// </summary>
        /// <param name="driver">Webdriver extension</param>
        /// <param name="selector">Defined selector</param>
        /// <param name="timeout">Max execution timeout</param>
        /// <returns>Returns web element</returns>
        public static IWebElement TryFindElementByJquery(this IWebDriver driver, string selector, int timeout)
        {
            return TryFindElementByJquery(driver, selector, timeout, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IWebElement TryFindDisabledElementByJquery(this IWebDriver driver, string selector)
        {
            return TryFindElementByJquery(driver, selector, 30, false);
        }

        public static IWebElement TryFindElementByJs(this IWebDriver driver, string selector)
        {
            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            IWebElement result = null;
            try
            {
                driver.Wait(d =>
                {
                    try
                    {
                        result = (IWebElement)d.JsExecuteJavaScript(string.Format("return document.querySelector(\"{0}\")", selector));
                        return (result != null);
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(30));
            }
            catch
            {
                return null;
            }
            return result;
        }

        public static List<IWebElement> TryFindElementsByJs(this IWebDriver driver, string selector, int timeout =30)
        {
            var temp = new List<IWebElement>();

            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            try
            {
                // IN FF THIS 'WAIT' RETURNS 0 -> EXCEPTION, WHILE OK IN CHROME
                driver.Wait(d =>
                {
                    try
                    {
                            var elements =
                                  (IList<IWebElement>)  driver.JsExecuteJavaScript(string.Format("return document.querySelectorAll(\"{0}\")", selector));
                            if (elements != null)
                                temp.AddRange(elements);
                      
                        driver.CheckErrorsOnPage();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return temp;
            }
            return temp;
        }

        /// <summary>
        /// Finds web element by jQuery selector
        /// </summary>
        /// <param name="driver">Webdriver extension</param>
        /// <param name="selector">Defined selector</param>
        /// <param name="timeout">Max execution timeout in seconds. Default to 30 sec.</param>
        /// <param name="isEnabled">Check whether element enabled or not. Defaults to true.</param>
        /// <returns>Returns web element</returns>
        public static IWebElement TryFindElementByJquery(this IWebDriver driver, string selector, int timeout = 30, bool isEnabled = true)
        {
            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            IWebElement res = null;
            bool condition;
            try
            {
                driver.Wait(d =>
                {
                    try
                    {
                        res = (IWebElement) d.JsExecuteJavaScript(string.Format("return $('{0}')[0]", selector));
                        condition = (!isEnabled) || (res.Displayed && res.Enabled);
                        driver.CheckErrorsOnPage();
                        //driver.WaitForPageLoaded();
                        return (res != null && condition);
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return null;
            }
            return res;
        }

     
        /// <summary>
        /// Check for element presence without awaiting "page ready" during seek period. Usable for WDM elements finding.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="selector">Element's jQuery selector</param>
        /// <param name="timeout">Seek period in seconds</param>
        /// <returns>Web element or null if not found</returns>
        public static IWebElement TryFindElementNowTm(this IWebDriver driver, string selector, int timeout)
        {
            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            IWebElement result = null;
            try
            {
                driver.Wait(d =>
                {
                    try
                    {
                        result = (IWebElement)d.JsExecuteJavaScript(string.Format("return $('{0}')[0]", selector));
                        return (result != null);
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Finds several web elements by jQuery selector
        /// </summary>
        /// <param name="driver">Webdriver extension</param>
        /// <param name="selector">Defined selector</param>
        /// <returns>Returns list of web elements</returns>
        public static List<IWebElement> TryFindElementsByJquery(this IWebDriver driver, string selector)
        {
            return TryFindElementsByJquery(driver, selector, 30);
        }

        /// <summary>
        /// Check for element presence immediately without awaiting "page ready". Usable for WDM elements finding.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="selector">Element's jQuery selector</param>
        /// <returns>Web elements list</returns>
        public static List<IWebElement> TryFindElementsNow(this IWebDriver driver, string selector)
        {
            return TryFindElementsNow(driver, selector, 1);
        }

        /// <summary>
        /// Check for element presence immediately - without awaiting "page ready". Usable for WDM elements finding.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="selector">Element's jQuery selector</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns>Web elements list</returns>
        public static List<IWebElement> TryFindElementsNow(this IWebDriver driver, string selector, int timeout)
        {
            var temp = new List<IWebElement>();

            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            try
            {
                // IN FF THIS 'WAIT' RETURNS 0 -> EXCEPTION, WHILE OK IN CHROME
                driver.Wait(d =>
                {
                    try
                    {
                        int len = driver.JsGetElementsCount(selector);
                        for (var i = 0; i < len; i++)
                        {
                            var elem =
                                (IWebElement)
                                    driver.JsExecuteJavaScript(string.Format("return $('{0}')[{1}]", selector, i));
                            if (elem != null)
                                temp.Add(elem);
                            else
                                break;
                        }
                        driver.CheckErrorsOnPage();
                        return (len > 0);
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return temp;
            }
            return temp;
        }

        /// <summary>
        /// Finds several web elements by jQuery selector
        /// </summary>
        /// <param name="driver">Webdriver extension</param>
        /// <param name="selector">Defined selector</param>
        /// <param name="timeout">Maximum execution timeout</param>
        /// <returns>Returns list of web elements</returns>
        public static List<IWebElement> TryFindElementsByJquery(this IWebDriver driver, string selector, int timeout)
        {
            var temp = new List<IWebElement>();

            if (string.IsNullOrEmpty(selector))
                throw new Exception("selector is empty");
            try
            {
                // IN FF THIS 'WAIT' RETURNS 0 -> EXCEPTION, WHILE OK IN CHROME
                driver.Wait(d =>
                {
                    try
                    {
                        int len = driver.JsGetElementsCount(selector);
                        for (var i = 0; i < len; i++)
                        {
                            var elem =
                                (IWebElement)
                                    driver.JsExecuteJavaScript(string.Format("return $('{0}')[{1}]", selector, i));
                            if (elem != null)
                                temp.Add(elem);
                            else
                                break;
                        }
                        driver.CheckErrorsOnPage();
                        return (len > 0);
                    }
                    catch
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return temp;
            }
            return temp;
        }

        /// <summary>
        /// Execute JavaScript Webdriver object extension.
        /// </summary>
        /// <param name="driver">Webdriver instance</param>
        /// <param name="script">Script to execute</param>
        /// <returns>Returns any possible object</returns>
        public static object JsExecuteJavaScript(this IWebDriver driver, string script)
        {
            var js = (IJavaScriptExecutor)driver;
            return js.ExecuteScript(script);
        }

        /// <summary>
        /// Counts number of web elements
        /// </summary>
        /// <param name="driver">Used Webdriver instance</param>
        /// <param name="selector">Defined selector</param>
        /// <returns>Number of elements or -1 if error is occurred</returns>
        public static int JsGetElementsCount(this IWebDriver driver, string selector)
        {
            try
            {
                return Convert.ToInt32(driver.JsExecuteJavaScript(string.Format("return $('{0}').length;", selector)));
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Counts number of visible web elements defined by selector
        /// </summary>
        /// <param name="driver">Used Webdriver instance</param>
        /// <param name="selector">Defined selector</param>
        /// <returns>Returns number of web elements or -1 if error is occurred</returns>
        private static int JsGetVisibleElementsCount(this IWebDriver driver, string selector)
        {
            try
            {
                return Convert.ToInt32(driver.JsExecuteJavaScript("return $('" + selector + "').filter(':visible').length;"));
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// This methods triggers "click" event on the first element defined by jQuery selector
        /// </summary>
        /// <param name="driver">Used Webdriver instance</param>
        /// <param name="selector">Defined selector</param>
        public static void JsClick(this IWebDriver driver, string selector)
        {
            driver.JsExecuteJavaScript(string.Format("$('{0}').first().trigger('click')", selector));
        }

        /// <summary>
        /// Check if current page has AngularJS error popup(s). Returns boolean result and stack trace (variable errorStackTrace). 
        /// </summary>
        private static void CheckErrorsOnPage(this IWebDriver driver)
        {
            try
            {
                var element = (IWebElement)driver.JsExecuteJavaScript("return $('#errorsContainer')[0]");
                if (element == null) 
                    return;
                var errorStackTrace = (string)driver.JsExecuteJavaScript("return $('#errorsContainer ul li span').map(function(ind, item){return $(item).text().trim()}).toArray().join(', ')");
                if (!string.IsNullOrEmpty(errorStackTrace))
                {
                    throw new UnhandledAlertException(string.Format("AngularJS error occured: {0}", errorStackTrace));
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
