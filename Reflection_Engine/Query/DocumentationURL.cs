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
using System.Text;

using BH.oM.Base.Attributes;
using BH.oM.Base.Attributes.Enums;
using System.Reflection;
using System.Linq;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        [Description("Query the documentation URLs available for a given C# member.")]
        [Input("member", "The C# member to query the documentation URLs for.")]
        [Output("urls", "The available documentation URLs for the given C# member. The list will be empty if no URLs are available.")]
        public static List<DocumentationURLAttribute> DocumentationURL(this MemberInfo member)
        {
            if(member == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the documentation URL of a null member info object.");
                return new List<DocumentationURLAttribute>();
            }

            return member.GetCustomAttributes<DocumentationURLAttribute>().ToList();
        }
    }
}
