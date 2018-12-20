/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Sets
{
    public static class Stitch
    {
        public static T A2B<T>(T a, T b)
        {
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dynamic ap = info.GetValue(a);
                dynamic bp = info.GetValue(b);

                if (ap is IEnumerable)
                    info.SetValue(a, As2Bs(ap, bp));
                else if (Engine.Sets.Compare.Value(ap, bp))
                    info.SetValue(a, bp);
            }

            return a;
        }

        /***************************************************/

        public static IEnumerable<T> As2Bs<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (dynamic ae in a)
            {
                foreach (dynamic be in b)
                {
                    if (Engine.Sets.Compare.Value(ae, be))
                    {
                        foreach (PropertyInfo info in typeof(T).GetProperties())
                            info.SetValue(ae, info.GetValue(be));
                        break;
                    }
                }
            }

            return a;
        }

    }
}
