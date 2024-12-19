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

using System;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.Engine.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns the horizontal orthogonal width of a generic opening based on global coordinates of its BoundingBox.")]
        [Input("opening", "A generic Opening object to query its width.")]
        [Output("width", "The total width of the generic opening.",typeof(Length))]
        public static double IWidth(this IOpening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null opening.");
                return 0;
            }
            
            return Width(opening as dynamic);
        }

        /***************************************************/

        [Description("Returns the horizontal orthogonal width of a door object based on global coordinates of its BoundingBox.")]
        [Input("door", "A door object to query its width.")]
        [Output("width", "The total width of the door object.")]
        public static double Width(this Door door)
        {
            if(door == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null door.");
                return 0;
            }

            return BH.Engine.Geometry.Query.HorizontalHypotenuseLength(door.Location.IBounds());
        }

        /***************************************************/
        /****             Fallback Methods              ****/
        /***************************************************/
        
        private static double Width(this object opening)
        {
            BH.Engine.Base.Compute.RecordError(String.Format("Width query for {0} type is not implemented.",opening.GetType()));
            return 0;
        }

        /***************************************************/
    }
}





