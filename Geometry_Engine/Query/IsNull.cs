/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Geometry is null and outputs a relevant error message.")]
        [Input("geometry", "The Geometry to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Geometry is null.")]
        public static bool IsNull(this IGeometry geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            if (geometry == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                ErrorMessage(methodName, "Geometry", msg);
                
                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Public Methods - Vector                   ****/
        /***************************************************/

        [Description("Checks if a Basis is null and outputs a relevant error message.")]
        [Input("geometry", "The Basis to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Basis is null.")]
        public static bool IsNull(this Basis geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Basis", msg);
                return true;
            }
            if (geometry.X.IsNull(msg + " Basis.X failed a null test.", methodName) || 
                geometry.Y.IsNull(msg + " Basis.Y failed a null test.", methodName) || 
                geometry.Z.IsNull(msg + " Basis.Z failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Plane is null and outputs a relevant error message.")]
        [Input("geometry", "The Plane to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Plane is null.")]
        public static bool IsNull(this Plane geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Plane", msg);
                return true;
            }
            if (geometry.Origin.IsNull(msg + " Plane.Origin failed a null test.", methodName) || 
                geometry.Normal.IsNull(msg + " Plane.Normal failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Point is null and outputs a relevant error message.")]
        [Input("geometry", "The Point to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Point is null.")]
        public static bool IsNull(this Point geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Point", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a Vector is null and outputs a relevant error message.")]
        [Input("geometry", "The Vector to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Vector is null.")]
        public static bool IsNull(this Vector geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Vector", msg);
                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Public Methods - CoordinateSystem         ****/
        /***************************************************/

        [Description("Checks if a Cartesian is null and outputs a relevant error message.")]
        [Input("geometry", "The Cartesian to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Cartesian is null.")]
        public static bool IsNull(this Cartesian geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Cartesian", msg);
                return true;
            }
            if (geometry.X.IsNull(msg + " Cartesian.X failed a null test.", methodName) || 
                geometry.Y.IsNull(msg + " Cartesian.Y failed a null test.", methodName) || 
                geometry.Z.IsNull(msg + " Cartesian.Z failed a null test.", methodName) ||
                geometry.Origin.IsNull(msg + " Cartesian.Origin failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if a Arc is null and outputs a relevant error message.")]
        [Input("geometry", "The Arc to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Arc is null.")]
        public static bool IsNull(this Arc geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Arc", msg);
                return true;
            }
            if (geometry.CoordinateSystem.IsNull(msg + " Arc.CoordiateSystem failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Circle is null and outputs a relevant error message.")]
        [Input("geometry", "The Circle to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Circle is null.")]
        public static bool IsNull(this Circle geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Circle", msg);
                return true;
            }
            if ( geometry.Centre.IsNull(msg + " Circle.Centre failed a null test.", methodName) || 
                geometry.Normal.IsNull(msg + " Circle.Normal failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Ellipse is null and outputs a relevant error message.")]
        [Input("geometry", "The Ellipse to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Ellipse is null.")]
        public static bool IsNull(this Ellipse geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Ellipse", msg);
                return true;
            }
            if (geometry.Centre.IsNull(msg + " Ellipse.Centre failed a null test.", methodName) || 
                geometry.Axis1.IsNull(msg + " Ellipse.Axis1 failed a null test.", methodName) || 
                geometry.Axis2.IsNull(msg + " Ellipse.Axis2 failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Line is null and outputs a relevant error message.")]
        [Input("geometry", "The Line to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Line is null.")]
        public static bool IsNull(this Line geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Line", msg);
                return true;
            }
            if (geometry.Start.IsNull(msg + " Line.StartPoint failed a null test.", methodName) || 
                geometry.End.IsNull(msg + " Line.EndPoint failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a NurbsCurve is null and outputs a relevant error message.")]
        [Input("geometry", "The NurbsCurve to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the NurbsCurve is null.")]
        public static bool IsNull(this NurbsCurve geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "NurbsCurve", msg);
                return true;
            }
            if (geometry.ControlPoints.IsNullOrContainsNulls(msg + " NurbsCurve.ControlPoints failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a PolyCurve is null and outputs a relevant error message.")]
        [Input("geometry", "The PolyCurve to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the PolyCurve is null.")]
        public static bool IsNull(this PolyCurve geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "PolyCurve", msg);
                return true;
            }
            if (geometry.Curves.IsNullOrContainsNulls(msg + " PolyCurve.Curves failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Polyline is null and outputs a relevant error message.")]
        [Input("geometry", "The Polyline to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Polyline is null.")]
        public static bool IsNull(this Polyline geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Polyline", msg);
                return true;
            }
            if (geometry.ControlPoints.IsNullOrContainsNulls(msg + " Polyline.ControlPoints failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/
        /**** Public Methods - Mesh                     ****/
        /***************************************************/

        [Description("Checks if a CellRelation is null and outputs a relevant error message.")]
        [Input("geometry", "The CellRelation to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the CellRelation is null.")]
        public static bool IsNull(this CellRelation geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "CellRelation", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a Mesh is null and outputs a relevant error message.")]
        [Input("geometry", "The Mesh to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Mesh is null.")]
        public static bool IsNull(this Mesh geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Mesh", msg);
                return true;
            }
            if (geometry.Vertices.IsNullOrContainsNulls(msg + " Mesh.Vertices failed a null test.", methodName) ||
                geometry.Faces.IsNullOrContainsNulls(msg + " Mesh.Faces failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/

        [Description("Checks if a Mesh3D is null and outputs a relevant error message.")]
        [Input("geometry", "The Mesh3D to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Mesh3D is null.")]
        public static bool IsNull(this Mesh3D geometry, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the object
            if (geometry == null)
            {
                ErrorMessage(methodName, "Mesh", msg);
                return true;
            }
            if (geometry.Vertices.IsNullOrContainsNulls(msg + " Mesh3D.Vertices failed a null test.", methodName) ||
                geometry.Faces.IsNullOrContainsNulls(msg + " Mesh3D.Faces failed a null test.", methodName) ||
                geometry.CellRelation.IsNullOrContainsNulls(msg + " Mesh3D.CellRelation failed a null test.", methodName))
                return true;

            return false;
        }

        /***************************************************/
        /**** Public Methods - Misc                     ****/
        /***************************************************/

        /***************************************************/
        /**** Public Methods - SettingOut               ****/
        /***************************************************/

        /***************************************************/
        /**** Public Methods - Solid                    ****/
        /***************************************************/

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        /***************************************************/
        /**** Public Methods - Surface                  ****/
        /***************************************************/

        [Description("Checks if an IGeometry is null and outputs a relevant error message.")]
        [Input("geometry", "The IGeometry to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("pass", "True if the IGeometry is null.")]
        public static bool IIsNull(this IGeometry geometry, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            //check the object
            if(geometry == null)
            {
                ErrorMessage(methodName, "Geometry", msg);
                return true;
            }
            //check attributes
            return IsNull(geometry as dynamic, msg + " One or more nested attributes was found to be null.", methodName);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

    }
    
}
