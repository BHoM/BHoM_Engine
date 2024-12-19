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
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static void IOpenHelpPage(object obj)
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is Type)
            {
                OpenHelpPage(obj as Type);
            }
            else if (obj is MethodBase)
            {
                OpenHelpPage(obj as MethodBase);
            }
            else
            {
                return;
            }
        }

        /*******************************************/

        public static void OpenHelpPage(Type type)
        {
            string url = Base.Query.BHoMWebsiteURL();
            if (type != null)
            {
                url = Query.Url(type) ?? url;
            }

            System.Diagnostics.Process.Start(url);
        }

        /*******************************************/

        public static void OpenHelpPage(MethodBase method)
        {
            string url = Base.Query.BHoMWebsiteURL();
            if (method != null)
            {
                url = Query.Url(method) ?? url;
            }

            System.Diagnostics.Process.Start(url);
        }

        /*******************************************/
    }
}






