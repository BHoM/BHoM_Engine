/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Materials
{
    public enum MaterialColumnData
    {
        Type = 1,
        Grade = 2,
        Name = 3,
        Weight = 5,
        Mass,
        YoungsModulus,
        PoissonRatio,
        CoefThermalExpansion,
        MinimumYieldStress,
        MinimumTensileStress,
        EffectiveYieldStress,
        EffectiveTensileStress,
        CompressiveStrength,
        StainAtUltimate
    }

    /***************************************************/

    /// <summary>Steel grade</summary>
    public enum SteelGrade
    {
        /// <summary>UK S235</summary>
        S235 = 0,
        /// <summary>UK S275</summary>
        S275,
        /// <summary>UK S355</summary>
        S355,
        /// <summary>UK S420</summary>
        S420,
        /// <summary>UK S450</summary>
        S450,
        /// <summary>UK S460</summary>
        S460,
        /// <summary>Not known</summary>
        unknown
    }

    /***************************************************/

    /// <summary>
    /// 
    /// </summary>
    public enum MaterialType
    {
        Aluminium,
        Steel,
        Concrete,
        Timber,
        Rebar,
        Tendon,
        Glass,
        Cable
    }

    /***************************************************/

    /// <summary>
    /// Default materials
    /// </summary>
    public enum DefaultMaterials
    {
        /// <summary>Steel</summary>
        Steel = 1,
        /// <summary>Concrete - short term properties</summary>
        ConcreteShortTerm,
        /// <summary>Concrete - long term properties</summary>
        ConcreteLongTerm,
        /// <summary>Aluminium</summary>
        Aluminium,
        /// <summary>Glass</summary>
        Glass
    }

    /***************************************************/

    /// <summary>
    /// Material analytical model
    /// </summary>
    public enum MaterialModel
    {
        /// <summary>Elastic isotropic</summary>
        MAT_ELAS_ISO = 0,
        /// <summary>Elastic orthotropic</summary>
        MAT_ELAS_ORTHO,
        /// <summary>Elasto-plastic isotropic</summary>
        MAT_ELAS_PLAS_ISO,
        /// <summary>Fabric</summary>
        MAT_FABRIC
    }
}
