using System;
using NUnit.Framework;
using tape.data;
using tape.io;
using tape.pipeline;

namespace test {

  [TestFixture]
  public class TapeTester {

    [Test]
    public void TestPipeline() {
      AudioReader reader = new AudioReader();
      SoundData audio = reader.ReadSoundFile("../../../data/file-system.wav");
      SoundData[] chunks = new AmplitudeAnalyzer().SplitChunks(audio);
      Assert.AreEqual(2, chunks.Length, "Two chunks are in the file.");
    }

    public static void Main() {}

  }

}
