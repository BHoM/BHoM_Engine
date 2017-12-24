namespace BH.Engine.Reflection
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool SetPropertyValue(this object obj, string propName, object value)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null)
            {
                prop.SetValue(obj, value);
                return true;
            }

            return false;
        }
    }
}
