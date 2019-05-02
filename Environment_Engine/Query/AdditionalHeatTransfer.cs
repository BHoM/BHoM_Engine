/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.MaterialFragments;
using BH.oM.Physical.Constructions;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the total additional heat transfer of a construction")]
        [Input("construction", "A Construction Object")]
        [Output("additionalHeatTransfer", "The additional heat transfer of the construction as a sum of all materials additional heat transfer")]
        public static double AdditionalHeatTransfer(this Construction construction)
        {
            double aht = 0;

            foreach (Layer l in construction.Layers)
            {
                IEnvironmentMaterial envMat = l.Material.Properties.Where(x => x is IEnvironmentMaterial).FirstOrDefault() as IEnvironmentMaterial;
                if (envMat != null)
                    aht += envMat.AdditionalHeatTransfer;
            }

            return aht;
        }
    }
}