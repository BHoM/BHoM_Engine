using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Engine_Test
{
    public static class Diffing_Engine_Test
    {
        public static void TestDiffing_Main()
        {
            Bar bar = new Bar();
            Bar bar2 = new Bar();

            List<object> objs = new List<object>();
            objs.Add(bar);
            objs.Add(bar2);

            // Add types whose properties you want to exclude from the fingerprint
            List<Type> exclusionTypes = new List<Type> { typeof(BH.oM.Base.BHoMObject) };

            // TODO: what happens if you want to specify single properties to be excluded, and those are already included in the exclusionTypes?
            // Do a Set instead of a List for exclusionsProps.

            foreach (var obj in objs)
            {
                byte[] objByte = obj.ToByteArray(exclusionTypes);

                using (MD5 md5Hash = MD5.Create())
                {
                    string hash = GetMd5Hash(md5Hash, objByte);
                    Console.WriteLine(hash);
                }
            }
           
        }

        static string GetMd5Hash(MD5 md5Hash, byte[] objByte)
        {
            byte[] data = md5Hash.ComputeHash(objByte);

            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static byte[] ToByteArray(this object obj, List<Type> exclusionTypes = null, List<PropertyInfo> exclusionsProps = null)
        {
            if (exclusionsProps == null)
                exclusionsProps = new List<PropertyInfo>();

            if (exclusionTypes != null)
            {
                // Extract properties to be excluded from the exclusionTypes
                var exctractedExclusionProps = exclusionTypes.SelectMany(t => t.GetProperties().ToList()).ToList();

                exclusionsProps.AddRange(exctractedExclusionProps);
            }

            // Protobuf-net implementation
            ProtoBuf.Meta.RuntimeTypeModel model = ProtoBuf.Meta.TypeModel.Create();

            AddPropsToModel(model, obj.GetType(), exclusionsProps);

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                model.Serialize(memoryStream, obj);
                bytes = memoryStream.GetBuffer();
            }

            return bytes;
        }

        public static void AddPropsToModel(ProtoBuf.Meta.RuntimeTypeModel model, Type objType, List<PropertyInfo> exclusionsProps = null)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            if (objType != null)
                props = objType.GetProperties().ToList();
            else
                throw new ArgumentNullException("Object type must be non null for the byte serialization.");

            if (exclusionsProps != null)
                props.RemoveAll(pr => exclusionsProps.Exists(t => t.DeclaringType == pr.DeclaringType && t.Name == pr.Name));

            props
                .Where(prop => prop.PropertyType.IsClass || prop.PropertyType.IsInterface).ToList()
                .ForEach(prop =>
                    {
                        AddPropsToModel(model, prop.PropertyType, exclusionsProps); //recursive call
                    }
               );

            var propsNames = props.Select(p => p.Name).OrderBy(name => name).ToList();

            model.Add(objType, true).Add(propsNames.ToArray());
        }

    }
}
