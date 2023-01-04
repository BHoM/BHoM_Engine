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

using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adds representation fragments to IBHoMObjects.")]
        [Input("component", "The configuration properties for the representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        public static void IRepresentationFragment(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {
            RepresentationFragment(component as dynamic, dataset, viewConfig);
        }

        /***************************************************/
        /****           Fallback Methods                ****/
        /***************************************************/
        private static void RepresentationFragment(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {

        }
    }
}


