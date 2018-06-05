using BH.oM.Base;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object PropertyValue(this object obj, string propName)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null)
                return prop.GetValue(obj);
            else if (obj is IBHoMObject)
            {
                IBHoMObject bhom = obj as IBHoMObject;
                if (bhom.CustomData.ContainsKey(propName))
                {
                    Compute.RecordWarning("The returned value is a custom data of the object, not a property");
                    return bhom.CustomData[propName];
                }
                else
                    return null;
            }
            else
                return null;
        }

        /***************************************************/
    }
}
