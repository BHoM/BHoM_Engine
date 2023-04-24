/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using MongoDB.Bson;
using BH.Engine.Versioning;
using System.Collections;
using System;

namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static BsonDocument ToBson(this object obj)
        {
            if (obj is null)
            {
                return null;
            }
            else if (obj is string)
            {
                BsonDocument document;
                BsonDocument.TryParse(obj as string, out document);
                return document;
            }
            else
            {
                BsonDocument document = new BsonDocument();
                obj.ISerialise(new MongoDB.Bson.IO.BsonDocumentWriter(document));
                if (document != null)
                    document.AddVersion();
                return document;
            }
                
        }

        /*******************************************/

        public static object FromBson(BsonDocument bson)
        {
            bool failed = false;
            object result = Compute.IDeserialise(bson, ref failed, "", false);

            if (failed)
            {
                //TODO: handle versioning here
                return result;
            }
            else
                return result;
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/


        /*******************************************/
    }
}




