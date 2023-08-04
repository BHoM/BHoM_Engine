using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System.Text;

namespace BH.Engine.Settings
{
    public static partial class Query
    {
        [Description("Obtain settings of the specified type if they exist in memory.")]
        [Output("settings", "The requested settings if they exist in memory. If they don't exist, a default is returned instead.")]
        public static T GetSettings<T>()
        {
            Type type = typeof(T);
            ISettings settings = null;
            if(!Global.BHoMSettings.TryGetValue(type, out settings))
            {
                BH.Engine.Base.Compute.RecordError($"Could not find settings of type {type} loaded in memory.");
                return default(T);
            }

            return (T)settings;
        }
    }
}
