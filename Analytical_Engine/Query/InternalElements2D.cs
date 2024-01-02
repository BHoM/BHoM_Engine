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

using System.Linq;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets inner IElement2Ds from a IPanel. Method required for all IElement2Ds. \n" +
         "For a IPanel this method will return a list of all of its openings.")]
        [Input("panel", "The IPanel to get internal IElement2Ds from.")]
        [Output("elements", "The list of the internal IELement2Ds of the IPanel, i.e. a list of the Openings of the IPanel.")]
        public static List<IElement2D> InternalElements2D<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return panel.Openings.Cast<IElement2D>().ToList();
        }

        /***************************************************/
    }
}





