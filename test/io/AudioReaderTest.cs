using System;
using System.IO;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.io {

  [TestFixture]
  public class AudioReaderTest {

    [Test]
    public void TestRead() {
      AudioReader reader = new AudioReader();
      SoundData data = reader.ReadSoundFile("../../../data/sine.wav");
      Assert.AreEqual(1, data.Duration,
                      "The duration of the data is one second");
    }

    [Test]
    public void TestCassette() {
      // DataReader reader = new DataReader();
      // reader.ReadSoundFile("test/record.wav");
    }

  }

}
