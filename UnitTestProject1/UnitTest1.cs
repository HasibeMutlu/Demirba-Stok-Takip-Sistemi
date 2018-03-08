using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YazılımSınamaProje1;

namespace YazilimTesti
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ytesti test1 = new ytesti();
        }
        [TestMethod]
        public void yetkiligiris()
        {
            ytesti test1 = new ytesti();
            int i = test1.yetkiligirisikontrol("Hasibe", "2121");
            Assert.AreEqual(2121, i);
        }
    }
}
