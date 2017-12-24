namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object GetPropertyValue(this object obj, string propName)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);
            if (prop == null) return null;

            return prop.GetValue(obj);
        }
        
    }
}
