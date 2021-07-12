/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/
        
        [Description("Gets the the dominant vector (orientation) of an Element1D based on its lines lengths.")]
        [Input("element1D", "Element1D to evaluate.")]
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis.")]
        [Input("orthogonalTolerance", "Optional, when orthogonalPriority is true it will only return orthogonal vector when its length is higher this number multiplied by the actual dominant vector lengths. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar. Default value in radians is approximately 5 degrees.")]
        [Output("dominantVector", "The the dominant vector of an Element1D.")]
        public static BH.oM.Geometry.Vector DominantVector(IElement1D element1D, bool orthogonalPriority = true, double orthogonalLengthTolerance = 0.5, double angleTolerance = 0.087)
        {
            List<Vector> vectors = new List<Vector>();
            List<ICurve> curves = element1D.IGeometry().ISubParts().ToList();

            foreach (ICurve curve in curves)
            {                
                //get vector from curves and make all of them positive to better group similar vectors 
                double x = curve.IStartDir().X;
                double y = curve.IStartDir().Y;
                double z = curve.IStartDir().Z;

                Vector vector = BH.Engine.Geometry.Create.Vector(x, y, z);
                vectors.Add(vector*curve.Length());
            }

            //group vectors by direction whilst comparing angle for tolerance
            List<List<Vector>> groupByNormal = GroupSimilarVectorsWithTolerance(vectors, angleTolerance);

            //then sum their total length
            List<double> groupedLengths = groupByNormal.Select(x => x.Select(y => y.Length()).Sum()).ToList();

            //get index of biggest length, which will be dominant vector
            int biggestLengthIndex = groupedLengths.IndexOf(groupedLengths.Max());

            Vector dominantVector = groupByNormal[biggestLengthIndex].First().Normalise();
            
            if(!orthogonalPriority)
                return dominantVector;

            if (dominantVector.X == 0 || dominantVector.X == 1)
                return dominantVector;

            //filter grouped vectors to find only curves that are X = 0 or 1 (horizontal or vertical lines)
            var orthogonalVectors = groupByNormal.Where(x => x.First().Normalise().X == 0 || x.First().Normalise().X == 1).ToList();
            //then sum their total length
            List<double> orthogonalLengths = orthogonalVectors.Select(x => x.Select(y => y.Length()).Sum()).ToList();
            //get index of biggest length, which will be dominant vector
            int biggestOrthogonalLengthIndex = orthogonalLengths.IndexOf(orthogonalLengths.Max());
            
            Vector orthogonalDominantVector = orthogonalVectors[biggestOrthogonalLengthIndex].First().Normalise();
            if (dominantVector.IsEqual(orthogonalDominantVector))
                return dominantVector;
            
            //check if length tolerance passes
            if (orthogonalLengths.Max() < groupedLengths.Max() * orthogonalLengthTolerance)
            {
                return dominantVector;
                //display warning
            }

            return orthogonalDominantVector;
        }

        /******************************************/
        /****            IElement2D            ****/
        /******************************************/
        
        [Description("Gets the the dominant vector (orientation) of an Element2D based on its outter lines lengths.")]
        [Input("element2D", "Element2D to evaluate.")]
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis.")]
        [Input("orthogonalTolerance", "Optional, when orthogonalPriority is true it will only return orthogonal vector when its length is higher this number multiplied by the actual dominant vector lengths. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar. Default value in radians is approximately 5 degrees.")]
        [Output("dominantVector", "The the dominant vector of an Element1D.")]
        public static BH.oM.Geometry.Vector DominantVector(IElement2D element2D, bool orthogonalPriority = true, double orthogonalLengthTolerance = 0.5, double angleTolerance = 0.087)
        {
            IElement1D outline = BH.Engine.Geometry.Create.PolyCurve(element2D.IOutlineElements1D().Select(x =>x as ICurve));
            return DominantVector(outline, orthogonalPriority, orthogonalLengthTolerance, angleTolerance);
        }
        /******************************************/
        /****              Private             ****/
        /******************************************/

        [Description("Groups vectors by direction whilst allowing for an angle discrepancy tolerance.")]
        [Input("vectors", "Vectors to evaluate.")]
        [Input("angleTolerance", "The angle in radians to compare vectors with each other for tolerance when grouping.")]
        [Output("GroupSimilarVectorsWithTolerance", "The grouped vectors.")]
        private static List<List<Vector>> GroupSimilarVectorsWithTolerance(List<Vector> vectors, double angleTolerance)
        {
            List<List<Vector>> result = new List<List<Vector>>();
            List<Vector> orderByLength = vectors.OrderBy(x => x.Length()).ToList();

            for (int i = 0; i < orderByLength.Count; i++)
            {
                List<Vector> sublist = new List<Vector>();
                sublist.Add(orderByLength[i]);
                
                for (int j = 0; j < orderByLength.Count; j++)
                {
                    if(!orderByLength[i].Equals(orderByLength[j]))
                    {
                        if (orderByLength[i].Angle(orderByLength[j]) <= angleTolerance)
                        {
                            sublist.Add(orderByLength[j]);
                            orderByLength.RemoveAt(j);
                            j = j - 1;
                        }
                    }
                }
                orderByLength.RemoveAt(i);
                i = i - 1;
                result.Add(sublist);
            }

            return result;
        }

        /******************************************/
    }
}

