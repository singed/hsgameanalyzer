using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSCore.Entities
{
    public class HSTurn
    {
        protected HSGameEventTypes EventType { get; set; }
        public bool Data { get; set; }
        private string _description;
        protected string Description {
            get { return Data ? "opponent's turn" : "my turn"; }
            set { _description = value; }
        }
    }
}
