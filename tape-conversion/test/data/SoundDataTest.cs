using System;
using System.IO;
using NUnit.Framework;
using tape.data;

namespace test.data {

  [TestFixture]
  public class SoundDataTest {

    [Test]
    public void TestLoad() {
      Stream stream = new FileStream("test/sine.wav", FileMode.Open);
      if (!stream.CanRead) {
        Assert.Fail("Test file is missing.");
      }

      SoundData data = new SoundData(stream);
      Assert.AreEqual(1, data.Duration, "The expected duration is one second.");
    }

  }

}
