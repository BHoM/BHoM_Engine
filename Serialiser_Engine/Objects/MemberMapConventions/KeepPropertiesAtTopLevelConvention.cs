using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using MongoDB.Bson.Serialization.Conventions;


namespace BH.Engine.Serialiser.MemberMapConventions
{
    public class KeepPropertiesAtTopLevelConvention : IMemberMapConvention
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public string Name { get { return "KeepPropertiesAtTopLevel"; } }


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public void Apply(BsonMemberMap memberMap)
        {
            Console.WriteLine("hey");
        }

        /*******************************************/
    }

}
