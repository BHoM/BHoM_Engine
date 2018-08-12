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
                    return bhom.CustomData[propName];
                }
                else
                {
                    Compute.RecordWarning("The object does not contain a property of that name");
                    return null;
                }
                    
            }
            else
                return null;
        }

        /***************************************************/
    }
}
