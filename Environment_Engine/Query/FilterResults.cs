using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Results.Mesh;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Filter a collection of Environment MeshResult objects to those which match the given object ID")]
        [Input("results", "A collection of Environment MeshResult objects to filter")]
        [Input("objectID", "The Object ID to filter by")]
        [Output("filteredResults", "The collection of Environment MeshResults which have the provided object ID")]
        public static List<MeshResult> FilterResultsByObjectID(this List<MeshResult> results, IComparable objectID)
        {
            return results.Where(x => x.ObjectId == objectID).ToList();
        }

        [Description("Filter a collection of Environment MeshResult objects to those which match the given result case")]
        [Input("results", "A collection of Environment MeshResult objects to filter")]
        [Input("resutlCase", "The Result Case to filter by")]
        [Output("filteredResults", "The collection of Environment MeshResults which have the provided result case")]
        public static List<MeshResult> FilterResultsByResultCase(this List<MeshResult> results, IComparable resultCase)
        {
            return results.Where(x => x.ResultCase == resultCase).ToList();
        }
    }
}
