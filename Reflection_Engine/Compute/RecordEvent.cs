using BH.oM.Reflection.Debuging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool RecordEvent(string message, EventType type = EventType.Unknown)
        {
            return RecordEvent(new Event { Message = message, Type = type });
        }

        /***************************************************/

        public static bool RecordError(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Error });
        }

        /***************************************************/

        public static bool RecordWarning(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Warning });
        }

        /***************************************************/

        public static bool RecordNote(string message)
        {
            return RecordEvent(new Event { Message = message, Type = EventType.Note });
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool RecordEvent(Event newEvent)
        {
            string trace = Environment.StackTrace;
            newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());

            Log log = Query.DebugLog();
            log.AllEvents.Add(newEvent);
            log.CurrentEvents.Add(newEvent);

            return true;
        }


        /***************************************************/
    }
}
