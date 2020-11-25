using BH.oM.Graphics.Components;
using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        public static void IRepresentationFragment(this IComponent component, object data, ViewConfig viewConfig)
        {
            RepresentationFragment(component as dynamic, data, viewConfig);
        }
        
        private static void RepresentationFragment(this IComponent component, object data, ViewConfig viewConfig)
        {

        }
    }
}
