using System;
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.Engine.Serialiser
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void ApplyTaggedName<T>(this T obj, string str) where T : IObject
        {
            if (string.IsNullOrWhiteSpace(str))
                return;

            string[] arr = str.Split(new string[] { "__Tags__:" }, StringSplitOptions.None);

            obj.Name = arr[0].TrimEnd(' ');

            if (arr.Length >= 2)
                obj.Tags = new HashSet<string>(arr[1].Split(new string[] { "_/_" }, StringSplitOptions.None));
        }

        /***************************************************/
    }
}
