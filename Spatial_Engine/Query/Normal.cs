/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/
        
        [Description("Returns the normal to the IElement2D which is perpendicular to its plane and oriented according to the right hand rule in relation to the outline curve.")]
        [Input("element2D", "The IElement2D to get the normal from.")]
        [Output("n", "The normal of the IElement2D based on the external outline curve.")]
        public static Vector Normal(this IElement2D element2D)
        {
            return element2D.OutlineCurve().Normal();
        }

        /******************************************/
    }
}



