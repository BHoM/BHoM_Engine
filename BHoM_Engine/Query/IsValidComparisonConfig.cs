using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        public static bool IsValidComparisonConfig(BaseComparisonConfig cc)
        {
            if (cc == null) { return false; }

            bool isValid = true;

            if (cc.UseGeometryHash) 
            {
                if (cc.TypeExceptions.Where(t => typeof(IGeometry).IsAssignableFrom(t)).Any())
                    return InvalidComparisonConfig(nameof(cc.TypeExceptions));

                if (cc.PropertyExceptions.Where(p => p.Contains("Geometry")).Any())
                    return InvalidComparisonConfig(nameof(cc.TypeExceptions));
            }
        }

        private static bool InvalidComparisonConfig(string invalidProp)
        {
            BH.Engine.Base.Compute.RecordError($"The comparison config is not valid because it has {nameof(BaseComparisonConfig.UseGeometryHash)} set to true while specifying also a {invalidProp} that applies to geometric types." +
                $"If you want to specify this {invalidProp}, you need to toggle off {nameof(BaseComparisonConfig.UseGeometryHash)}.");

            return false;
        }
    }
}
