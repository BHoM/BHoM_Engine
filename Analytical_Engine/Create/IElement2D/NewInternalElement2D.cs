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

using System;
using BH.oM.Analytical.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;


namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a new Element2D, appropriate to the input type. For this case the appropriate type for the Panel will be a new Opening, in the position provided. \n" +
                     "Method required for any IElement2D that contians internal IElement2Ds.")]
        [Input("panel", "Panel just used to determine the appropriate type of IElement2D to create.")]
        [Output("opening", "The created Opening as a IElement2D.")]
        public static IElement2D NewInternalElement2D<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return Activator.CreateInstance<TOpening>();
        }

        /***************************************************/
    }
}




