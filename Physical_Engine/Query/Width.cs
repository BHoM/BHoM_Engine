/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the width of a generic opening.")]
        [Input("opening", "A generic Opening object.")]
        [Output("width", "The total width of the generic opening.")]
        public static double IWidth(this IOpening opening)
        {
            return Width(opening as dynamic);
        }

        /***************************************************/

        [Description("Returns the width of a door object.")]
        [Input("door", "A door object.")]
        [Output("width", "The total width of the door object.")]
        public static double Width(this Door door)
        {
            if(door == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the width of a null door.");
                return 0;
            }

            return BH.Engine.Geometry.Query.Width((door.Location.IExternalEdges().FirstOrDefault()));
        }

        /***************************************************/
        /****             Fallback Methods              ****/
        /***************************************************/
        
        private static double Width(this object opening)
        {
            BH.Engine.Reflection.Compute.RecordError(String.Format("Width query for {0} type is not implemented.",opening.GetType()));
            return 0;
        }

        /***************************************************/
    }
}

