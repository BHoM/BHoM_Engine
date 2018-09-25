using BH.oM.DataManipulation.Queries;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Results;
using BH.oM.Geometry;

namespace BH.Engine.Structure.Results
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
         
        [Description("Specific filter query to retrieve structural mesh results")]

        public static FilterQuery MeshResult(   MeshResultSmoothingType smoothing, 
                                                MeshResultLayer layer,
                                                double layerPosition,
                                                MeshResultType resultType, 
                                                CoordinateSystem coordinateSystem = null,                                           
                                                IEnumerable<object> cases = null, 
                                                IEnumerable<object> objectIds = null)
        {
            FilterQuery query = new FilterQuery();
            query.Type = typeof(MeshResults);

            query.Equalities["Smoothing"] = smoothing;
            query.Equalities["Layer"] = layer;
            query.Equalities["LayerPosition"] = layerPosition;
            query.Equalities["ResultType"] = resultType;
            query.Equalities["CoordinateSystem"] = coordinateSystem;
            if (cases != null)
                query.Equalities["Cases"] = cases.ToList();
            if (objectIds != null)
                query.Equalities["ObjectIds"] = objectIds.ToList();

            return query;
        }

        /***************************************************/
    }
}
