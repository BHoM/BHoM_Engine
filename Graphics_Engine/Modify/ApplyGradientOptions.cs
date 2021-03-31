

using BH.Engine.Base;
using BH.oM.Graphics;
using BH.oM.Graphics.Colours;
using BH.oM.Graphics.Enums;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Sets up the properties of a GradientOptions object for usage.")]
        [Input("gradientOptions", "GradientOptions object to modify.")]
        [Input("allValues", "The values to set gradient auto range from. Optional if range is already set.")]
        [Input("defaultGradient", "Sets which gradient to use as default if no gradient is already set. Defaults to BlueToRed.")]
        [Output("gradientOptions", "A GradientOptions object which is ready for usage.")]
        [PreviousVersion("4.2", "BH.Engine.Graphics.Query.AutoRange(BH.oM.Graphics.GradientOptions, System.Collections.Generic.IEnumerable<System.Double>)")]
        [PreviousVersion("4.2", "BH.Engine.Graphics.Query.CenteringOptions(BH.oM.Graphics.GradientOptions)")]
        [PreviousVersion("4.2", "BH.Engine.Graphics.Query.DefaultGradient(BH.oM.Graphics.GradientOptions, System.String)")]
        public static GradientOptions ApplyGradientOptions(this GradientOptions gradientOptions, IEnumerable<double> allValues = null, string defaultGradient = "BlueToRed")
        {
            
            GradientOptions result = gradientOptions.ShallowClone();

            // Checks if bounds exist or can be automatically set
            if ((double.IsNaN(result.To) || double.IsNaN(result.From)) && (allValues == null || allValues.Count() < 1))
            {
                BH.Engine.Reflection.Compute.RecordError("No bounds have been manually set for Gradient, and no values are provided by which to set them.");
                return result;
            }

            // Optional auto-domain
            if (double.IsNaN(result.From))
                result.From = allValues.Min();
            if (double.IsNaN(result.To))
                result.To = allValues.Max();
            
            // Sets a default gradient if none is already set
            if (result.Gradient == null)
            {
                result.Gradient = Library.Query.Match("Gradients", defaultGradient) as Gradient;
                if (result.Gradient == null)
                {
                    Engine.Reflection.Compute.RecordError("Could not find gradient " + defaultGradient + " in the Library, make sure you have BHoM Datasets or create a custom gradient");
                    return null;
                }
            }

            // Centering Options
            switch (result.GradientCenteringOptions)
            {
                case GradientCenteringOptions.Symmetric:
                    result.To = Math.Max(Math.Abs(result.To), Math.Abs(result.From));
                    result.From = -result.To;
                    break;
                case GradientCenteringOptions.Asymmetric:
                    result.Gradient = result.Gradient.CenterGradientAsymmetric(result.From, result.To);
                    break;
                case GradientCenteringOptions.None:
                default:
                    break;
            }

            return result;
        }

        /***************************************************/

    }
}
