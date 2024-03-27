/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PileFoundation from a pile cap and piles.")]
        [Input("pileCap", "The pile cap defining the outer edge of the PileFoundation and location in 3D.")]
        [Input("piles", "Piles contained within the outline of the pile cap.")]
        [Output("pileFoundation", "The created PileFoundation containing the pile cap and pile elements.")]
        public static PileFoundation PileFoundation(PadFoundation pileCap, List<Pile> piles)
        {
            if (piles.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The pile list is null or empty.");
                return null;
            }

            if (piles.Any(x => x.IsNull()))
                return null;

            if (pileCap.IsNull())
                return null;

            return new PileFoundation() { PileCap = pileCap, Piles = piles };
        }

        /***************************************************/

        [Description("Creates a structural PileFoundation from the piles, an offset and the PileCap properties using the GrahamScan method to determine the pile cap outline.")]
        [Input("piles", "One or more Piles used to define the outline of the PileCap.")]
        [Input("offset", "The offset from the centre of the piles to the edge of the PileCap.")]
        [InputFromProperty("property")]
        [InputFromProperty("orientationAngle")]
        [Output("pileFoundation", "The created PileFoundation containing the dervied pile cap and pile elements.")]
        public static PileFoundation PileFoundation(List<Pile> piles, double offset = 0, ConstantThickness property = null, double orientationAngle = 0)
        {
            List<Point> pts = new List<Point>();

            foreach (Pile pile in piles)
            {
                pts.Add(pile.TopNode.Position);
            }

            List<Point> convexHull = Geometry.Compute.GrahamScan(pts);
            convexHull.Add(convexHull[0]);

            return PileFoundation(PadFoundation((PolyCurve)Geometry.Create.Polyline(convexHull).Offset(offset), property, orientationAngle), piles);
        }

        /***************************************************/

    }
}
