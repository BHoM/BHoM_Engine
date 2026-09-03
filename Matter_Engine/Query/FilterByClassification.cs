/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;
using System.Collections.Generic;
using System;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Filters a collection of Material by a MaterialClassification. Requires the provided Materials to contain a MaterialClassification as one of its properties to be able to filter. The filtering is done by matching all non-empty properties of the classification, e.g. if the classification has a non-empty Category and Type, but an empty Grade and Constituent, then all materials with the same Category and Type will be returned.")]
        [Input("materials", "The collection of materials to filter.")]
        [Input("classification", "The material classification to filter by.")]
        [Output("filteredMaterials", "The filtered collection of materials.")]
        public static List<Material> FilterByClassification(this IEnumerable<Material> materials, MaterialClassification classification)
        {
            if (materials == null)
            {
                Base.Compute.RecordError("Cannot filter by material classification on a null collection of materials.");
                return null;
            }

            return materials.Where(m => m.MaterialClassification().ClassificationMatches(classification)).ToList();
        }

        /******************************************/

        [Description("Filters a collection of Material by classification properties. Requires the provided Materials to contain a MaterialClassification as one of its properties to be able to filter. The filtering is done by matching all non-empty provided properties, e.g. if the category and type are provided, but grade and constituent are empty, then all materials with the same category and type will be returned.")]
        [Input("materials", "The collection of materials to filter.")]
        [Input("category", "The category to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("type", "The type to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("grade", "The grade to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Output("filteredMaterials", "The filtered collection of materials.")]
        public static List<Material> FilterByClassification(this IEnumerable<Material> materials, string category = "", string type = "", string grade = "", string constituent = "")
        {
            if (materials == null)
            {
                Base.Compute.RecordError("Cannot filter by material classification on a null collection of materials.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(grade) && string.IsNullOrWhiteSpace(constituent))
            {
                return materials.ToList();
            }

            return materials.FilterByClassification(new MaterialClassification() { Category = category, Type = type, Grade = grade, Constituent = constituent });
        }

        /******************************************/

        [Description("Filters a collection of IMaterialProperties by a material classification. Requires the provided IMaterialProperties to contain a MaterialClassification as a Fragment to be able to filter. The filtering is done by matching all non-empty properties of the classification, e.g. if the classification has a non-empty Category and Type, but an empty Grade and Constituent, then all materials with the same Category and Type will be returned.")]
        [Input("materialProperties", "The collection of material properties to filter.")]
        [Input("classification", "The material classification to filter by.")]
        [Output("filteredProperties", "The filtered collection of material properties.")]
        public static List<T> FilterByClassification<T>(this IEnumerable<T> materialProperties, MaterialClassification classification) where T : IMaterialProperties
        {
            if (materialProperties == null)
            {
                Base.Compute.RecordError("Cannot filter by material classification on a null collection of material properties.");
                return null;
            }

            return materialProperties.Where(m => m.MaterialClassification().ClassificationMatches(classification)).ToList();
        }

        /******************************************/

        [Description("Filters a collection of IMaterialProperties by classification properties. Requires the provided IMaterialProperties to contain a MaterialClassification as a Fragment to be able to filter. The filtering is done by matching all non-empty provided properties, e.g. if the category and type are provided, but grade and constituent are empty, then all materials with the same category and type will be returned.")]
        [Input("materials", "The collection of materials to filter.")]
        [Input("category", "The category to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("type", "The type to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("grade", "The grade to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, this property will not be used for filtering.")]
        [Output("filteredMaterials", "The filtered collection of materials.")]
        public static List<T> FilterByClassification<T>(this IEnumerable<T> materials, string category = "", string type = "", string grade = "", string constituent = "") where T : IMaterialProperties
        {
            if (materials == null)
            {
                Base.Compute.RecordError("Cannot filter by material classification on a null collection of material properties.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(grade) && string.IsNullOrWhiteSpace(constituent))
            {
                return materials.ToList();
            }

            return materials.FilterByClassification(new MaterialClassification() { Category = category, Type = type, Grade = grade, Constituent = constituent });
        }

        /******************************************/
        /****  Private Methods                 ****/
        /******************************************/

        private static bool ClassificationMatches(this MaterialClassification classification, MaterialClassification other)
        {
            if (classification == null || other == null)
                return false;

            if (!classification.Category.OtherStringEmptyOrEqual(other.Category))
                return false;

            if (!classification.Type.OtherStringEmptyOrEqual(other.Type))
                return false;

            if (!classification.Grade.OtherStringEmptyOrEqual(other.Grade))
                return false;

            if (!classification.Constituent.OtherStringEmptyOrEqual(other.Constituent))
                return false;

            return true;
        }

        /******************************************/

        private static bool OtherStringEmptyOrEqual(this string classificationString, string other)
        {
            //Do not filter by empty strings
            if (string.IsNullOrWhiteSpace(other)) 
                return true;

            if (ReferenceEquals(classificationString, other))
                return true;

            if (classificationString is null)
                return false;

            int i = 0;
            int j = 0;

            //Ignore case when comparing classification properties
            //This is to avoid issues with different casing in the classification properties, e.g. "Concrete" vs "concrete"
            //The comparisons below also ignore any whitespaces to make sure for example `CEM 1` matches to `CEM1` as well as `CEM 1 `
            while (true)
            {
                while (i < classificationString.Length && char.IsWhiteSpace(classificationString[i]))
                    i++;

                while (j < other.Length && char.IsWhiteSpace(other[j]))
                    j++;

                bool classificationEnd = i >= classificationString.Length;
                bool otherEnd = j >= other.Length;

                if (classificationEnd || otherEnd)
                    return classificationEnd && otherEnd;

                if (char.ToUpperInvariant(classificationString[i]) != char.ToUpperInvariant(other[j]))
                    return false;

                i++;
                j++;
            }
        }

        /******************************************/
    }
}






