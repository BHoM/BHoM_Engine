/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/
        
        [Description("Gets the dominant vector (orientation) of an Element1D based on its line lengths.")]
        [Input("element1D", "Element1D to evaluate.")]
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis (X, Y or Z vectors).")]
        [Input("orthogonalLengthFactor", "Optional, tests the orthogonal vector lengths in relation to the actual non-orthogonal dominant vector. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5 for it to pass the test.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar.")]
        [Output("dominantVector", "The dominant vector of an Element1D.")]
        public static BH.oM.Geometry.Vector DominantVector(this IElement1D element1D, bool orthogonalPriority = true, double orthogonalLengthFactor = 0.5, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (element1D == null || orthogonalPriority == null || orthogonalLengthFactor == null || angleTolerance == null)
            {
                BH.Engine.Base.Compute.RecordError("One or more of the inputs is empty or null.");
                return null;
            }

            List<ICurve> curves = element1D.IGeometry().ISubParts().ToList();
            
            if(!curves.Any(x=> x.IIsLinear()))
                BH.Engine.Base.Compute.RecordWarning("Non-linear curves are using an approximate vector between its start and end.");
            
            List<Vector> vectors = curves.Select(x => (x.IStartPoint() -x.IEndPoint())).ToList();

            //group vectors by direction whilst comparing angle for tolerance
            List<List<Vector>> groupByNormal = GroupSimilarVectorsWithTolerance(vectors, angleTolerance);
            groupByNormal = groupByNormal.OrderByDescending(x => x.Sum(y => y.Length())).ToList();
            List<Vector> largestGlobal = groupByNormal[0];

            Vector dominantVector = largestGlobal[0].Normalise();
            if (!orthogonalPriority)
                return dominantVector;
            
            List<Vector> largestOrthogonal = groupByNormal.FirstOrDefault(x => (x.First().IsOrthogonal(angleTolerance)));
            if (largestOrthogonal != null)
            {
                if (largestGlobal.Sum(x => x.Length()) * orthogonalLengthFactor > largestOrthogonal.Sum(x => x.Length()))
                    BH.Engine.Base.Compute.RecordWarning("Orthogonal vector was found but didn't pass the length tolerance in relation to the actual non-orthogonal dominant vector. The actual dominant vector is the output.");
                else
                    dominantVector = largestOrthogonal[0].Normalise();
            }

            return dominantVector;
        }

        /******************************************/
        /****            IElement2D            ****/
        /******************************************/
        
        [Description("Gets the dominant vector (orientation) of an Element2D based on its line lengths.")]
        [Input("element2D", "Element2D to evaluate.")]
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis (X, Y or Z vectors).")]
        [Input("orthogonalLengthFactor", "Optional, tests the orthogonal vector lengths in relation to the actual non-orthogonal dominant vector. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5 for it to pass the test.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar.")]
        [Output("dominantVector", "The dominant vector of an Element2D.")]
        public static BH.oM.Geometry.Vector DominantVector(this IElement2D element2D, bool orthogonalPriority = true, double orthogonalLengthFactor = 0.5, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (element2D == null || orthogonalPriority == null || orthogonalLengthFactor == null || angleTolerance == null)
            {
                BH.Engine.Base.Compute.RecordError("One or more of the inputs is empty or null.");
                return null;
            }

            IElement1D outline = BH.Engine.Geometry.Create.PolyCurve(element2D.IOutlineElements1D().Select(x =>x.IGeometry()));
            return DominantVector(outline, orthogonalPriority, orthogonalLengthFactor, angleTolerance);
        }
        
        /******************************************/
        /****              Private             ****/
        /******************************************/

        [Description("Groups vectors by direction whilst allowing for an angle discrepancy tolerance.")]
        [Input("vectors", "Vectors to evaluate.")]
        [Input("angleTolerance", "The angle in radians to compare vectors with each other for tolerance when grouping.")]
        [Output("groupedVectors", "The grouped vectors.")]
        private static List<List<Vector>> GroupSimilarVectorsWithTolerance(List<Vector> vectors, double angleTolerance)
        {
            List<List<Vector>> result = new List<List<Vector>>();
            List<Vector> orderByLength = vectors.OrderByDescending(x => x.Length()).ToList();

            while (orderByLength.Count != 0)
            {
                List<Vector> sublist = new List<Vector>();
                sublist.Add(orderByLength[0]);
                
                for (int i = orderByLength.Count - 1; i > 0; i--)
                {
                    if (orderByLength[0].IsParallel(orderByLength[i],angleTolerance) != 0)
                    {
                        sublist.Add(orderByLength[i]);
                        orderByLength.RemoveAt(i);
                    }
                }
                orderByLength.RemoveAt(0);
                result.Add(sublist);
            }

            return result;
        }

        /******************************************/
    }
}




