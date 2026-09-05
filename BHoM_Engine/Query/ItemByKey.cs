using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        public static object ItemByKey(string key)
        {
            Objects.IAssemblyResolver resolver = Global.AssemblyResolver;

            if (key.Contains('('))
            {
                //TODO need to support ctors etc. - if oM in name then ctor!

                string[] split = key.Split('(');
                if (split.Length != 2)
                    return null;

                string typeName = split[0].Substring(0, split[0].LastIndexOf('.'));
                resolver.MakeSureAssemblyIsLoadedForType(typeName);

                string[] parameterTypeNames = split[1].Substring(0, split[1].Length - 1).Split(',');
                foreach (string parameterTypeName in parameterTypeNames)
                {
                    resolver.MakeSureAssemblyIsLoadedForType(typeName);
                }

                System.Type type = Create.EngineType(typeName);
                return type.GetMethods().FirstOrDefault(x => x.ToText(includePath: true) == key);
            }
            else
            {
                //TODO: need to start supporting enums etc.

                resolver.MakeSureAssemblyIsLoadedForType(key);
                return Create.Type(key);
            }
        }
    }
}
