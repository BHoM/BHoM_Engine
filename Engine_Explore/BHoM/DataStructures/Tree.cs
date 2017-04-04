using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.DataStructure
{
    public class Tree<T>
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public T Value { get; set; }

        public List<Tree<T>> Childrens { get; set; } = new List<Tree<T>>();

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Tree() { }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
