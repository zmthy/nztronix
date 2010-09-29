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
      AmplitudeAnalyzer ampAnalyzer = new AmplitudeAnalyzer();
      ampAnalyzer.SetVerbosity(true);
      SoundData[] chunks = ampAnalyzer.SplitChunks(audio);
      Assert.AreEqual(2, chunks.Length, "Two chunks are in the file.");
    }

  }

}
