namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static string ILabel(this object obj)
        {
            if (obj == null)
                return "null";

            object label;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Label), new object[] { }, out label))
            {
                BH.Engine.Base.Compute.RecordError($"Could not get label for an object of type {obj.GetType().FullName}.");
                return null;
            }

            return label as string;
        }

        /***************************************************/

        public static string Label(this object obj)
        {
            return obj?.GetType().Name ?? "null";
        }

        /***************************************************/
    }
}
