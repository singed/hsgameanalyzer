using HSDeckCollector.Core;
using OpenQA.Selenium;

namespace HSDeckCollector.Managers
{
    /// <summary>
    /// Parent manager
    /// </summary>
    public class ParentManager
    {
        private static Browser _browser;

        /// <summary>
        /// Default initialization of used webdriver
        /// </summary>
        public static IWebDriver Driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentManager"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        protected ParentManager(Browser browser)
        {
            _browser = browser;
            Driver = browser.WebDriver;
        }

        /// <summary>
        /// Gets the current browser.
        /// </summary>
        /// <value>
        /// The current browser.
        /// </value>
        public static Browser CurrentBrowser
        {
            get { return _browser; }
        }
    }
}
