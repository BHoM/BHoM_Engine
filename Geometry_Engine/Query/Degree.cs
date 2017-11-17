using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static int GetDegree(this NurbSurface surf, string UorV)
        {
            if (UorV == "u" || UorV == "U")
            {
                int uDegree = 1;
                List<double> uKnots = surf.UKnots;
                for (int i = 1; i < uKnots.Count; i++)
                {
                    if (uKnots[i - 1] == uKnots[i])
                    {
                        uDegree++;
                    }
                    else
                    {
                        break;
                    }
                }
                return uDegree;
            }
            else if (UorV == "v" || UorV == "V")
            {
                int vDegree = 1;
                List<double> vKnots = surf.VKnots;
                for (int i = 1; i < vKnots.Count; i++)
                {
                    if (vKnots[i - 1] == vKnots[i])
                    {
                        vDegree++;
                    }
                    else
                    {
                        break;
                    }
                }
                return vDegree;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
