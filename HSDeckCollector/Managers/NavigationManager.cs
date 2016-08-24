using System;
using HSDeckCollector.Core;
using HSDeckCollector.Extensions;

namespace HSDeckCollector.Managers
{
    /// <summary>
    /// Global navigation, manages user's navigation via browser URL
    /// </summary>
    public class NavigationManager : ParentManager
    {
        /// <summary>
        /// Base application url. Different to POS and HOS 
        /// </summary>
        private string BaseUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationManager"/> class.
        /// </summary>
        /// <param name="browser">Browser object to create element manager for.</param>
        /// <param name="baseUrl">URL</param>
        public NavigationManager(Browser browser, string baseUrl) : base(browser)
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Goes to custom page.
        /// </summary>
        /// <param name="pageToNavigate">The page to navigate.</param>
        public void GoToPage(string pageToNavigate)
        {
            Driver.Navigate().GoToUrl(BaseUrl + pageToNavigate);
            WaitExtensions.Wait(() => Driver.Url.Contains(BaseUrl + pageToNavigate), TimeSpan.FromSeconds(3), "URL isn't reached");
        }

        public string CurrentUrl => Driver.Url;

        /// <summary>
        /// Goes to custom page.
        /// </summary>
        /// <param name="pageToNavigate">The page to navigate.</param>
        public void GoToPagePos(string pageToNavigate)
        {
            Driver.Navigate().GoToUrl(BaseUrl + pageToNavigate);
            WaitExtensions.Wait(() => Driver.WaitForPageLoaded(), TimeSpan.FromSeconds(3), "URL isn't reached");
        }
    }
}
