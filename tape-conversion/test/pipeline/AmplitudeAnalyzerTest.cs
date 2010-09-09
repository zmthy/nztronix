using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.pipeline;

namespace tapeconversion.test.pipeline {

  [TestFixture]
  class AmplitudeAnalyzerTest {

    private AmplitudeAnalyzer analyzer = new AmplitudeAnalyzer();

    [Test]
    public void TestChunking() {
      Int16[] data = {
                       12, 13, 15, 12, 16, 12, 13, 14, 15, 13, 12, 13,
                       5000,
                       50,
                       5000,
                       50,
                       5000,
                       5000,
                       50,
                       50,
                       12, 13, 15, 12, 16, 12, 13, 14, 15, 13, 12, 13,
                       5000,
                       50,
                       5000,
                       50,
                       5000,
                       5000,
                       50,
                       50,
                       12, 13, 15, 12, 16, 12, 13, 14, 15, 13, 12, 13
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);
      SoundData[] chunks = analyzer.SplitChunks(audio);
      Assert.AreEqual(2, chunks.Length, "The test should produce 2 chunks.");

      for (int a = 0; a < 2; ++a) {
        IEnumerator<Int16> ie = chunks[a].GetEnumerator();
        for (int i = 12; i < 20; ++i) {
          ie.MoveNext();
          Assert.AreEqual(data[i], ie.Current);
        }
        Assert.IsFalse(ie.MoveNext());
      }
    }

  }

}
