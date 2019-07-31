/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates wall base on given construction, bottom curve and height")]
        [Input("construction", "Construction of the wall")]
        [Input("bottomEdge", "Curve representing bottom edge of the wall")]
        [Input("height", "Wall height")]
        [Output("Physical Wall")]
        public static Wall Wall(Construction construction, ICurve bottomEdge, double height)
        {
            if (construction == null || bottomEdge == null || height <= 0)
                return null;

            if (Geometry.Query.IIsClosed(bottomEdge))
                return null;

            Point aPoint_1 = Geometry.Query.IStartPoint(bottomEdge);
            Point aPoint_2 = Geometry.Query.IEndPoint(bottomEdge);

            ICurve aICurve = Geometry.Modify.ITranslate(bottomEdge, Geometry.Create.Vector(0, 0, height));

            Line aLine_1 = Geometry.Create.Line(Geometry.Query.IEndPoint(bottomEdge), Geometry.Query.IStartPoint(aICurve));
            Line aLine_2 = Geometry.Create.Line(Geometry.Query.IEndPoint(aICurve), Geometry.Query.IStartPoint(bottomEdge));


            PolyCurve aPolyCurve = Geometry.Create.PolyCurve(new ICurve[] { bottomEdge, aLine_1, aICurve, aLine_2 });

            Wall aWall = new Wall()
            {
                Construction = construction,
                Location = Geometry.Create.PlanarSurface(aPolyCurve)
            };

            return aWall;
        }

        /***************************************************/

        [Description("Creates Wall object.")]
        [Input("construction", "Construction of the wall")]
        [Input("edges", "Edges of Wall (profile)")]
        [Output("Physical Wall")]
        public static Wall Wall(Construction construction, ICurve edges, IEnumerable<ICurve> internalEdges = null)
        {
            if (construction == null || edges == null)
                return null;

            PlanarSurface aPlanarSurface = Geometry.Create.PlanarSurface(edges);
            if (aPlanarSurface == null)
                return null;

            Wall aWall = new Wall()
            {
                Construction = construction,
                Location = aPlanarSurface
            };

            return aWall;
        }

        /***************************************************/
    }
}
