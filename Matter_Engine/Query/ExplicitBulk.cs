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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Materials;
using BH.oM.Physical.Elements;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the material take off information stored as a fragment on the object and return it as an ExplicitBulk element. This could be data extracted from an external package such as Revit and stored in VolumetricMaterialTakeoff fragment attached to a given BHoMObject when pulled.")]
        [Input("bHoMObject", "BHoMObject to be queried for the material take off information.")]
        [Output("bulk", "ExplicitBulk element extrated based on Material take off information stored as a fragment on the input BHoMObject.")]
        public static ExplicitBulk ExplicitBulk(this IBHoMObject bHoMObject)
        {
            if (bHoMObject == null)
            {
                Base.Compute.RecordError($"Cannot extract {nameof(VolumetricMaterialTakeoff)} from a null object.");
                return null;
            }

            VolumetricMaterialTakeoff takeOff = bHoMObject.FindFragment<VolumetricMaterialTakeoff>();
            if (takeOff == null)
                return null;

            return new ExplicitBulk { MaterialComposition = Create.MaterialComposition(takeOff), Volume = takeOff.SolidVolume() };
        }

        /***************************************************/
    }
}


