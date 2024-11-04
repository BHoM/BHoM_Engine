using BH.Engine.Versioning;
using BH.oM.Dimensional;
using BH.oM.Structure.Elements;
using System.Reflection;

namespace BH.Engine.TestHelper
{
    public static partial class Compute
    {
        public static string ExtensionMethodToCallHelper(this Bar a, double b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, Bar b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, object c, object d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, Panel c, object d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this IElement2D a, double b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string IExtensionMethodToCallHelper(this object a, object b, object c, object d)
        {
            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(a, nameof(ExtensionMethodToCallHelper), new object[] { b, c, d }, out result))
            {
                BH.Engine.Base.Compute.RecordError("Extension method not found.");
                return null;
            }
            else
                return (string)result;
        }
    }
}
