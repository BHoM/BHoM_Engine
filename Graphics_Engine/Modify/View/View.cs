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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a dataset by adding representation fragments to define a view of the data.")]
        [Input("view", "The configuration properties for the view representation.")]
        [Input("dataset", "Dataset to generate a view of.")]
        public static void IView(this IView view, Dataset dataset)
        {
            if(view == null || dataset == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot modify a view if the view or dataset are null.");
                return;
            }

            View(view as dynamic, dataset);
        }
        
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        public static void View(this IView view, Dataset dataset)
        {

        }
    }
}






