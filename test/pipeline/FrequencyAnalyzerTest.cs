using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.pipeline;

namespace test.pipeline {

  [TestFixture]
  public class FrequencyAnalyzerTest {

    FrequencyAnalyzer analyzer = new FrequencyAnalyzer();

    [Test]
    public void TestGoodData() {
      Int16[] data = {
                       5000,
                       50,
                       5000,
                       50,
                       5000,
                       5000,
                       50,
                       50,
                       5000,
                       50,
                       5000,
                       50,
                       5000,
                       5000,
                       50,
                       50
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);
      BinaryData bin = analyzer.ConvertToSquare(audio);
      IEnumerator<bool> ie = bin.GetEnumerator();

      for (int i = 0; i < 4; ++i) {
        ie.MoveNext();
        Assert.AreEqual(i % 2 == 0, ie.Current, "Sample " + i);
      }
    }

    [Test]
    public void TestBadData() {
      Int16[] data = {
                       50,
                       50,
                       50,
                       50
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);

      try {
        BinaryData bin = analyzer.ConvertToSquare(audio);
        Assert.Fail("An exception was expected, but not thrown.");
      } catch { }
    }

    [Test]
    public void TestLowQualityData() {
      Int16[] data = {
                       5230,
                       23,
                       64,
                       4654,
                       43,
                       53,
                       4987,
                       5231,
                       54,
                       34,
                       34
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);
      BinaryData bin = analyzer.ConvertToSquare(audio);
      IEnumerator<bool> ie = bin.GetEnumerator();

      for (int i = 0; i < 2; ++i) {
        ie.MoveNext();
        Assert.AreEqual(i % 2 == 0, ie.Current, "Sample " + i);
      }
    }

  }

}
