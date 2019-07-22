using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using BH.Engine.Serialiser;

namespace Diffing_Engine
{
    public static partial class TestDiffing
    {
        public static void TestHashing_Protobuf(List<object> objs)
        {


            // Add types whose properties you want to exclude from the fingerprint
            List<Type> exclusionTypes = new List<Type> { typeof(BH.oM.Base.BHoMObject) };

            foreach (var obj in objs)
            {
                byte[] objByte = obj.ToByteArray(exclusionTypes);

                using (MD5 md5Hash = MD5.Create())
                {
                    string hash = GetMd5Hash(md5Hash, objByte);
                    Console.WriteLine(obj.GetType().Name + ": " + hash);
                }
            }

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

    public class TestclassA
    {
        public int X;
        public int Y;
    }

    public class TestclassB
    {
        public int X;
        public int Y;
    }
}
