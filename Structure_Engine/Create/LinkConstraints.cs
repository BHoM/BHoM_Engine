using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        public static LinkConstraint CreateLinkConstraint(string name, bool[] fixity)
        {
            return new LinkConstraint
            {
                XtoX = fixity[0],
                YtoY = fixity[1],
                ZtoZ = fixity[2],
                XtoYY = fixity[3],
                XtoZZ = fixity[4],
                YtoXX = fixity[5],
                YtoZZ = fixity[6],
                ZtoXX = fixity[7],
                ZtoYY = fixity[8],
                XXtoXX = fixity[9],
                YYtoYY = fixity[10],
                ZZtoZZ = fixity[11],
                Name = name
            };
        }

        public static LinkConstraint CreateFixedConstraint()
        {
            return new LinkConstraint
            {
                XtoX   = true,
                YtoY   = true,
                ZtoZ   = true,
                XtoYY  = true,
                XtoZZ  = true,
                YtoXX  = true,
                YtoZZ  = true,
                ZtoXX  = true,
                ZtoYY  = true,
                XXtoXX = true,
                YYtoYY = true,
                ZZtoZZ = true,
                Name = "Fixed"
            };
        }

        //public static LinkConstraint CreatePinnedConstraint()
        //{
        //    bool[] fixities = new bool[12];

        //    for (int i = 0; i < 9; i++)
        //    {
        //        fixities[i] = true;
        //    }
        //    LinkConstraint constr = CreateLinkConstraint("Pinned", fixities);
        //    return constr;
        //}

        public static LinkConstraint Pinned
        {
            get
            {
                bool[] fixities = new bool[12];

                for (int i = 0; i < 9; i++)
                {
                    fixities[i] = true;
                }

                LinkConstraint constr = CreateLinkConstraint("Pinned", fixities);
                return constr;
            }
        }

    }
}
