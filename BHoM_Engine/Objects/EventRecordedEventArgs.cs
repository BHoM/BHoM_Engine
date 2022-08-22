using BH.oM.Base.Debugging;
using System;

namespace BH.Engine.Base.Objects
{
    public class EventRecordedEventArgs : EventArgs
    {

        /***************************************************/
        /****             Public properties             ****/
        /***************************************************/

        public Event RecordedEvent { get; }


        /***************************************************/
        /****            Public constructor             ****/
        /***************************************************/

        public EventRecordedEventArgs(Event recordedEvent)
        {
            RecordedEvent = recordedEvent;
        }

        /***************************************************/
    }
}
