using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return new CustomObject
            {
                CustomData = customData,
                Name = name
            };
        }

        /***************************************************/
    }
}
