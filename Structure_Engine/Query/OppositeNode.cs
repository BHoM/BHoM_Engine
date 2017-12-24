﻿using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node GetOppositeNode(this Bar bar, Node node)
        {
            if (bar.EndNode.BHoM_Guid == node.BHoM_Guid)
                return bar.StartNode;
            else
                return bar.EndNode;
        }

        /***************************************************/
    }

}
