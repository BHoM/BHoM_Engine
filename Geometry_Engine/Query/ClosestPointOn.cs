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

using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        public static Point ClosestPointOn(this IElement0D element0D, Point point)
        {
            return element0D.IGeometry();
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Point ClosestPointOn(this IElement1D element1D, Point point)
        {
            return IClosestPoint(element1D.IGeometry(), point);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static Point ClosestPointOn(this IElement2D element2D, Point point)
        {
            if (!element2D.IsPlanar())
            {
                Engine.Reflection.Compute.RecordError("Non-Planar element");
                return null;
            }

            Plane panelPlane = element2D.FitPlane();
            List<Point> cPt = new List<Point> { point.Project(panelPlane) };

            foreach (PolyCurve outline in element2D.IInternalOutlineCurves())
            {
                if (outline.IsContaining(cPt))
                    return outline.ClosestPointOn(cPt[0]);
            }

            PolyCurve panelOutline = element2D.IOutlineCurve();
            return (panelOutline.IsContaining(cPt)) ? cPt[0] : panelOutline.ClosestPointOn(cPt[0]);
        }


        /******************************************/
        /****        Interface methods         ****/
        /******************************************/

        public static Point IClosestPointOn(this IElement element, Point point)
        {
            return ClosestPointOn(element as dynamic, point);
        }


        /******************************************/
        /****        Deprecated methods        ****/
        /******************************************/

        [DeprecatedAttribute("2.3", "Input type changed from BHoMObject to IElement", null, "IClosestPointOn")]
        public static Point IClosestPointOn(this BHoMObject element, Point point)
        {
            return IClosestPointOn(element as IElement, point);
        }

        /******************************************/
    }
}
