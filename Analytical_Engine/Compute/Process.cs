using System.Collections.Generic;
using System.ComponentModel;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Physical.Elements;

namespace BH.Engine.Analytical
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Execute processes assigned to a relation")]
        public static List<ProcessResult> Process(this ProcessRelation processRelation, IBHoMObject source, IBHoMObject target )
        {
            List<ProcessResult> results = new List<ProcessResult>();

            foreach (IProcess process in processRelation.Processes)
                results.Add(process.IProcess( source, target));

            return results;
        }
        /***************************************************/

        [Description("Execute processes assigned to a relation")]
        public static ProcessResult IProcess(this IProcess process, IBHoMObject source, IBHoMObject target)
        {
            return Process(process as dynamic, source, target);
        }
        /***************************************************/

        [Description("Execute processes assigned to a relation")]
        public static ProcessResult Process(this ColumnGridProcess process, IBHoMObject source, IBHoMObject target)
        {
            IElement0D sourceElement = source as IElement0D;
            IElement0D targetElement = target as IElement0D;
            double distance = Geometry.Query.Distance(sourceElement.IGeometry(), targetElement.IGeometry());
            bool passed = distance < process.Tolerance;
            ColumnGridResult processResult = new ColumnGridResult(process.BHoM_Guid, "column grid result", -1, distance, passed);
            return processResult;
        }
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static ProcessResult Process(this IProcess process, IBHoMObject source, IBHoMObject target)
        {
            // Do nothing
            return null;
        }

        /***************************************************/
    }
}
