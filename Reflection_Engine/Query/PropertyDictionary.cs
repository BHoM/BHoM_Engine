using BH.oM.Base;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, object> GetPropertyDictionary(this object obj)
        {
            return _GetPropertyDictionary(obj as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static Dictionary<string, object> _GetPropertyDictionary(this object obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || prop.GetMethod.GetParameters().Count() > 0) continue;
                var value = prop.GetValue(obj, null);
                dic[prop.Name] = value;
            }
            return dic;
        }

        /***************************************************/

        public static Dictionary<string, object> _GetPropertyDictionary(this CustomObject obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>(obj.CustomData);

            if (!dic.ContainsKey("Name"))
                dic["Name"] = obj.Name;

            if (!dic.ContainsKey("BHoM_Guid"))
                dic["BHoM_Guid"] = obj.BHoM_Guid;

            if (!dic.ContainsKey("Tags"))
                dic["Tags"] = obj.Tags;

            return dic;
        }
    }
}
