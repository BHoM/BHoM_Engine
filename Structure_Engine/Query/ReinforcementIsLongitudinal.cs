using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsLongitudinal(this LayerReinforcement reinforcement)
        {
            return true;
        }

        /***************************************************/

        public static bool IsLongitudinal(this PerimeterReinforcement reinforcement)
        {
            return true;
        }

        /***************************************************/

        public static bool IsLongitudinal(this TieReinforcement reinforcement)
        {
            return false;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsLongitudinal(this Reinforcement reinforcement)
        {
            return IsLongitudinal(reinforcement as dynamic);
        }
    }
}
