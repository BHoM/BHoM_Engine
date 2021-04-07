using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Humans.ViewQuality;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Humans.Modify
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Update the HeadOutline of a Spectator.")]
        [Input("spectator", "Spectator to update.")]
        [Input("newHeadOutline", "2d, closed, planar reference Polyline that represents the outline of the head. " +
            "The headOutline should be created in the XY plane where the origin represents the reference eye location of the spectator." +
            "If none provided the default is a simple Polyline based on an ellipse with major radius of 0.11 and minor radius of 0.078.")]
        public static void HeadOutline(this Spectator spectator, Polyline newHeadOutline)
        {
            if (!newHeadOutline.IsPlanar() || !newHeadOutline.IsClosed())
            {
                Reflection.Compute.RecordError("The reference headOutline must be closed and planar.");
                return;
            }
            //local cartesian
            Cartesian local = spectator.Cartesian();

            //transform the reference head outline
            TransformMatrix transform = Geometry.Create.OrientationMatrixGlobalToLocal(local);
            spectator.HeadOutline = newHeadOutline.Transform(transform);

        }

        /***************************************************/

        [Description("Update the HeadOutlines of all Spectators in an Audience.")]
        [Input("audience", "Audience to update.")]
        [Input("newHeadOutline", "2d, closed, planar reference Polyline that represents the outline of the head. " +
        "The headOutline should be created in the XY plane where the origin represents the reference eye location of the spectator." +
        "If none provided the default is a simple Polyline based on an ellipse with major radius of 0.11 and minor radius of 0.078.")]
        public static void HeadOutline(this Audience audience, Polyline newHeadOutline)
        {
            foreach (Spectator spectator in audience.Spectators)
                spectator.HeadOutline(newHeadOutline);
        }
    }
}
