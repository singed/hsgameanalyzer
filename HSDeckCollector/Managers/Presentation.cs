using HSDeckCollector.Core;
using HSDeckCollector.Extensions;

namespace HSDeckCollector.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class Presentation : ActionManager
    {
        /// <summary>
        /// Just constructor
        /// </summary>
        /// <param name="browser"></param>
        public Presentation(Browser browser) :
            base(browser)
        {

        }

        /// <summary>
        /// Display message with defined text during time interval
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="miliseconds">Time interval in miliseconds</param>
        public void Message(string message, int miliseconds)
        {
            string style = "position:absolute;top:40%;left:20%;background-color:white;font-size:2.5em;font-weight:900;font-family:Arial,Helvetica,sans-serif;";
            Driver.JsExecuteJavaScript("function tempAlert(msg,duration){var el = document.createElement(\"div\");el.setAttribute(\"style\",\"" + style + "\");el.innerHTML = msg;setTimeout(function(){el.parentNode.removeChild(el);},duration);document.body.appendChild(el);}tempAlert('" + message + "', " + miliseconds.ToString() + ");");
        }
    }
}
