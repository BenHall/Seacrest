using NUnit.Framework;

namespace UnitTesting1.Tests
{
    [TestFixture]
    public class OneMethodClassTests
    {
        [Test]
        public void Test_Example1_With_One_Method_Call()
        {
            OneMethodClass oneMethodClass = new OneMethodClass();
            string v = oneMethodClass.Method1();
            Assert.That(v, Is.EqualTo("Method1 String"));
        }

        [Test]
        public void Test_Example2_With_One_Method_Call()
        {
            OneMethodClass oneMethodClass = new OneMethodClass();
            string v = oneMethodClass.Method2();
            Assert.That(v, Is.EqualTo("Method2 String"));
        }
    }
}
