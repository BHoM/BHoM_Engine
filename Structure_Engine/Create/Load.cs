using BH.oM.Geometry;
using BH.oM.Structural.Loads;

namespace BH.Engine.Structure
{
    public static partial class Create  //TODO: Review those constructors since most of them where missing (AD added default ones)
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static AreaTemperatureLoad AreaTemperatureLoad(Loadcase loadcase, double t)
        {
            return new AreaTemperatureLoad { Loadcase = loadcase, TemperatureChange = t };
        }

        /***************************************************/

        public static AreaUniformalyDistributedLoad AreaUniformalyDistributedLoad(Loadcase loadcase, Vector pressure)
        {
            return new AreaUniformalyDistributedLoad { Loadcase = loadcase, Pressure = pressure };
        }

        /***************************************************/

        public static BarPointLoad BarPointLoad(Loadcase loadcase, double distFromA, Vector force, Vector moment)
        {
            return new BarPointLoad { Loadcase = loadcase, DistanceFromA = distFromA, Force = force, Moment = moment };
        }

        /***************************************************/

        public static BarPrestressLoad BarPrestressLoad(Loadcase loadcase, double prestress)
        {
            return new BarPrestressLoad { Loadcase = loadcase, Prestress = prestress };
        }

        /***************************************************/

        public static BarTemperatureLoad BarTemperatureLoad(Loadcase loadcase, Vector temperatureChange)
        {
            return new BarTemperatureLoad { Loadcase = loadcase, TemperatureChange = temperatureChange };
        }

        /***************************************************/

        public static BarUniformlyDistributedLoad BarUniformlyDistributedLoad(Loadcase loadcase, Vector force, Vector moment)
        {
            return new BarUniformlyDistributedLoad { Loadcase = loadcase, Force = force, Moment = moment };
        }

        /***************************************************/

        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, double distFromA, Vector forceA, Vector momentA, double distFromB, Vector forceB, Vector momentB)
        {
            return new BarVaryingDistributedLoad
            {
                Loadcase = loadcase,
                DistanceFromA = distFromA,
                ForceA = forceA,
                MomentA = momentA,
                DistanceFromB = distFromB,
                ForceB = forceB,
                MomentB = momentB
            };
        }

        /***************************************************/

        public static GeometricalAreaLoad GeometricalAreaLoad(ICurve contour, Vector force)
        {
            return new GeometricalAreaLoad { Contour = contour, Force = force };
        }

        /***************************************************/

        public static GeometricalLineLoad GeometricalLineLoad(BH.oM.Geometry.Line line, Vector force, Vector moment = null)
        {
            return new GeometricalLineLoad
            {
                Location = line,
                ForceA = force,
                ForceB = force,
                MomentA = moment == null ? new Vector { X = 0, Y = 0, Z = 0 } : moment,
                MomentB = moment == null ? new Vector { X = 0, Y = 0, Z = 0 } : moment
            };
            
        }

        /***************************************************/
        public static GeometricalLineLoad GeometricalLineLoad(BH.oM.Geometry.Line line, Vector forceA, Vector forceB, Vector momentA = null, Vector momentB = null)
        {
            return new GeometricalLineLoad
            {
                Location = line,
                ForceA = forceA,
                ForceB = forceB,
                MomentA = momentA == null ? new Vector { X = 0, Y = 0, Z = 0 } : momentA,
                MomentB = momentB == null ? new Vector { X = 0, Y = 0, Z = 0 } : momentB
            };
        }

        /***************************************************/

        public static GravityLoad GravityLoad(Loadcase loadcase, Vector direction)
        {
            return new GravityLoad { GravityDirection = direction };
        }

        /***************************************************/

        public static PointAcceleration PointAcceleration(Loadcase loadcase, Vector translationAcc, Vector rotationAcc)
        {
            return new PointAcceleration { Loadcase = loadcase, TranslationalAcceleration = translationAcc, RotationalAcceleration = rotationAcc };
        }

        /***************************************************/

        public static PointDisplacement PointDisplacement(Loadcase loadcase, Vector translation, Vector rotation)
        {
            return new PointDisplacement { Loadcase = loadcase, Translation = translation, Rotation = rotation };
        }

        /***************************************************/

        public static PointForce PointForce(Loadcase loadcase, Vector force, Vector moment)
        {
            return new PointForce { Loadcase = loadcase, Force = force, Moment = moment };
        }

        /***************************************************/

        public static PointVelocity PointVelocity(Loadcase loadcase, Vector translation, Vector rotation)
        {
            return new PointVelocity { Loadcase = loadcase, TranslationalVelocity = translation, RotationalVelocity = rotation };
        }

        /***************************************************/
    }
}
