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

using BH.oM.Environment;
using BH.oM.Environment.Materials;
using BH.oM.Physical.Properties.Construction;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the total Absorptance of a construction")]
        [Input("construction", "A Construction object")]
        [Output("absorptance", "The total absorptance of the construction")]
        public static Absorptance Absorptance(this Construction construction)
        {
            List<Absorptance> absorptances = new List<Absorptance>();

            foreach(Layer l in construction.Layers)
            {
                IEnvironmentMaterial envMat = l.Material.Properties.Where(x => x is IEnvironmentMaterial).FirstOrDefault() as IEnvironmentMaterial;
                if (envMat != null)
                    absorptances.Add(envMat.Absorptance);
            }

            double value = 0;
            foreach (Absorptance a in absorptances)
                value += a.Value;

            Absorptance rtn = new Absorptance();
            rtn.AbsorptanceUnit = AbsorptanceUnit.Percent;
            rtn.AbsorptanceType = AbsorptanceType.Undefined; //ToDo - fix this method to be calculated correctly...
            rtn.Value = value;

            return rtn;
        }
    }
}