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
        [Description("Returns a deep clone of a given BHoM Object and attaches to it the given Fragment.")]
        [Input("iBHoMObject", "Any object implementing the IBHoMObject interface that can have fragment properties appended to it.")]
        [Input("fragment", "Any fragment object implementing the IFragment interface to append to the object.")]
        [Input("replace", "If set to true and the object already contains a fragment of the type being added, the fragment will be replaced by this instance.")]
        [Output("iBHoMObject", "The BHoM object with the added fragment.")]
        public static IBHoMObject AddFragment(this IBHoMObject iBHoMObject, IFragment fragment, bool replace = false)
        {
            if (iBHoMObject == null || fragment == null)
                return null;
            IBHoMObject o = iBHoMObject.DeepClone();

            // Give a warning if the fragment is supposed to be unique but is found on the object
            List<Type> currentFragmentTypes = iBHoMObject.Fragments.Select(x => x.GetType()).ToList();
            foreach (Type restriction in fragment.GetType().UniquenessRestrictions())
            {
                if (currentFragmentTypes.Any(x => restriction.IsAssignableFrom(x) && x != fragment.GetType()))
                    Engine.Base.Compute.RecordWarning("There is already a fragment of type " + restriction + " on this object. \nThe Fragment will still be added but consider reviewing this task as fragments of that type are supposed to be unique.");
            }

            // Make sure this fragment can be added to that object
            if (fragment.CanTarget(iBHoMObject))
            {
                if (!replace)
                    o.Fragments.Add(fragment);
                else
                    o.Fragments.AddOrReplace(fragment);
            }
            else
                Engine.Base.Compute.RecordError("An object of type " + iBHoMObject.GetType() + " is not a valid target for a fragment of type " + fragment.GetType() + ". The fragment was not added.");
            

            return o;
        }
    }
}




