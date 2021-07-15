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
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis (X and Y vectors.")]
        [Input("orthogonalLengthTolerance", "Optional, tests the orthogonal vector length's in relation to the actual non-orthogonal dominant vector. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5 for it to pass the test.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar.")]
        [Output("dominantVector", "The dominant vector of an Element1D.")]
        public static BH.oM.Geometry.Vector DominantVector(IElement1D element1D, bool orthogonalPriority = true, double orthogonalLengthTolerance = 0.5, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            List<ICurve> curves = element1D.IGeometry().ISubParts().ToList();
            List<Vector> vectors = curves.Select(x => x.IStartDir() * x.Length()).ToList();
            
            /*foreach (ICurve curve in curves)
            {                
                //get vector from curves and make all of them positive to better group similar vectors 
                double x = curve.IStartDir().X;
                double y = curve.IStartDir().Y;
                double z = curve.IStartDir().Z;
                
                if (x < 0)
                    x = x * -1;
                if (y < 0)
                    y = y * -1;
                if (z < 0)
                    z = z * -1;

                Vector vector = BH.Engine.Geometry.Create.Vector(x, y, z);
                vectors.Add(vector*curve.Length());
            }*/

            //group vectors by direction whilst comparing angle for tolerance
            List<List<Vector>> groupByNormal = GroupSimilarVectorsWithTolerance(vectors, angleTolerance);

            //then sum their total length
            List<double> groupedLengths = groupByNormal.Select(x => x.Select(y => y.Length()).Sum()).ToList();

            //get index of biggest length, which will be dominant vector
            int biggestLengthIndex = groupedLengths.IndexOf(groupedLengths.Max());

            Vector dominantVector = groupByNormal[biggestLengthIndex].First().Normalise();
            
            if(!orthogonalPriority)
                return dominantVector;
            else if (!element1D.IIsPlanar())
            {
                BH.Engine.Reflection.Compute.RecordWarning("Calculation of orthogonal vectors for non-planar curves is not implemented. The original dominant vector is returned instead.");
                return dominantVector;
            }

            if (dominantVector.X == 0 || dominantVector.X == 1)
                return dominantVector;

            //filter grouped vectors to find only curves that are fully in X, Y or Z vectors (orthogonal planes)
            Vector compareVectorX = BH.Engine.Geometry.Create.Vector(1, 0, 0);
            Vector compareVectorXn = BH.Engine.Geometry.Create.Vector(-1, 0, 0);
            Vector compareVectorY = BH.Engine.Geometry.Create.Vector(0, 1, 0);
            Vector compareVectorYn = BH.Engine.Geometry.Create.Vector(0, -1, 0);
            Vector compareVectorZ = BH.Engine.Geometry.Create.Vector(0, 0, 1);
            Vector compareVectorZn = BH.Engine.Geometry.Create.Vector(0, 0, -1);
            var orthogonalVectors = groupByNormal.Where(x => x.First().Normalise().IsEqual(compareVectorX) 
                                                             || x.First().Normalise().IsEqual(compareVectorXn)
                                                             || x.First().Normalise().IsEqual(compareVectorY)
                                                             || x.First().Normalise().IsEqual(compareVectorYn)
                                                             || x.First().Normalise().IsEqual(compareVectorZ)
                                                             || x.First().Normalise().IsEqual(compareVectorZn)).ToList();
            //then sum their total length
            List<double> orthogonalLengths = orthogonalVectors.Select(x => x.Select(y => y.Length()).Sum()).ToList();
            //get index of biggest length, which will be dominant vector
            int biggestOrthogonalLengthIndex = orthogonalLengths.IndexOf(orthogonalLengths.Max());
            
            Vector orthogonalDominantVector = orthogonalVectors[biggestOrthogonalLengthIndex].First().Normalise();
            
            //check if length tolerance passes
            if (orthogonalLengths.Max() < groupedLengths.Max() * orthogonalLengthTolerance)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Orthogonal vector was found but didn't pass the length tolerance in relation to the actual non-orthogonal dominant vector. The actual dominant vector is the output.");
                return dominantVector;
            }

            return orthogonalDominantVector;
        }

        /******************************************/
        /****            IElement2D            ****/
        /******************************************/
        
        [Description("Gets the the dominant vector (orientation) of an Element2D based on its lines lengths.")]
        [Input("element1D", "Element2D to evaluate.")]
        [Input("orthogonalPriority", "Optional, if true gives priority to curves that are on the orthogonal axis (X and Y vectors.")]
        [Input("orthogonalLengthTolerance", "Optional, tests the orthogonal vector length's in relation to the actual non-orthogonal dominant vector. For example if the dominant vector is 10 in length but the orthogonal is only 5 in length, then this number should be 0.5 for it to pass the test.")]
        [Input("angleTolerance", "Optional, angle in radians that vectors will be considered similar.")]
        [Output("dominantVector", "The dominant vector of an Element2D.")]
        public static BH.oM.Geometry.Vector DominantVector(IElement2D element2D, bool orthogonalPriority = true, double orthogonalLengthTolerance = 0.5, double angleTolerance = 0.087)
        {
            IElement1D outline = BH.Engine.Geometry.Create.PolyCurve(element2D.IOutlineElements1D().Select(x =>x.IGeometry()));
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
                
                for (int j = 1; j < orderByLength.Count; j++)
                {
                    if (orderByLength[i].Angle(orderByLength[j]) <= angleTolerance)
                    {
                        sublist.Add(orderByLength[j]);
                        orderByLength.RemoveAt(j);
                        j = j - 1;
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

