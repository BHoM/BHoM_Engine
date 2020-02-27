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

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IElement1D element1D, bool recursive = true)
        {
            return Spatial.Query.ElementCurves(element1D, recursive);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<ICurve> ElementCurves(this IElement2D element2D, bool recursive)
        {
            return Spatial.Query.ElementCurves(element2D, recursive);
        }


        /******************************************/
        /**** Public Methods - Interfaces      ****/
        /******************************************/

        public static List<ICurve> IElementCurves(this IElement element, bool recursive = true)
        {
            return Spatial.Query.IElementCurves(element, recursive);
        }


        /******************************************/

        public static List<ICurve> IElementCurves(this IEnumerable<IElement> elements, bool recursive = true)
        {
            return Spatial.Query.IElementCurves(elements, recursive);

        }


        /******************************************/
        /****        Deprecated methods        ****/
        /******************************************/

        [DeprecatedAttribute("2.3", "Input type changed from BHoMObject to IElement", null, "IElementCurves")]
        public static List<ICurve> IElementCurves(this BHoMObject element, bool recursive = true)
        {
            return IElementCurves(element as IElement, recursive);
        }


        /******************************************/

        [DeprecatedAttribute("2.3", "Input type changed from List<BHoMObject> to IEnumerable<IElement>", null, "IElementCurves")]
        public static List<ICurve> IElementCurves(this List<BHoMObject> elements, bool recursive = true)
        {
            return IElementCurves(elements.Cast<IElement>(), recursive);
        }

        /******************************************/
    }
}

