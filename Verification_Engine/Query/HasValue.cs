using BH.Engine.Base;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static bool? IHasValue(this object obj)
        {
            if (obj == null)
                return false;

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(HasValue), out result))
            {
                BH.Engine.Base.Compute.RecordError($"Can't check if object has value because type {obj.GetType().Name} is currently not supported.");
                return null;
            }

            return (bool?)result;
        }

        public static bool? HasValue(this double obj)
        {
            return !double.IsNaN(obj);
        }

        public static bool? HasValue(this string obj)
        {
            return !string.IsNullOrEmpty(obj);
        }

        public static bool? HasValue(this object obj)
        {
            return obj != null;
        }
    }
}
