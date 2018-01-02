using BH.oM.Geometry;
using BH.oM.Humans;
using System.Collections.Generic;


namespace BH.Engine.Humans
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, Line> TrackingLines(this Skeleton skeleton)
        {
            Dictionary<string, Line> lines = new Dictionary<string, Line>
            {
                { "Neck", skeleton.Neck.TrackingLine },
                { "RightShoulder", skeleton.RightShoulder.TrackingLine },
                { "LeftShoulder", skeleton.LeftShoulder.TrackingLine },
                { "UppperArmRight", skeleton.RightUpperArm.TrackingLine },
                { "LeftUpperArm", skeleton.LeftUpperArm.TrackingLine },
                { "RightLowerArm", skeleton.RightLowerArm.TrackingLine },
                { "LeftLowerArm", skeleton.LeftLowerArm.TrackingLine },
                { "RightHand", skeleton.RightHand.TrackingLine },
                { "LeftHand", skeleton.LeftHand.TrackingLine },
                { "RightThumb", skeleton.RightThumb.TrackingLine },
                { "LeftThumb", skeleton.LeftThumb.TrackingLine },
                { "Spine", skeleton.Spine.TrackingLine },
                { "RightHip", skeleton.RightHip.TrackingLine },
                { "LeftHip", skeleton.LeftHip.TrackingLine },
                { "RightUpperLeg", skeleton.RightUpperLeg.TrackingLine },
                { "RightLowerLeg", skeleton.RightLowerLeg.TrackingLine },
                { "LeftUpperLeg", skeleton.LeftUpperLeg.TrackingLine },
                { "LeftLowerLeg", skeleton.LeftLowerLeg.TrackingLine },
                { "RightFoot", skeleton.RightFoot.TrackingLine },
                { "LeftFoot", skeleton.LeftFoot.TrackingLine }
            };

            return lines;
        }

        /***************************************************/
    }
}
