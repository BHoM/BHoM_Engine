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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        [Description("Returns a deep clone of a given BHoM Object with the Fragment of the input fragmentType removed.")]
        [Input("iBHoMObject", "Any object implementing the IBHoMObject interface that can have fragment properties appended to it.")]
        [Input("fragmentType", "The type of fragment that should be removed from the object.")]
        [Output("iBHoMObject", "The BHoM object with the added fragment.")]
        public static IBHoMObject RemoveFragment(this IBHoMObject iBHoMObject, Type fragmentType = null)
       {
            if (fragmentType == null) return iBHoMObject;
            if (iBHoMObject == null) return null;
            IBHoMObject o = iBHoMObject.DeepClone();

            if (!typeof(IFragment).IsAssignableFrom(fragmentType))
            {
                Base.Compute.RecordError("Provided input in fragmentType is not a Fragment type (does not implement IFragment interface).");
                return iBHoMObject;
            }

            if (!iBHoMObject.Fragments.Contains(fragmentType))
            {
                Base.Compute.RecordWarning($"{iBHoMObject.GetType().Name} does not contain any `{fragmentType.Name}` fragment.");
                return iBHoMObject;
            }

            o.Fragments.Remove(fragmentType);
           
            return o;
        }
    }
}




