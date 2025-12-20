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

using System.Numerics;
using MongoDB.Bson;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static Complex DeserialiseComplex(this BsonValue bson)
        {
            if (bson.IsBsonNull)
                return default(Complex);

            if (bson.IsBsonDocument)
            {
                BsonDocument value = bson.AsBsonDocument.GetValue("_v") as BsonDocument;
                double real = 0;
                double imaginary = 0;
                if(value.Contains("Real"))
                    real = value["Real"].AsDouble;
                else
                    Base.Compute.RecordWarning("Real property not found in the BsonDocument when deserialising Complex, default value set to 0.");

                if (value.Contains("Imaginary"))
                    imaginary = value["Imaginary"].AsDouble;
                else
                    Base.Compute.RecordWarning("Imaginary property not found in the BsonDocument when deserialising Complex, default value set to 0.");

                return new Complex(real, imaginary);
            }

            Base.Compute.RecordError("Failed to deserialise Complex number from " + bson.ToString());
            return default(Complex);
        }
    }
} 
