using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        [Description("Verifies that, when the ComparisonConfig requests the GeometryHash workflow, it is NOT at the same time requesting a numerical approximation for objects belonging to Geometrical types.")]
        [Output("True if the ComparisonConfig is compatible with the GeometryHash worfklow.")]
        public static bool IsCompatibleWithGeometryHash(this BaseComparisonConfig cc, bool raiseWarning = true)
        {
            if (cc == null) { return false; }

            bool isCompatible = true;

            if (!cc.UseGeometryHash)
                return true;

            if (cc.NumericTolerance != double.MinValue)
            {
                if (raiseWarning) Compute.RecordWarning($"Please note that the input {nameof(ComparisonConfig)}.{nameof(cc.NumericTolerance)} will not be considered for Geometry objects because {nameof(cc.UseGeometryHash)} was set to true.");
                isCompatible = false;
            }

            if (cc.SignificantFigures != int.MaxValue)
            {
                if (raiseWarning) Compute.RecordWarning($"Please note that the input {nameof(ComparisonConfig)}.{nameof(cc.SignificantFigures)} will not be considered for Geometry objects because {nameof(cc.UseGeometryHash)} was set to true.");
                isCompatible = false;
            }

            if (cc.PropertyNumericTolerances?.Where(p => p.Name.Contains("Geometry")).Any() ?? false)
            {
                if (raiseWarning) Compute.RecordWarning($"Please note that the input {nameof(ComparisonConfig)}.{nameof(cc.PropertyNumericTolerances)} will not be considered for Geometry objects because {nameof(cc.UseGeometryHash)} was set to true.");
                isCompatible = false;
            }

            if (cc.PropertySignificantFigures?.Where(p => p.Name.Contains("Geometry")).Any() ?? false)
            {
                if (raiseWarning) Compute.RecordWarning($"Please note that the input {nameof(ComparisonConfig)}.{nameof(cc.PropertySignificantFigures)} will not be considered for Geometry objects because {nameof(cc.UseGeometryHash)} was set to true.");
                isCompatible = false;
            }

            return isCompatible;
        }

        private static bool InvalidComparisonConfig(string invalidProp)
        {
            BH.Engine.Base.Compute.RecordError($"The comparison config is not valid because it has {nameof(BaseComparisonConfig.UseGeometryHash)} set to true while specifying also a {invalidProp} that applies to geometric types." +
                $"If you want to specify this {invalidProp}, you need to toggle off {nameof(BaseComparisonConfig.UseGeometryHash)}.");

            return false;
        }
    }
}
