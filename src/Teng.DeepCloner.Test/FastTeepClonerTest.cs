using FastDeepCloner;

namespace Teng.DeepCloner.Test
{
    public class FastTeepClonerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestObject2InterfaceConverter()
        {
            User user = new User() { Name = "Test" };
            IUser stu = user.ActAsInterface<IUser>();

            Assert.AreEqual(user.Name, stu.Name);

            Assert.Pass();
        }
    }
}