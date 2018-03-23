using BH.oM.Geometry;
using BH.oM.Structural.Loads;
using BH.oM.Base;
using BH.oM.Structural.Elements;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Structure
{
    public static partial class Create  //TODO: Review those constructors since most of them where missing (AD added default ones)
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static AreaTemperatureLoad AreaTemperatureLoad(Loadcase loadcase, double t, BHoMGroup<IAreaElement> group, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return new AreaTemperatureLoad {
                Loadcase = loadcase,
                TemperatureChange = t,
                Objects = group,
                Axis = axis,
                Projected = projected,
                Name = name
            };
        }
        /***************************************************/

        public static AreaTemperatureLoad AreaTemperatureLoad(Loadcase loadcase, double t, IEnumerable<IAreaElement> objects, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return AreaTemperatureLoad(loadcase, t, new BHoMGroup<IAreaElement>() { Elements = objects.ToList() }, axis, projected, name);
        }

        /***************************************************/

        public static AreaUniformalyDistributedLoad AreaUniformalyDistributedLoad(Loadcase loadcase, Vector pressure, BHoMGroup<IAreaElement> group, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return new AreaUniformalyDistributedLoad
            {
                Loadcase = loadcase,
                Pressure = pressure,
                Objects = group,
                Axis = axis,
                Projected = projected,
                Name = name
            };
        }

        /***************************************************/

        public static AreaUniformalyDistributedLoad AreaUniformalyDistributedLoad(Loadcase loadcase, Vector pressure, IEnumerable<IAreaElement> objects, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return AreaUniformalyDistributedLoad(loadcase, pressure, new BHoMGroup<IAreaElement>() { Elements = objects.ToList() }, axis, projected, name);
        }

        /***************************************************/

        public static BarPointLoad BarPointLoad(Loadcase loadcase, BHoMGroup<Bar> group, double distFromA, Vector force = null, Vector moment = null,  LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (force == null && moment == null)
                throw new ArgumentException("Bar point load requires either the force or the moment vector to be defined");

            return new BarPointLoad {
                Loadcase = loadcase,
                DistanceFromA = distFromA,
                Force = force == null ? new Vector() : force,
                Moment = moment == null ? new Vector() : moment,
                Objects = group,
                Axis = axis,
                Name = name
            };
        }

        /***************************************************/

        public static BarPointLoad BarPointLoad(Loadcase loadcase, double distFromA, IEnumerable<Bar> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return BarPointLoad(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, distFromA, force, moment, axis, name);
        }

        /***************************************************/

        public static BarPrestressLoad BarPrestressLoad(Loadcase loadcase, double prestress, BHoMGroup<Bar> group, string name = "")
        {
            return new BarPrestressLoad
            {
                Loadcase = loadcase,
                Prestress = prestress,
                Objects = group,
                Name = name
            };
        }

        /***************************************************/

        public static BarPrestressLoad BarPrestressLoad(Loadcase loadcase, double prestress, IEnumerable<Bar> objects, string name = "")
        {
            return BarPrestressLoad(loadcase, prestress, new BHoMGroup<Bar>() { Elements = objects.ToList() }, name);
        }

        /***************************************************/

        public static BarTemperatureLoad BarTemperatureLoad(Loadcase loadcase, double temperatureChange, BHoMGroup<Bar> group, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return new BarTemperatureLoad
            {
                Loadcase = loadcase,
                TemperatureChange = temperatureChange,
                Objects = group,
                Axis = axis,
                Projected = projected,
                Name = name
            };
        }

        /***************************************************/

        public static BarTemperatureLoad BarTemperatureLoad(Loadcase loadcase, double temperatureChange, IEnumerable<Bar> objects, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarTemperatureLoad(loadcase, temperatureChange, new BHoMGroup<Bar>() { Elements = objects.ToList() }, axis, projected, name);
        }
        /***************************************************/

        public static BarUniformlyDistributedLoad BarUniformlyDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if (force == null && moment == null)
                throw new ArgumentException("Bar uniform load requires either the force or the moment vector to be defined");

            return new BarUniformlyDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                Force = force == null? new Vector(): force,
                Moment = moment == null? new Vector():moment,
                Axis = axis,
                Name = name,
                Projected = projected
                
            };
        }

        /***************************************************/

        public static BarUniformlyDistributedLoad BarUniformlyDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarUniformlyDistributedLoad(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, force, moment, axis, projected, name);
        }

        /***************************************************/

        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, double distFromA = 0, Vector forceA = null, Vector momentA = null, double distFromB = 0, Vector forceB = null, Vector momentB = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if((forceA == null || forceB == null) && (momentA == null || momentB == null))
                throw new ArgumentException("Bar varying load requires either the force at A and B OR the moment at A and B to be defined");

            return new BarVaryingDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                DistanceFromA = distFromA,
                DistanceFromB = distFromB,
                ForceA = forceA == null ? new Vector() : forceA,
                ForceB = forceB == null ? new Vector():forceB,
                MomentA = momentA == null? new Vector() : momentA,
                MomentB = momentB == null? new Vector() : momentB,
                Projected = projected,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, double distFromA = 0, Vector forceA = null, Vector momentA = null, double distFromB = 0, Vector forceB = null, Vector momentB = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarVaryingDistributedLoad(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, distFromA, forceA, momentA, distFromB, forceB, momentB, axis, projected, name);
        }

        
        /***************************************************/

        public static GravityLoad GravityLoad(Loadcase loadcase, Vector direction, BHoMGroup<BHoMObject> group, string name = "")
        {
            return new GravityLoad
            {
                Loadcase = loadcase,
                GravityDirection = direction,
                Objects = group,
                Name = name
            };
        }

        /***************************************************/

        public static GravityLoad GravityLoad(Loadcase loadcase, Vector direction, IEnumerable<IBHoMObject> objects, string name = "")
        {
            return GravityLoad(loadcase, direction, new BHoMGroup<BHoMObject>() { Elements = objects.Cast<BHoMObject>().ToList() }, name);
        }

        /***************************************************/

        public static PointAcceleration PointAcceleration(Loadcase loadcase, BHoMGroup<Node> group, Vector translationAcc = null, Vector rotationAcc = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (translationAcc == null && rotationAcc == null)
                throw new ArgumentException("Point acceleration requires either the translation or the rotation vector to be defined");

           return new PointAcceleration
            {
                Loadcase = loadcase,
                Objects = group,
                TranslationalAcceleration = translationAcc == null? new Vector() : translationAcc,
                RotationalAcceleration = rotationAcc == null? new Vector() : rotationAcc,
                Axis = axis,
                Name = name
            };


        }

        /***************************************************/

        public static PointAcceleration PointAcceleration(Loadcase loadcase, IEnumerable<Node> objects, Vector translationAcc = null, Vector rotationAcc = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointAcceleration(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, translationAcc, rotationAcc, axis, name);
        }

        /***************************************************/

        public static PointDisplacement PointDisplacement(Loadcase loadcase, BHoMGroup<Node> group, Vector translation = null, Vector rotation = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (translation == null && rotation == null)
                throw new ArgumentException("Point displacement requires either the translation or the rotation vector to be defined");

            return new PointDisplacement
            {
                Loadcase = loadcase,
                Objects = group,
                Translation = translation == null? new Vector() : translation,
                Rotation = rotation == null? new Vector() : rotation,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        public static PointDisplacement PointDisplacement(Loadcase loadcase, IEnumerable<Node> objects, Vector translation= null, Vector rotation = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointDisplacement(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, translation, rotation, axis, name);
        }

        /***************************************************/

        public static PointForce PointForce(Loadcase loadcase, BHoMGroup<Node> group, Vector force = null, Vector moment = null,  LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (force == null && moment == null)
                throw new ArgumentException("Point force requires either the force or the moment vector to be defined");

            return new PointForce
            {
                Loadcase = loadcase,
                Objects = group,
                Force = force == null ? new Vector() : force,
                Moment = moment == null ? new Vector() : moment,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/
        public static PointForce PointForce(Loadcase loadcase, IEnumerable<Node> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointForce(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, force, moment, axis, name);
        }

        /***************************************************/

        public static PointVelocity PointVelocity(Loadcase loadcase, BHoMGroup<Node> group, Vector translation = null, Vector rotation = null,  LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (translation == null && rotation == null)
                throw new ArgumentException("Point velocity requires either the translation or the rotation vector to be defined");

            return new PointVelocity
            {
                Loadcase = loadcase,
                Objects = group,
                TranslationalVelocity = translation == null ? new Vector() : translation,
                RotationalVelocity = rotation == null ? new Vector() : rotation,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        public static PointVelocity PointVelocity(Loadcase loadcase, IEnumerable<Node> objects, Vector translation = null, Vector rotation = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointVelocity(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, translation, rotation, axis, name);
        }

        /***************************************************/
    }
}
