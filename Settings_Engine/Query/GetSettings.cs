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
        [Input("type", "The type of settings you want to obtain.")]
        [Output("settings", "The requested settings if they exist in memory. If they don't exist, a default is returned instead.")]
        public static object GetSettings(Type type)
        {
            ISettings settings = null;
            if (!Global.BHoMSettings.TryGetValue(type, out settings))
            {
                BH.Engine.Base.Compute.RecordWarning($"Could not find settings of type {type} loaded in memory. Returning a default instance of the settings object instead.");
                ISettings obj = null;
                try
                {
                    var activator = Activator.CreateInstance(type);

                    try
                    {
                        obj = activator as ISettings;
                    }
                    catch(Exception ex)
                    {
                        BH.Engine.Base.Compute.RecordError(ex, $"Could not cast object of type {activator.GetType()} to an ISettings object. Settings not saved in memory.");
                        return activator;
                    }
                }
                catch(Exception ex)
                {
                    BH.Engine.Base.Compute.RecordError(ex, $"Could not create instance of object of type {type}.");
                    return null;
                }

                Global.BHoMSettings[type] = obj;

                return obj;
            }

            return settings;
        }
    }
}
