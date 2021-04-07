using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Humans.ViewQuality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Humans
{
    public static partial class Query
    {

        public static Cartesian Cartesian(this Spectator spectator)
        {
            //set local orientation
            Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, spectator.Head.PairOfEyes.ViewDirection);
            Vector viewY = Geometry.Query.CrossProduct(spectator.Head.PairOfEyes.ViewDirection, rowVector);
            Vector viewX = Geometry.Query.CrossProduct(spectator.Head.PairOfEyes.ViewDirection, viewY);
            //viewX reversed to ensure cartesian Z matches the view direction
            viewX = viewX.Reverse();
            viewX = viewX.Normalise();
            viewY = viewY.Normalise();
            //local cartesian
            Cartesian local = Geometry.Create.CartesianCoordinateSystem(spectator.Head.PairOfEyes.ReferenceLocation, viewX, viewY);

            return local;
        }
    }
}
