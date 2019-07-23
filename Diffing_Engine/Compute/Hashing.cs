using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace Diffing_Engine
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        [Input("except", "Name of the fields to be ignored. For example, \"BHoM_Guid\".")]
        public static string SHA256Hash(IBHoMObject obj, List<string> except = null)
        {
            var json = obj.ToDiffingJson(except);

            return SHA256Hash(json);
        }

        public static string SHA256Hash(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in SHA256_byte(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        ///***************************************************/


        ///***************************************************/
        ///**** Private Methods                           ****/
        ///***************************************************/
        ///
        private static byte[] SHA256_byte(string inputString)
        {
            HashAlgorithm algorithm = System.Security.Cryptography.SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }


    }
}
