using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.data {

  [TestFixture]
  public class BinaryDataTest {

    [Test]
    public void TestEnumerator() {
      List<bool> data = new List<bool>();
      for (int i = 0; i < 10; ++i) {
        data.Add(i % 2 == 0);
      }
      BinaryData bin = new BinaryData(data);

      IEnumerator<bool> ie = bin.GetEnumerator();
      for (int i = 0; i < data.Count; ++i) {
        ie.MoveNext();
        Assert.AreEqual(data[i], ie.Current, "Enumerator values.");
      }
    }

  }

}

