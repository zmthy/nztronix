using System;
using System.IO;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.io {

  [TestFixture]
  public class DataReaderTest {

    [Test]
    public void TestRead() {
      DataReader reader = new DataReader();
      SoundData data = reader.ReadSoundFile("test/sine.wav");
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
