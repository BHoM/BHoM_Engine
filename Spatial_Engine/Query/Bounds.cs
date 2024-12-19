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
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Queries the IElement0Ds BoundingBox. Acts on the Point definition of the IElement0D through the Geometry_Engine.")]
        [Input("element0D", "The IElement0D with the geometry to get the BoundingBox of. i.e. no extent or equivalent will be considered.")]
        [Output("bounds", "A BoundingBox encapsulating the Point definition of the IElement0D.")]
        public static BoundingBox Bounds(this IElement0D element0D)
        {
            return Engine.Geometry.Query.Bounds(element0D.IGeometry());
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Queries the IElement1Ds BoundingBox. Acts on the ICurve definition of the IElement1D through the Geometry_Engine.")]
        [Input("element1D", "The IElement1D with the geometry to get the BoundingBox of. i.e. no cross-section or equivalent will be considered.")]
        [Output("bounds", "A BoundingBox encapsulating the ICurve definition of the IElement1D.")]
        public static BoundingBox Bounds(this IElement1D element1D)
        {
            return Engine.Geometry.Query.IBounds(element1D.IGeometry());
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the IElement2Ds BoundingBox. Acts on the element curve definition of the IElement2D through the Geometry_Engine.")]
        [Input("element2D", "The IElement2D with the geometry to get the BoundingBox of. i.e. no thickness or equivalent will be considered.")]
        [Output("bounds", "A BoundingBox encapsulating the element curve definition of the IElement2D.")]
        public static BoundingBox Bounds(this IElement2D element2D)
        {
            List<ICurve> elementCurves = element2D.ElementCurves(true);

            if (elementCurves.Count == 0)
                return null;

            BoundingBox box = Engine.Geometry.Query.IBounds(elementCurves[0]);
            for (int i = 1; i < elementCurves.Count; i++)
                box += Engine.Geometry.Query.IBounds(elementCurves[i]);

            return box;
        }


        /******************************************/
        /****            IElement              ****/
        /******************************************/

        [Description("Queries the IElements BoundingBox. Acts on the elements geometrical definition of the IElement through the Geometry_Engine.")]
        [Input("elements", "The IElements with the geometry to get the BoundingBox of. i.e. added properties implying extents will not be considered.")]
        [Output("bounds", "A BoundingBox encapsulating the geometrical definition of the IElements.")]
        public static BoundingBox Bounds(this IEnumerable<IElement> elements)
        {
            if (elements.Count() == 0)
                return null;

            BoundingBox box = elements.First().IBounds();
            foreach (IElement element in elements.Skip(1))
            {
                box += element.IBounds();
            }

            return box;
        }


        /******************************************/
        /****        Interface methods         ****/
        /******************************************/

        [Description("Queries the IElements BoundingBox. Acts on the elements geometrical definition of the IElement through the Geometry_Engine.")]
        [Input("element", "The IElement with the geometry to get the BoundingBox of. i.e. added properties implying extents will not be considered.")]
        [Output("bounds", "A BoundingBox encapsulating the geometrical definition of the IElements.")]
        public static BoundingBox IBounds(this IElement element)
        {
            return Bounds(element as dynamic);
        }


        /******************************************/
        /****             Profiles             ****/
        /******************************************/

        [Description("Queries the BoundingBox of a Profile. Acts on the profile edges through the Geometry_Engine.")]
        [Input("profile", "The profile with the geometry to get the BoundingBox of.")]
        [Output("bounds", "A BoundingBox encapsulating the geometrical definition of the profile.")]
        public static BoundingBox Bounds(this IProfile profile)
        {
            if (profile?.Edges == null || !profile.Edges.Any())
                return null;

            BoundingBox bounds = new BoundingBox();
            foreach (BH.oM.Geometry.ICurve edge in profile.Edges)
            {
                bounds += edge.IBounds();
            }

            return bounds;
        }

        /******************************************/
    }
}






