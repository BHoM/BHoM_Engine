using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        public static List<BH.oM.Graphics.Components.IComponent> IComponents(this IView view)
        {
            return Components(view as dynamic);
        }

        /***************************************************/
        /**** Private Methods                          ****/
        /***************************************************/
        private static List<BH.oM.Graphics.Components.IComponent> Components(this CustomView view)
        {

            return view.Children;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/
        private static List<BH.oM.Graphics.Components.IComponent> Components(this IView view)
        {
            return new List<oM.Graphics.Components.IComponent>();
        }
        /***************************************************/
        
    }
}
