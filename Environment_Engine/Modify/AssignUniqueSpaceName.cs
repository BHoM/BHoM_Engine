using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        public static List<Space> AssignUniqueSpaceName(List<Space> spaces)
        {
            
            List<Space> spacesWithNames = new List<Space>();
            foreach (Space s in spaces)
            {
                string name = s.Name + "_" + s.SpaceType.ToString();
                if (spaces.Where(x => x.SpaceType == s.SpaceType).Count() > 1)
                {
                    int current = spacesWithNames.Where(x => x.Name.StartsWith(name)).Count();
                    name += (current + 1).ToString();
                }

                s.Name = name;
                spacesWithNames.Add(s);
            }

            return spacesWithNames;
        }
    }
}
