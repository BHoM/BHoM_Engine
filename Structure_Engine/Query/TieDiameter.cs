using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetTieDiameter(this ConcreteSection property)
        {
            foreach (Reinforcement reo in property.Reinforcement)
            {
                if (reo is TieReinforcement)
                {
                    return reo.Diameter;
                }
            }
            return 0;
        }

    }
}
