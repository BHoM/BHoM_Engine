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
            return SHA256Hash(obj.ToDiffingByteArray(except));
        }

        ///***************************************************/


        ///***************************************************/
        ///**** Private Methods                           ****/
        ///***************************************************/

        private static string SHA256Hash(byte[] inputObj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in SHA256_byte(inputObj))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static byte[] SHA256_byte(byte[] inputObj)
        {
            HashAlgorithm algorithm = System.Security.Cryptography.SHA256.Create();
            return algorithm.ComputeHash(inputObj);
        }


    }
}
