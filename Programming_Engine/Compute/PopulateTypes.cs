using BH.oM.Programming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Programming
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public static INode IPopulateTypes(this INode node)
        {
            return PopulateTypes(node as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ClusterContent PopulateTypes(this ClusterContent content)
        {
            if (content == null)
                return content;

            content.InternalNodes = content.InternalNodes.Select(x => x.IPopulateTypes()).ToList();

            // Collect the types
            Dictionary<Guid, Type> nodeTypes = content.DataTypePerParam();

            // Assign the types to the inputs
            foreach (DataParam input in content.Inputs)
            {
                if (input.TargetIds.Count > 0 && nodeTypes.ContainsKey(input.TargetIds[0]))
                    input.DataType = nodeTypes[input.TargetIds[0]];
            }

            // Assign the types to the outputs
            foreach (ReceiverParam output in content.Outputs)
            {
                if (nodeTypes.ContainsKey(output.SourceId))
                    output.DataType = nodeTypes[output.SourceId];
            }

            return content;
        }

        /***************************************************/

        public static MethodNode PopulateTypes(this MethodNode node)
        {
            if (node == null || node.Method == null)
                return node;

            ParameterInfo[] methodParams = node.Method.GetParameters();
            if (node.Inputs.Count != methodParams.Length)
                return node;

            for (int i = 0; i < methodParams.Length; i++)
            {
                node.Inputs[i].DataType = methodParams[i].ParameterType;
                if (methodParams[i].HasDefaultValue)
                    node.Inputs[i].DefaultValue = methodParams[i].DefaultValue;
            }
                

            if (node.Outputs.Count == 1)
                node.Outputs[0].DataType = node.Method.ReturnType;
            else if (node.Outputs.Count > 1 && node.Method.ReturnType is oM.Reflection.Interface.IOutput)
            {
                Type[] types = node.Method.ReturnType.GetGenericArguments();
                if (types.Length == node.Outputs.Count)
                {
                    for (int i = 0; i < types.Length; i++)
                        node.Outputs[i].DataType = types[i];
                }
            }

            return node;
        }

        /***************************************************/

        public static ConstructorNode PopulateTypes(this ConstructorNode node)
        {
            if (node == null || node.Constructor == null)
                return node;

            ParameterInfo[] methodParams = node.Constructor.GetParameters();
            if (node.Inputs.Count != methodParams.Length)
                return node;

            for (int i = 0; i < methodParams.Length; i++)
            {
                node.Inputs[i].DataType = methodParams[i].ParameterType;
                if (methodParams[i].HasDefaultValue)
                    node.Inputs[i].DefaultValue = methodParams[i].DefaultValue;
            }


            if (node.Outputs.Count > 0)
                node.Outputs[0].DataType = node.Constructor.DeclaringType;

            return node;
        }

        /***************************************************/

        public static InitialiserNode PopulateTypes(this InitialiserNode node)
        {
            if (node == null || node.ObjectType == null)
                return node;

            PropertyInfo[] properties = node.ObjectType.GetProperties();
            object instance = Activator.CreateInstance(node.ObjectType);

            foreach (ReceiverParam input in node.Inputs)
            {
                PropertyInfo match = properties.FirstOrDefault(x => x.Name == input.Name);
                if (match != null)
                {
                    input.DataType = match.PropertyType;
                    input.DefaultValue = match.GetValue(instance);
                }
            }

            if (node.Outputs.Count > 0)
                node.Outputs[0].DataType = node.ObjectType;

            return node;
        }

        /***************************************************/

        public static TypeNode PopulateTypes(this TypeNode node)
        {
            if (node == null || node.Type == null)
                return node;

            foreach (DataParam param in node.Outputs)
                param.DataType = typeof(Type);

            return node;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static INode PopulateTypes(this INode node)
        {
            return node;
        }

        /***************************************************/
    }
}
