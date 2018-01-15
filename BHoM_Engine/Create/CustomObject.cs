using BH.oM.Base;
using System.Collections.Generic;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CustomObject CustomObject(Dictionary<string, object> data, string name = "")
        {
            return new CustomObject
            {
                CustomData = new Dictionary<string, object>(data),
                Name = name
            };
        }

        /***************************************************/

        public static CustomObject CustomObject(List<string> propertyNames, List<object> propertyValues, string name = "")
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            if (propertyNames.Count == propertyValues.Count)
            {
                for (int i = 0; i < propertyValues.Count; i++)
                    customData.Add(propertyNames[i], propertyValues[i]);
            }
            else
                throw new System.Exception("The list of property names must be the same length as the list of property values when creating a Custon object.");

            return new CustomObject
            {
                CustomData = customData,
                Name = name
            };
        }

        /***************************************************/
    }
}
