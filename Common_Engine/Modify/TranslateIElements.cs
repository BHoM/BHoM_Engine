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

using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Common
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - IElements                ****/
        /***************************************************/

        public static IElement2D Translate(this IElement2D element2D, Vector transform) //todo: move this to analytical along with other IElement methods
        {
            List<IElement1D> newOutline = new List<IElement1D>();
            foreach (IElement1D element1D in element2D.IOutlineElements1D())
            {
                newOutline.Add(element1D.Translate(transform));
            }
            IElement2D result = element2D.ISetOutlineElements1D(newOutline);

            List<IElement2D> newInternalOutlines = new List<IElement2D>();
            foreach (IElement2D internalElement2D in result.IInternalElements2D())
            {
                newInternalOutlines.Add(internalElement2D.Translate(transform));
            }
            result = result.ISetInternalElements2D(newInternalOutlines);
            return result;
        }

        /***************************************************/

        public static IElement1D Translate(this IElement1D element1D, Vector transform) //todo: move this to analytical along with other IElement methods
        {
            return element1D.ISetGeometry(Geometry.Modify.ITranslate(element1D.IGeometry(), transform));
        }

        /******************************************/

        public static IElement0D Translate(this IElement0D element0D, Vector transform) //todo: move this to analytical along with other IElement methods
        {
            return element0D.ISetGeometry(Geometry.Modify.Translate(element0D.IGeometry(), transform));
        }
    }
}

