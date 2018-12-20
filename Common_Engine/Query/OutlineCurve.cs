/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Common;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static PolyCurve IOutlineCurve(this IElement2D element2D)
        {
            return new PolyCurve { Curves = element2D.IOutlineElements1D().Select(e => e.IGeometry()).ToList() };
        }

        /******************************************/

        public static PolyCurve IOutlineCurve(this List<IElement1D> elements1D)
        {
            return new PolyCurve { Curves = elements1D.Select(e => e.IGeometry()).ToList() };
        }

        /******************************************/

        public static List<PolyCurve> IInternalOutlineCurves(this IElement2D element2D)
        {
            return element2D.IInternalElements2D().Select(x => x.IOutlineCurve()).ToList();
        }

        /******************************************/
    }
}
