using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static object IFormulaComponent(this object obj)
        {
            if (obj == null)
                return null;

            return BH.Engine.Base.Compute.RunExtensionMethod(obj, nameof(FormulaComponent));
        }

        public static object FormulaComponent(this object obj)
        {
            return obj;
        }
    }
}
