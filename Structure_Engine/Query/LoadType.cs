using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadType LoadType(this AreaTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaTemperature;
        }

        /***************************************************/

        public static LoadType LoadType(this AreaUniformalyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaUniformLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarPointLoad load)
        {
            return oM.Structure.Loads.LoadType.BarPointLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarPrestressLoad load)
        {
            return oM.Structure.Loads.LoadType.Pressure;
        }

        /***************************************************/

        public static LoadType LoadType(this BarTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.BarTemperature;
        }

        /***************************************************/

        public static LoadType LoadType(this BarUniformlyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarUniformLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarVaryingDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarVaryingLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this GravityLoad load)
        {
            return oM.Structure.Loads.LoadType.Selfweight;
        }

        /***************************************************/

        public static LoadType LoadType(this PointAcceleration load)
        {
            return oM.Structure.Loads.LoadType.PointAcceleration;
        }

        /***************************************************/

        public static LoadType LoadType(this PointDisplacement load)
        {
            return oM.Structure.Loads.LoadType.PointDisplacement;
        }

        /***************************************************/

        public static LoadType LoadType(this PointForce load)
        {
            return oM.Structure.Loads.LoadType.PointForce;
        }

        /***************************************************/

        public static LoadType LoadType(this PointVelocity load)
        {
            return oM.Structure.Loads.LoadType.PointVelocity;
        }

        /***************************************************/
    }

}
