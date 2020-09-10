/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Compute
    {

        ///***************************************************/
        ///**** Private Methods                           ****/
        ///***************************************************/

        [Description("Computes a SHA 256 hash from the given string.")]
        public static string SHA256Hash(string hashString)
        {
            byte[] hashBytes = System.Text.ASCIIEncoding.Default.GetBytes(hashString);

            return SHA256Hash(hashBytes);
        }

        private static string SHA256Hash(byte[] inputObj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in SHA256_byte(inputObj))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        
        ///***************************************************/

        private static byte[] SHA256_byte(byte[] inputObj)
        {
            HashAlgorithm algorithm = System.Security.Cryptography.SHA256.Create();
            return algorithm.ComputeHash(inputObj);
        }

    }
}

