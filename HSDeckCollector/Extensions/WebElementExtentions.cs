using OpenQA.Selenium;

namespace HSDeckCollector.Extensions
{
    /// <summary>
    /// IWebElements extensions
    /// </summary>
    public static class WebElementExtentions
    {
        /// <summary>
        /// Remove typed text
        /// </summary>
        /// <param name="element"></param>
        public static void ClearInput(this IWebElement element)
        {
            var l = element.GetAttribute("value").Length;
            for (var i = 0; i < l; i++)
                element.SendKeys(Keys.Backspace);
        }

        /// <summary>
        /// Input text "one-by-one" character with Enter keypress at the end
        /// </summary>
        /// <param name="element">Web element</param>
        /// <param name="text">Text to input</param>
        public static void InputText(this IWebElement element, string text)
        {
            element.SendKeys(" ");
            foreach (var c in text)
                element.SendKeys(c.ToString());
            element.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Input text "one-by-one" character
        /// </summary>
        /// <param name="element">Web element</param>
        /// <param name="text">Text to input</param>
        public static void InputFormText(this IWebElement element, string text)
        {
            foreach (var c in text)
                element.SendKeys(c.ToString());
        }
    }
}
