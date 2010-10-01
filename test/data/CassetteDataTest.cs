using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape.Data;
using Tape.IO;

namespace Test.Data {

  [TestFixture]
  public class CassetteDataTest {

    private const string name = "name";

    [Test]
    public void TestCassette() {
      bool[] data = {
                       false,
                       false, false, false, false, false, false, false, false,
                       true, true
                     };
      List<bool> list = new List<bool>();
      list.AddRange(data);
      CassetteData cassette =
          new CassetteData(name, new ByteData(new BinaryData(list)));

      Assert.AreEqual(name, cassette.Filename, "Name equality.");

      Assert.AreEqual(1, cassette.Length, "Only one byte passed.");
      IEnumerator<byte> ie = cassette.GetEnumerator();
      ie.MoveNext();
      Assert.AreEqual(0, ie.Current, "Expected value gained.");
      Assert.IsFalse(ie.MoveNext(), "Only one value in enumerator.");
    }

  }

}
