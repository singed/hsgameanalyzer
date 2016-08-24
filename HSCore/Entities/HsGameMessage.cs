namespace HSCore.Entities
{
    public class HsGameMessage
    {
        public HsGameMessage(HSGameEventTypes type)
        {
            EventType = type;
        }

        public HSGameEventTypes EventType { get; set; }
        private dynamic _data;
        public dynamic Data {
            get { return _data ?? "null"; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return EventType.ToString();
        }
    }
}
