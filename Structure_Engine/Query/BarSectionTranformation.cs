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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Constructs the transformation matrix needed to move the section curves of the Bar from the default drawing position around the global origin to the start of the Bar and aligned with its tangent.")]
        [Input("bar", "The Bar to extract the transformation matrix from. Will make use of the position, tangent and normal of the Bar to generate the matrix.")]
        [Output("transform", "The generated transformation matrix.")]
        public static TransformMatrix BarSectionTranformation(this Bar bar)
        {
            if (bar.IsNull())
                return null;

            Vector trans = bar.Start.Position - Point.Origin;

            Vector gX = Vector.XAxis;
            Vector gY = Vector.YAxis;
            Vector gZ = Vector.ZAxis;

            Vector lX = bar.Tangent(true);
            Vector lZ = bar.Normal();
            Vector lY = lZ.CrossProduct(lX);

            TransformMatrix localToGlobal = new TransformMatrix();

            localToGlobal.Matrix[0, 0] = gX.DotProduct(lX);
            localToGlobal.Matrix[0, 1] = gX.DotProduct(lY);
            localToGlobal.Matrix[0, 2] = gX.DotProduct(lZ);

            localToGlobal.Matrix[1, 0] = gY.DotProduct(lX);
            localToGlobal.Matrix[1, 1] = gY.DotProduct(lY);
            localToGlobal.Matrix[1, 2] = gY.DotProduct(lZ);

            localToGlobal.Matrix[2, 0] = gZ.DotProduct(lX);
            localToGlobal.Matrix[2, 1] = gZ.DotProduct(lY);
            localToGlobal.Matrix[2, 2] = gZ.DotProduct(lZ);
            localToGlobal.Matrix[3, 3] = 1;

            return Engine.Geometry.Create.TranslationMatrix(trans) * localToGlobal * GlobalToSectionAxes;


        }

        /***************************************************/
        /**** Private Property                          ****/
        /***************************************************/


        private static TransformMatrix GlobalToSectionAxes
        {
            get
            {
                Vector gX = Vector.XAxis;
                Vector gY = Vector.YAxis;
                Vector gZ = Vector.ZAxis;

                //Global system vectors, Sections are drawn in global XY plane with y relating to the normal
                Vector lX = Vector.ZAxis;
                Vector lY = Vector.XAxis;
                Vector lZ = Vector.YAxis;

                TransformMatrix transform = new TransformMatrix();



                transform.Matrix[0, 0] = lX.DotProduct(gX);
                transform.Matrix[0, 1] = lX.DotProduct(gY);
                transform.Matrix[0, 2] = lX.DotProduct(gZ);

                transform.Matrix[1, 0] = lY.DotProduct(gX);
                transform.Matrix[1, 1] = lY.DotProduct(gY);
                transform.Matrix[1, 2] = lY.DotProduct(gZ);

                transform.Matrix[2, 0] = lZ.DotProduct(gX);
                transform.Matrix[2, 1] = lZ.DotProduct(gY);
                transform.Matrix[2, 2] = lZ.DotProduct(gZ);

                transform.Matrix[3, 3] = 1;

                return transform;
            }
        }

        /***************************************************/
    }
}






