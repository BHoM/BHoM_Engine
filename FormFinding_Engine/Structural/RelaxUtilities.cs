using FormFinding_Engine.Base;

namespace FormFinding_Engine.Structural
{

    public static class NodeProps
    {
        public const string FORCE = "Force";
        public const string MASS = "Mass";
        public const string ACC = "Acceleration";
        public const string VEL = "Velocity";
        public const string DISP = "Displacement";
        public const string POS = "Position";
        public const string ENRG = "Energy";
    }


    public static class RelaxUtilities
    {

        public static double[] NewPosition(this RelaxNode node)
        {
            double[] pos = node.Data[NodeProps.POS];
            double[] disp = node.Data[NodeProps.DISP];

            double[] newPos = new double[pos.Length];

            for (int i = 0; i < pos.Length; i++)
            {
                newPos[i] = pos[i] + disp[i];
            }

            return newPos;
        }

        public static double[] Force(this RelaxNode node)
        {
            return node.Data[NodeProps.FORCE];
        }
        public static void SetForce(this RelaxNode node, double[] values)
        {
            node.Data[NodeProps.FORCE] = values;
        }
        public static double Mass(this RelaxNode node)
        {
            return node.Data[NodeProps.MASS][0];
        }
        public static void SetMass(this RelaxNode node, double value)
        {
            node.Data[NodeProps.MASS][0] = value;
        }
        public static double[] Acceleration(this RelaxNode node)
        {
            return node.Data[NodeProps.ACC];
        }
        public static void SetAcceleration(this RelaxNode node, double[] values)
        {
            node.Data[NodeProps.ACC] = values;
        }
        public static double[] Velocity(this RelaxNode node)
        {
            return node.Data[NodeProps.VEL];
        }
        public static void SetVelocity(this RelaxNode node, double[] values)
        {
             node.Data[NodeProps.VEL] = values;
        }
        public static double[] Displacement(this RelaxNode node)
        {
            return node.Data[NodeProps.DISP];
        }
        public static double[] Position(this RelaxNode node)
        {
            return node.Data[NodeProps.POS];
        }
        public static double Energy(this RelaxNode node)
        {
            return node.Data[NodeProps.ENRG][0];
        }
    }
}
