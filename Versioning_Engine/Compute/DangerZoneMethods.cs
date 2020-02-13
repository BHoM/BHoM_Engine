using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Versioning
{
    public static partial class Compute
    {

        /***************************************************/

        public static List<string> DangerZoneMethods(List<MethodInfo> allBHoMMethods, string culprit, string fix)
        {
            List<string> methodStrings = allBHoMMethods.Select(
                x => Engine.Reflection.Convert.ToText(x, true, "(", ",", ")", false, false, 10000, 10000, true)
                ).Where(x => x.Contains(culprit)).ToList();

            List<string> methodNames = methodStrings.Select(x => string.Join("", x.TakeWhile(y => y != '(').Reverse().TakeWhile(y => y != '.').Reverse())).ToList();

            List<string> fixedMethodStrings = methodStrings.Select(x => x.Replace(culprit, fix)).ToList();

            List<string> preMethodNames = fixedMethodStrings.Select(y => string.Join("", y.TakeWhile(x => x != '(').Reverse().SkipWhile(x => x != '.').Skip(1).Reverse())).ToList();

            List<string> results = new List<string>();
            for (int i = 0; i < methodStrings.Count; i++)
            {
                string upintillMethodName = preMethodNames[i];

                string fixedString = fixedMethodStrings[i];
                string[] inputStrings = string.Join("", fixedString.SkipWhile(x => x != '(').Skip(1).Reverse().Skip(1).Reverse()).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                inputStrings = inputStrings.Select(x => "typeof(" + x + ")").ToArray();
                string expression = "typeof(" + upintillMethodName + ").GetMethod(" + '"' + methodNames[i] + '"';

                if (inputStrings.Length > 0)
                {
                    expression += ", new Type[] {" + string.Join(", ", inputStrings) + "}";
                }
                string result = "{/n" + '"' + methodStrings[i] + '"' + ",/n" + expression + ")/n}";
                results.Add(result);
            }

            return results.Select(x => x.Replace("/n", Environment.NewLine)).ToList();
        }

        /***************************************************/

    }
}