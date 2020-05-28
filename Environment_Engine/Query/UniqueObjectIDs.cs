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
        [Description("Returns a collection of unique Object IDs from a collection of MeshResult objects")]
        [Input("meshResults", "A collection of Environment Mesh Results to obtain the unique object IDs from")]
        [Output("uniqueIDs", "The collection of unique object IDs from the Mesh Results")]
        public static List<IComparable> UniqueObjectIDs(this List<MeshResult> meshResults)
        {
            return meshResults.Select(x => x.ObjectId).Distinct().ToList();
        }
    }
}
