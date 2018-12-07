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

        [Description("Get the abbreviated name of the given property from an object")]
        [Input("obj", "Object to query the property from")]
        [Input("propName", "Name of abbreviated property")]
        [Output("abbreviation", "Abbreviated property name")]
        public static string PropertyAbbreviation(this object obj, string propName)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null)
            {
                object[] attributes = prop.GetCustomAttributes(typeof(AbbreviationAttribute), false);
                if (attributes.Length == 1)
                {
                    AbbreviationAttribute attribute = (AbbreviationAttribute)attributes[0];
                    if (attribute != null)
                        return attribute.Name;
                }
            }
            
            return "";
        }

        /***************************************************/
    }
}
