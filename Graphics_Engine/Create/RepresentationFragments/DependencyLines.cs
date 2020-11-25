using BH.Engine.Reflection;
using BH.oM.Base;
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
        private static void RepresentationFragment(this DependencyLines component, object data, ViewConfig viewConfig)
        {
            object x = data.PropertyValue(component.Start);
        }
        
    }
}
