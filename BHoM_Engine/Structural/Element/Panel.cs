using BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = BHoM.Structural.Elements;
namespace BHoM.Structural.Element
{
    public static class XPanel
    {
        private static bool IsInside(Curve c, List<Curve> crvs)
        {
            for (int i = 0; i < crvs.Count; i++)
            {
                if (!crvs[i].Equals(c))
                {
                    if (crvs[i].ContainsCurve(c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static BHoM.Geometry.Group<Curve> GetContours(this BHE.Panel panel)
        {
            //
            return null;
        }
        
        public static BHoM.Geometry.Group<Curve> GetOpennings(this BHE.Panel panel)
        {
            return null;
        }
    }
}
