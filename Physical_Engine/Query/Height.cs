/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        [Description("Returns the vertical orthogonal height of a generic opening based on global coordinates of its BoundingBox.")]
        [Input("opening", "A generic Opening object to query its height.")]
        [Output("height", "The total height of the generic opening.", typeof(Length))]
        public static double IHeight(this IOpening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the height of a null opening.");
                return 0;
            }
            
            return Height(opening as dynamic);
        }

        /***************************************************/

        [Description("Returns the vertical orthogonal height of a door object based on global coordinates of its BoundingBox.")]
        [Input("door", "A door object to query its height.")]
        [Output("height", "The total height of the door object.")]
        public static double Height(this Door door)
        {
            if(door == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the height of a null door.");
                return 0;
            }

            return BH.Engine.Geometry.Query.Height(door.Location.IBounds());
        }

        /***************************************************/
        /****             Fallback Methods              ****/
        /***************************************************/
        
        private static double Height(this object opening)
        {
            BH.Engine.Base.Compute.RecordError(String.Format("Height query for {0} type is not implemented.",opening.GetType()));
            return 0;
        }

        /***************************************************/
    }
}




