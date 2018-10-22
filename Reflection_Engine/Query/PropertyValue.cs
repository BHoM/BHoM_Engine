using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System.Collections;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the value of a property with a given name from an object")]
        [Input("obj", "object to get the value from")]
        [Input("propName", "name of the property to get the value from")]
        [Output("value", "value of the property")]
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
            else if (obj is IDictionary)
            {
                IDictionary dic = obj as IDictionary;
                if (dic.Contains(propName))
                    return dic[propName];
                else
                    return null;
            }
            else
                return null;
        }

        /***************************************************/
    }
}
