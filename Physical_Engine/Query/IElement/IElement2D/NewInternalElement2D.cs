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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a valid IElement2D which can be assigned as an internal element to the ISurface")]
        [Input("surface", "The 2-dimensional element which a valid corresponding internal element is to be gotten from")]
        [Output("element2D", "a Void which can be assigned as an internal element to a ISurface")]
        public static IElement2D NewInternalElement2D(this oM.Physical.Elements.ISurface surface)
        {
            Engine.Base.Compute.RecordNote("The ISurface's IOpening may have been modified and replaced, if so the new IOpening has been set as a Void");
            return new Void();
        }

        /***************************************************/
    }
}






