using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Engine_Test
{
    public static class Diffing_Engine_Test
    {
        public static void TestDiffing_Main()
        {
            Bar bar = new Bar();

            bar.GetAllPropertiesObjects();



        }

        public static void GetAllPropertiesObjects(this object obj, List<object> allPropertiesObjs = null, List<Type> exclusionTypes = null)
        {

            if (allPropertiesObjs == null)
                allPropertiesObjs = new List<object>();

            Type type = obj.GetType();
            //var intfs = type.GetInterfaces().ToList();
            //intfs.ForEach(i => Console.WriteLine(i));

            var allProps = type.GetProperties().ToList();
            //pi.ForEach(i => Console.WriteLine(i));

            List<PropertyInfo> exclusionsProps = new List<PropertyInfo>();

            if (exclusionTypes == null)
            {
                exclusionTypes = new List<Type>();

                // Add types you want their properties to be excluded from the list of the fingerprint
                exclusionTypes.Add(typeof(BH.oM.Base.BHoMObject));

                var defaultExclusionProps = exclusionTypes.SelectMany(t => t.GetProperties().ToList()).ToList();

                exclusionsProps.AddRange(defaultExclusionProps);
            }

            allProps.RemoveAll(pr => exclusionsProps.Exists(t => t.DeclaringType == pr.DeclaringType && t.Name == pr.Name));


            foreach (var property in allProps)
            {
                object propertyObject = property.GetValue(obj);

                //Func<object, object> getProp = (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), property.GetGetMethod());

                //object propertyObject = getProp(obj);
                if (propertyObject != null)
                    allPropertiesObjs.Add(propertyObject);

            }

            //// Test with protobuf

            ProtoBuf.Meta.RuntimeTypeModel model = ProtoBuf.Meta.TypeModel.Create();

            AddPropsToModel(model, obj.GetType(), exclusionsProps);

            //add all properties you want to serialize. 
            //in this case we just loop over all the public properties of the class
            //Order by name so the properties are in a predictable order
            //var props = obj.GetType().GetProperties().ToList();
            //props.RemoveAll(pr => exclusionsProps.Exists(t => t.DeclaringType == pr.DeclaringType && t.Name == pr.Name));
            //var propsNames = props.Select(p => p.Name).OrderBy(name => name).ToList();

            //model.Add(obj.GetType(), true).Add(propsNames.ToArray());

            //props = typeof(BH.oM.Structure.Elements.Node).GetProperties().ToList();
            //props.RemoveAll(pr => exclusionsProps.Exists(t => t.DeclaringType == pr.DeclaringType && t.Name == pr.Name));
            //propsNames = props.Select(p => p.Name).OrderBy(name => name).ToList();

            //model.Add(typeof(BH.oM.Structure.Elements.Node), true).Add(propsNames.ToArray());


            byte[] bytes;

            using (var memoryStream = new MemoryStream())
            {
                model.Serialize(memoryStream, obj);
                bytes = memoryStream.GetBuffer();
            }

        }

        public static void AddPropsToModel(ProtoBuf.Meta.RuntimeTypeModel model, Type objType, List<PropertyInfo> exclusionsProps = null, PropertyInfo prInfo = null)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            if (objType != null)
                props = objType.GetProperties().ToList();
            else
                props = prInfo.PropertyType.GetProperties().ToList();

            if (exclusionsProps != null)
                props.RemoveAll(pr => exclusionsProps.Exists(t => t.DeclaringType == pr.DeclaringType && t.Name == pr.Name));

            props
                .Where(prop => prop.PropertyType.IsClass || prop.PropertyType.IsInterface).ToList()
                .ForEach(prop =>
                    {
                        AddPropsToModel(model, prop.PropertyType, exclusionsProps, prop); //recursive call
                    }
               );

            var propsNames = props.Select(p => p.Name).OrderBy(name => name).ToList();

            model.Add(objType, true).Add(propsNames.ToArray());

        }

    }
}
