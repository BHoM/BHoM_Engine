using BH.oM.Base;
using AutoBogus;
using NUnit.Framework;
using BH.Engine.Base;

namespace BH.Engine.Base
{
    public partial class Query
    {
        [SetUp]
        public void Setup()
        {
            // BH.Engine.Base.Create.RandomObject() can't deal with generics. Using AutoFaker instead. Example:
            // var test = new AutoFaker<Container<int>>().Generate();
        }

        private static IEnumerable<TestCaseData> GetTestContainers()
        {
            yield return new TestCaseData(new AutoFaker<Container<int>>().Generate(), true);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<int>>().Generate(), true);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<int>>().Generate(), true);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<int>>().Generate(), false);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<int>>().Generate(), false);
        }

        [Test, TestCaseSource(nameof(GetTestContainers))]
        public void Unpack<T>(Container<T> container, bool allSameType)
        {
            var result = container.Unpack();
            if (allSameType)
                Assert.That(result.OfType<T>().Count(), Is.EqualTo(result.Count()));
            else
                Assert.That(result.OfType<T>().Count(), Is.Not.EqualTo(result.Count()));
        }
    }
}