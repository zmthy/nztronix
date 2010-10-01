using System;
using NUnit.Framework;
using Tape.Data;
using Tape.IO;
using Tape.Pipeline;

namespace Test {

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
