using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace HSDeckCollector.Core
{
    /// <summary>
    /// Browser settings
    /// </summary>
    public class Browser : IDisposable
    {
        private readonly IWebDriver _driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="Browser"/> class.
        /// </summary>
        /// <param name="webDriverType">Type of the web driver.</param>
        /// <exception cref="System.ApplicationException">Unsupported driver type!</exception>
        public Browser(Type webDriverType)
        {
            if (webDriverType == typeof(InternetExplorerDriver))
            {
                var options = new InternetExplorerOptions
                {
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                    EnableNativeEvents = true
                };
                var path = System.Text.RegularExpressions.Regex.Replace(Environment.CurrentDirectory, @"\\([^\\])+\\([^\\])+\\([^\\])+(\\)?$", "") + "\\WebBrowserDrivers";
                _driver = new InternetExplorerDriver(path, options, TimeSpan.FromSeconds(60));      

            }
            else if (webDriverType == typeof(EdgeDriver))
            {
                var options = new EdgeOptions
                {
                    PageLoadStrategy = EdgePageLoadStrategy.Eager
                };
                var path = System.Text.RegularExpressions.Regex.Replace(Environment.CurrentDirectory, @"\\([^\\])+\\([^\\])+\\([^\\])+(\\)?$", "") + "\\WebBrowserDrivers";
                _driver = new EdgeDriver(path, options, TimeSpan.FromSeconds(60));
                _driver.Manage().Window.Maximize();
            }
            else if (webDriverType == typeof(ChromeDriver))
            {
                var options = new ChromeOptions();
                options.AddArguments("start-maximized", "chrome.switches", "--disable-extensions");
                
                var path = System.Text.RegularExpressions.Regex.Replace(Environment.CurrentDirectory, @"\\([^\\])+\\([^\\])+\\([^\\])+(\\)?$", "") + "\\WebBrowserDrivers";
                _driver = new ChromeDriver(path, options, TimeSpan.FromSeconds(60));
                _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
                _driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(15));
            }
            else if (webDriverType == typeof(FirefoxDriver))
            {
                _driver = new FirefoxDriver(new FirefoxOptions());
                _driver.Manage().Window.Maximize();
            }
            else
            {
                throw new ApplicationException("Unsupported driver type!");
            }
        }

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <value>
        /// The web driver.
        /// </value>
        public IWebDriver WebDriver
        {
            get
            {
                return _driver;
            }
        }

        /// <summary>
        /// Dispose objects
        /// </summary>
        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
