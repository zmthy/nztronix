using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;

namespace test.data {

  [TestFixture]
  public class ByteDataTest {

    [Test]
    public void TestConversion() {
      bool[] temp = {
                      // Start bit.
                      false,
                      // Data - 00100111, or 39.
                      true, true, true, false,
                      false, true, false, false,
                      // Stop bits.
                      true, true
                    };
      List<bool> list = new List<bool>();
      list.AddRange(temp);
      ByteData data = new ByteData(new BinaryData(list));

      Assert.AreEqual(1, data.Length, "Only one byte passed.");
      IEnumerator<byte> ie = data.GetEnumerator();
      ie.MoveNext();
      Assert.AreEqual(39, ie.Current, "Expected value gained.");
      Assert.IsFalse(ie.MoveNext(), "Only one value in enumerator.");
    }

  }

}
