using BH.oM.Base;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        public static void UniqueEntityNames(this List<IBHoMObject> entities)
        {
            
            List<string> distinctNames = entities.Select(x => x.Name).Distinct().ToList();

            foreach (string name in distinctNames)
            {
                List<IBHoMObject> matchnodes = entities.FindAll(x => x.Name == name);
                if (matchnodes.Count > 1)
                {
                    for (int i = 0; i < matchnodes.Count; i++)
                        matchnodes[i].Name += "_" + i;
                }
            }    
        }
        /***************************************************/
    }
}
