using System;
using System.Collections.Generic;
using HSDeckCollector.Core;
using HSDeckCollector.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace HSDeckCollector.Managers
{
    /// <summary>
    /// Class which contains common actions/interactions with elements
    /// </summary>
    public class ActionManager : ParentManager
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        protected const byte OneMoment = 1;

        /// <summary>
        /// 
        /// </summary>
        protected const byte CoupleOfSeconds = 2;

        /// <summary>
        /// 
        /// </summary>
        protected const byte ShortWait = 5;

        /// <summary>
        /// 
        /// </summary>
        protected const byte WaitABit = 15;

        /// <summary>
        /// 
        /// </summary>
        protected const byte WaitLonger = 30;

        /// <summary>
        /// 
        /// </summary>
        protected const byte TooLongTime = 120;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new element manager object for the given browser. Call constructor of the base class.
        /// </summary>
        /// <param name="browser">Browser object to create element manager for.</param>
        public ActionManager(Browser browser)
            : base(browser)
        {
        }

        #endregion
        /// <summary>
        /// Double-clicks web element.
        /// </summary>
        /// <param name="element">Web element to be clicked.</param>
        protected void DoubleClick(IWebElement element)
        {
            try
            {
                var action = new Actions(CurrentBrowser.WebDriver);
                action.MoveToElement(element).Perform();
                action.DoubleClick(element).Perform();
            }
            catch (Exception)
            {
                throw new Exception("Double click of element failed");
            }
        }

        /// <summary>
        /// Double-clicks web element by jQuery selector.
        /// </summary>
        /// <param name="selector">Web element selector.</param>
        protected void DoubleClickByJquery(string selector)
        {
            var element = Driver.TryFindElementByJquery(selector);
            try
            {
                var action = new Actions(CurrentBrowser.WebDriver);
                action.DoubleClick(element).Perform();
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Double click of element '{0}' failed", selector));
            }
        }

        /// <summary>
        /// Tries the find element by jQuery.
        /// </summary>
        /// <param name="selector">Web element selector.</param>       
        /// <returns>Web element object</returns> 
        public IWebElement TryFindElementByJquery(string selector)
        {
            return Driver.TryFindElementByJquery(selector);
        }

        /// <summary>
        /// Tries to find disabled web element by jQuery
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <returns>Web element object</returns>
        public IWebElement TryFindDisabledElementByJquery(string selector)
        {
            return Driver.TryFindDisabledElementByJquery(selector);
        }
        
        /// <summary>
        /// Tries the find element by jQuery.
        /// </summary>
        /// <param name="selector">The selector.</param>       
        /// <param name="timeOut">Max timeout time</param>
        /// <returns>Web element object</returns> 
        protected IWebElement TryFindElementByJquery(string selector, int timeOut)
        {
            return Driver.TryFindElementByJquery(selector, timeOut);
        }

        /// <summary>
        /// Tries the find element by jQuery.
        /// </summary>
        /// <param name="selector">The selector.</param>       
        /// <param name="timeOut">Max timeout time</param>
        /// <param name="isEnabled">Should method check element enable state or not?</param>
        /// <returns>Web element object</returns> 
        protected IWebElement TryFindElementByJquery(string selector, int timeOut, bool isEnabled)
        {
            return Driver.TryFindElementByJquery(selector, timeOut, isEnabled);
        }

        /// <summary>
        /// Tries the find several elements by jQuery.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="timeOut">Max timeout time</param>
        /// <returns>IWebElement's list</returns>
        protected IEnumerable<IWebElement> TryFindElementsByJquery(string selector, int timeOut)
        {
            return Driver.TryFindElementsByJquery(selector, timeOut);
        }

        /// <summary>
        /// Tries the find several elements by jQuery.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>IWebElement's list</returns>
        public List<IWebElement> TryFindElementsByJquery(string selector)
        {
            return Driver.TryFindElementsByJquery(selector);
        }

        /// <summary>
        /// Click on web element using Selenium Click() method.
        /// </summary>
        /// <param name="selector">Web element selector.</param>
        public void ClickElement(string selector)
        {
            ClickElement(selector, CoupleOfSeconds);
        }

        /// <summary>
        /// Click on web element using Selenium Click() method.
        /// </summary>
        /// <param name="selector">Web element selector.</param>
        /// <param name="timeOut">Maximum waiting timeout</param>
        protected void ClickElement(string selector, int timeOut)
        {
            var dif = DateTime.Now + TimeSpan.FromSeconds(timeOut);
            while (DateTime.Now < dif)
            {
                try
                {
                    var elem = Driver.TryFindElementByJquery(selector);
                    if (elem == null) continue;
                        elem.Click();
                        return;
                    }
                catch
                {
                    // ignored
                }
            }
            throw new InvalidElementStateException(string.Format("Element {0} is unreachable", selector));
        }

        /// <summary>
        /// Click on web element using Selenium Click() method.
        /// </summary>
        /// <param name="selector">Web element selector.</param>
        protected void ClickElementByJs(string selector)
        {
            var elem = Driver.TryFindElementByJquery(selector);
            if (elem != null)
            {
                Driver.JsClick(selector);
            }
            else
            {
                throw new ElementNotVisibleException(string.Format("Element {0} is not clickable", selector));
            }
        }

        /// <summary>
        /// Get string value from non-KendoUI web element
        /// </summary>
        /// <param name="selector">Web element selector (non-KendoUI).</param>
        /// <returns>Returns element's value.</returns>
        public string TryGetElementValue(string selector)
        {
            return TryGetElementValue(selector, "unknown");
        }

        /// <summary>
        /// Get string value from web element
        /// </summary>
        /// <param name="selector">Web element selector.</param>     
        /// <param name="elementType">Type of web element. For KendoUI it can be "dropdown", "combobox", "checkbox", or empty for rest of fields.</param>
        /// <returns>Returns element's value.</returns>
        public string TryGetElementValue(string selector, string elementType)
        {
            var webElement = Driver.TryFindElementByJquery(selector, ShortWait, false);
            if (webElement == null)
            {
                return string.Empty;
            }
            var result = DefineKendoUiValue(selector, elementType);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            result = webElement.Text.Trim();
            return !string.IsNullOrEmpty(result) ? result : webElement.GetAttribute("value").Trim();
        }

        /// <summary>
        /// Get KendoUI web element value.
        /// </summary>
        /// <param name="selector">KendoUI element selector</param>
        /// <param name="elementType">KendoUI element type</param>
        /// <returns>Returns element's value.</returns>
        private string DefineKendoUiValue(string selector, string elementType)
        {
            switch (elementType)
            {
                case "dropdown":
                    {
                        return (string)Driver.JsExecuteJavaScript(string.Format("return $('{0}').data('kendoDropDownList').text()", selector));
                    }
                case "combobox":
                    {
                        return (string)Driver.JsExecuteJavaScript(string.Format("return $('{0}').data('kendoComboBox').text()", selector));
                    }
                case "checkbox":
                    {
                        return ((bool)Driver.JsExecuteJavaScript(string.Format("return $('{0}').prop('checked')", selector))).ToString();
                    }
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get text from element(s) immediately - without awaiting "page ready" during seek period. 
        /// Usable for I4C simulator counters.
        /// </summary>
        /// <param name="selector">Selector of placeholder(s)</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns>(string) Returns string.</returns>
        public static string TryGetFullTextNow(string selector, int timeout)
        {
            if (string.IsNullOrEmpty(selector))
                throw new Exception("Selector is empty");
            string result = string.Empty;
            try
            {
                Driver.Wait(d =>
                {
                    try
                    {
                        result = ((IJavaScriptExecutor)Driver)
                            .ExecuteScript(
                                "var result = ''; $('" + selector + "').each(function(c, d)" +
                                "{ result += d.innerHTML; }); return result;")
                            .ToString();

                        return !string.IsNullOrEmpty(result);
                    }
                    catch
                    {
                        return false;
                    }
                },
                TimeSpan.FromSeconds(timeout));
            }
            catch
            {
                return string.Empty;
            }
            return result ?? string.Empty;
        }


        /// <summary>
        /// Drags and drops web element to a specified place on current page. Do not use for Dashboard widgets.
        /// </summary>
        /// <param name="elementToDrag">Web element selector to be gragged.</param>     
        /// <param name="whereToDrop">Target place element selector.</param>
        protected void DragAndDrop(string elementToDrag, string whereToDrop)
        {
            var action = new Actions(CurrentBrowser.WebDriver);
            var element = Driver.TryFindElementByJquery(elementToDrag);
            var place = Driver.TryFindElementByJquery(whereToDrop);
            if (element == null || place == null)
                throw new ElementNotVisibleException("Check are both an element and a place, where element should be dropped, are available");
            action.ClickAndHold(element).MoveToElement(place).Release().Build().Perform();
        }

        /// <summary>
        /// Put mouse pointer over specified web element on current page
        /// </summary>
        /// <param name="elementToMoveMouseOver">Web element selector to move mouse pointer over it.</param>     
        protected void MouseOver(string elementToMoveMouseOver)
        {
            var action = new Actions(Driver);
            action.MoveToElement(TryFindElementByJquery(elementToMoveMouseOver, ShortWait)).Perform();
            // Next 'stupid jumpings' were added because of unexpected 'jump back to top' of mouse pointer in FF (some kind of 'dirty hack')
            action.MoveByOffset(0, -5).Perform();
            action.MoveToElement(TryFindElementByJquery(elementToMoveMouseOver, ShortWait)).Perform();
        }

        /// <summary>
        /// Scrolls current web page to bottom
        /// </summary>       
        private void ScrollToElement(int x, int y)
        {
            Driver.JsExecuteJavaScript(string.Format("window.scrollTo({0},{1});", x, y));
        }

        /// <summary>
        /// Scrolls current web page to bottom
        /// </summary>       
        public void PageScrollDown()
        {
            Driver.WaitForPageLoaded();
            Driver.JsExecuteJavaScript("window.scrollTo(0,document.body.scrollHeight);");
        }

        /// <summary>
        /// Scrolls current web page to top
        /// </summary>       
        protected void PageScrollUp()
        {
            Driver.WaitForPageLoaded();
            Driver.JsExecuteJavaScript("window.scrollTo(0,0);");
        }

        /// <summary>
        /// Waits for page completeness and checks if web element exists on current page (1500 ms timeout)
        /// </summary>
        /// <param name="selector">Web element selector.</param>
        protected bool IsItExist(string selector)
        {
            Driver.WaitForPageLoaded();
            try
            {
                return (Driver.TryFindElementByJquery(selector, CoupleOfSeconds) != null);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check that element is NOT exists after defined time
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>Return true if element is not exists</returns>
        protected bool IsItNotExist(string selector)
        {
            Driver.WaitForPageLoaded();
            var dif = DateTime.Now + TimeSpan.FromSeconds(ShortWait);
            while (DateTime.Now < dif)
            {
                if (Driver.TryFindElementByJquery(selector, OneMoment) == null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check that element is NOT exists during time period
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="time"></param>
        /// <returns>Return true if element is not exists</returns>
        protected bool IsItNotExist(string selector, int time)
        {
            Driver.WaitForPageLoaded();
            var dif = DateTime.Now + TimeSpan.FromSeconds(time);
            while (DateTime.Now < dif)
            {
                if (Driver.TryFindElementByJquery(selector, OneMoment) == null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Drags and drops widget (like Dashboard widget) to another one widget's placeholder.
        /// </summary>
        /// <param name="elementToDrag">Draggable element selector.</param>     
        /// <param name="whereToDrag">Target place (like widget) selector.</param>       
        protected void DragAndDropToOffset(string elementToDrag, string whereToDrag)
        {
            var action = new Actions(Driver);
            var element = Driver.TryFindElementByJquery(elementToDrag);
            var place = Driver.TryFindElementByJquery(whereToDrag);
            if (element == null || place == null)
            {
                throw new ElementNotVisibleException(
                    string.Format("Check whether element '{0}' and place '{1}' are available",
                        elementToDrag,
                        whereToDrag));
            }
            int elementTop = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').offset().top;", elementToDrag)));
            int elementLeft = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').offset().left;", elementToDrag)));          
            int placeTop = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').offset().top;", whereToDrag)));
            int placeLeft = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').offset().left;", whereToDrag)));
            int placeHeight = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').height();", whereToDrag)));
            int placeWidth = Convert.ToInt16(Driver.JsExecuteJavaScript(string.Format("return $('{0}').width();", whereToDrag)));
            var offsetX = (int)(elementTop <= placeTop ? placeHeight * 0.6 : placeHeight * 0.4);
            var offsetY = (int)(elementLeft <= placeLeft ? placeWidth * 0.6 : placeWidth * 0.4);

            ScrollToElement(elementLeft, elementTop);
            action.ClickAndHold(element).Perform();
            action.MoveToElement(place, offsetY, offsetX).Perform();
            action.Release().Perform();
            Driver.WaitForPageLoaded();
        }

        /// <summary>
        /// Returns value from selector
        /// </summary>
        /// <param name="selector">Defined selector</param>
        /// <returns>Returns text of defined selector</returns>
        protected string GetValueFromSelector(string selector)
        {
            return TryFindElementByJquery(selector).Text;
        }

    }
}
