using NUnit.Framework;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Base;
using BH.oM.Geometry;
using Shouldly;
using BH.oM.Test.NUnit;
using System.Reflection;

namespace BH.Tests.Engine.Structure.Query
{
    public class IGeometryTests : NUnitTest
    {
        [Test]
        public void ConcreteSection()
        {
            ConcreteSection concreteSection = (ConcreteSection)Create.RandomObject(typeof(ConcreteSection));
            IGeometry geom = concreteSection.IGeometry();
            geom.ShouldNotBeNull();
        }
    }
}