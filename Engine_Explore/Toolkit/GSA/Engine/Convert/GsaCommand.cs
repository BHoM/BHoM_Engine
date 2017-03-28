using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.Engine;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;


namespace Engine_Explore.Engine.Convert
{
    public static class GsaCommand
    {
        public static string Write(Node node, string index)
        {
            string constraint = Write(node.Constraint);
            return  "NODE.2, " + index + ", " + node.Name + " , NO_RGB, " + 
                    node.Point.X + " , " + node.Point.Y + " , " + node.Point.Z + 
                    ", NO_GRID, " + 0 + ", REST," + constraint + ", STIFF,0,0,0,0,0,0";
        }

        /***************************************************/

        public static string Write(NodeConstraint constraint)
        {
            int X = ((constraint.UX == DOFType.Fixed) ? 1 : 0);
            int Y = ((constraint.UY == DOFType.Fixed) ? 1 : 0);
            int Z = ((constraint.UZ == DOFType.Fixed) ? 1 : 0);
            int XX = ((constraint.RX == DOFType.Fixed) ? 1 : 0);
            int YY = ((constraint.RY == DOFType.Fixed) ? 1 : 0);
            int ZZ = ((constraint.RZ == DOFType.Fixed) ? 1 : 0);

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }
    }
}
