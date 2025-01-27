/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification.Results;
using BH.oM.Verification.Specifications;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Verifies an object against a specification and returns a result object with all necessary details.")]
        [Input("objects", "Objects to check against the specification.")]
        [Input("specification", "Specification to check the objects against.")]
        [Output("result", "Result object containing reference to the input specification as well as requirement verification results per each pair of extracted object and requirement.")]
        public static SpecificationResult IVerifySpecification(this IEnumerable<object> objects, ISpecification specification)
        {
            if (objects == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify specification against null objects.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(objects, nameof(VerifySpecification), new object[] { specification }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because specification of type {specification.GetType().Name} is currently not supported.");
                return null;
            }

            return (SpecificationResult)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Verifies an object against a specification and returns a result object with all necessary details.")]
        [Input("objects", "Objects to check against the specification.")]
        [Input("specification", "Specification to check the objects against.")]
        [Output("result", "Result object containing reference to the input specification as well as requirement verification results per each pair of extracted object and requirement.")]
        public static SpecificationResult VerifySpecification(this IEnumerable<object> objects, Specification specification)
        {
            // Extract the objects to verify
            List<object> extracted = objects.IExtract(specification.Extraction);

            // Then apply the check to the extracted objects
            List<RequirementResult> requirementResults = extracted.SelectMany(x => specification.Requirements.Select(y => IVerifyRequirement(x, y))).ToList();

            // Finally return the result
            return new SpecificationResult(specification, extracted, requirementResults);
        }

        /***************************************************/
    }
}

