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

using BH.oM.Base;

namespace BH.Engine.Serialiser
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string TaggedName(this IBHoMObject obj)
        {
            if(obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the tagged name of a null object.");
                return "";
            }

            string str = string.IsNullOrWhiteSpace(obj.Name) ? "" : obj.Name;

            if (obj.Tags.Count > 0)
            {
                str += " __Tags__:";

                foreach (string tag in obj.Tags)
                {
                    str += tag + "_/_";
                }

                str = str.TrimEnd("_/_");
            }

            return str;

        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }

        /***************************************************/

    }
}






