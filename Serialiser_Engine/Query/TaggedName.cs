using BH.oM.Base;

namespace BH.Engine.Serialiser
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string TaggedName(this IBHoMObject obj)
        {
            string str = string.IsNullOrWhiteSpace(obj.Name) ? "" : obj.Name;

            if (obj.Tags.Count > 0)
            {
                str += " __Tags__:";

                foreach (string tag in obj.Tags)
                {
                    str += tag + "_/_";
                }

                str = str.TrimEnd("_/_");
            }

            return str;

        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }

        /***************************************************/

    }
}
