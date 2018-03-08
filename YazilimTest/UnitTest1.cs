using Microsoft.VisualStudio.TestTools.UnitTesting;
using static YazılımSınamaProje1.frmDemirbasStokTakip;

namespace YazilimTest
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
        public void yetkiligirisi()
        {
            ytesti test1 = new ytesti();
            int i = test1.yetkiligirisikontrol("Hasibe", "2121");
            Assert.AreEqual(2121, i);
        }
        [TestMethod]
        public void TestMethod2()
        {
            yazilimTesti test2 = new yazilimTesti();
        }
        [TestMethod]
        public void personelgiris()
        {
            yazilimTesti test2 = new yazilimTesti();
            int i = test2.personelgiriskontrol("Intizar", "4321");
            Assert.AreEqual(4321, i);
        }
    }
}