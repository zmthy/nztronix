using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.data {

  [TestFixture]
  public class SoundDataTest {

    [Test]
    public void TestData() {
      AudioReader reader = new AudioReader();
      SoundData data = reader.ReadSoundFile("../../../data/sine.wav");

      int length = 0;
      IEnumerator<Int16> ie = data.GetEnumerator();
      while (ie.MoveNext()) {
        length++;
      }

      Assert.AreEqual(data.Length, length, "Data length");
    }

    [Test]
    public void TestEnumerator() {
      Int16[] data = {
                       1, 2, 3, 4, 5, 6, 7, 8, 9, 10
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);
      IEnumerator<Int16> ie = audio.GetEnumerator();
      for (int i = 0; i < data.Length; ++i) {
        ie.MoveNext();
        Assert.AreEqual(data[i], ie.Current, "Enumerator values.");
      }
    }

  }

}
