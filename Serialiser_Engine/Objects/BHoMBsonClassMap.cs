using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;

namespace BH.Engine.Serialiser.Conventions
{
    public class BHoMBsonClassMap : BsonClassMap
    {
        public BHoMBsonClassMap(Type type) : base(type)
        { }


        public BsonMemberMap MapImmutableMember(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }
            if (!(memberInfo is FieldInfo) && !(memberInfo is PropertyInfo))
            {
                throw new ArgumentException("MemberInfo must be either a FieldInfo or a PropertyInfo.", "memberInfo");
            }
            //EnsureMemberInfoIsForThisClass(memberInfo);

            //if (_frozen) { ThrowFrozenException(); }

            if (memberInfo is PropertyInfo && memberInfo.DeclaringType != this.ClassType)
            {
                PropertyInfo propInfo = memberInfo as PropertyInfo;
                if (propInfo.CanWrite)
                {
                    return new BsonMemberMap(this, memberInfo);
                }
            }

            var memberMap = this.DeclaredMemberMaps.ToList().Find(m => m.MemberInfo == memberInfo);
            if (memberMap == null)
            {
                memberMap = new BsonMemberMap(this, memberInfo);

                FieldInfo info = typeof(BsonClassMap).GetField("_declaredMemberMaps", BindingFlags.NonPublic | BindingFlags.Instance);
                var declaredMemberMaps = info.GetValue(this) as List<BsonMemberMap>;

                declaredMemberMaps.Add(memberMap);
            }
            return memberMap;
        }
    }
}
