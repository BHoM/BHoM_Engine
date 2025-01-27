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
using BH.oM.Base.Attributes;
using BH.oM.Facade.Elements;
using BH.oM.Geometry;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a CurtainWall at its centre. Method required for automatic display in UI packages.")]
        [Input("curtainWall", "CurtainWall to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the CurtainWall at its centre.")]
        public static PlanarSurface Geometry(this CurtainWall curtainWall)
        {
            return Engine.Geometry.Create.PlanarSurface(curtainWall?.ExternalEdges?.Select(x => x?.Curve).ToList().IJoin().FirstOrDefault());
        }

        /***************************************************/

        [Description("Gets the geometry of a facade Opening as an outline curve. Method required for automatic display in UI packages.")]
        [Input("opening", "Facade Opening to get the outline geometry from.")]
        [Output("outline", "The geometry of the facade Opening.")]
        public static PolyCurve Geometry(this Opening opening)
        {
            return new PolyCurve { Curves = opening?.Edges?.Select(x => x?.Curve).ToList().IJoin().Cast<ICurve>().ToList() };
        }

        /***************************************************/
    }
}

