/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static BoundingBox Bounds(this IElement0D element0D)
        {
            return Geometry.Query.Bounds(element0D.IGeometry());
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static BoundingBox Bounds(this IElement1D element1D)
        {
            return Geometry.Query.IBounds(element1D.IGeometry());
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static BoundingBox Bounds(this IElement2D element2D)
        {
            List<ICurve> elementCurves = element2D.ElementCurves(true);

            if (elementCurves.Count == 0)
                return null;

            BoundingBox box = Geometry.Query.IBounds(elementCurves[0]);
            for (int i = 1; i < elementCurves.Count; i++)
                box += Geometry.Query.IBounds(elementCurves[i]);

            return box;
        }


        /******************************************/
        /****            IElement              ****/
        /******************************************/

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

        public static BoundingBox IBounds(this IElement element)
        {
            return Bounds(element as dynamic);
        }


        /******************************************/
    }
}

