using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.data {

  [TestFixture]
  public class SoundDataTest {

    [Test]
    public void TestEnumerator() {
      DataReader reader = new DataReader();
      SoundData data = reader.ReadSoundFile("test/sine.wav");

      int length = 0;
      IEnumerator<Int16> e = data.GetEnumerator();
      while (e.MoveNext()) {
        length++;
      }

      Assert.AreEqual(data.Length, length);
    }

  }

}

