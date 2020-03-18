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
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IElement1D element1D, bool recursive = true)
        {
            return new List<ICurve> { element1D.IGeometry() };
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IElement2D element2D, bool recursive)
        {
            List<ICurve> result = new List<ICurve>();

            PolyCurve outline = element2D.OutlineCurve();
            foreach (ICurve curve in outline.Curves)
            {
                if (recursive)
                    result.AddRange(curve.ISubParts());
                else
                    result.Add(curve);
            }

            foreach (IElement2D e in element2D.IInternalElements2D())
            {
                result.AddRange(e.ElementCurves(recursive));
            }

            return result;
        }


        /******************************************/
        /****            IElement              ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IEnumerable<IElement> elements, bool recursive = true)
        {
            List<ICurve> result = new List<ICurve>();
            foreach (IElement element in elements)
            {
                result.AddRange(element.IElementCurves(recursive));
            }
            return result;
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        public static List<ICurve> IElementCurves(this IElement element, bool recursive = true)
        {
            return ElementCurves(element as dynamic, recursive);
        }
        
        /******************************************/
    }
}

