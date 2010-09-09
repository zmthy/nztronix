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

      ie.MoveNext();
      Assert.AreEqual(true, ie.Current);
      ie.MoveNext();
      Assert.AreEqual(false, ie.Current);
      ie.MoveNext();
      Assert.AreEqual(true, ie.Current);
      ie.MoveNext();
      Assert.AreEqual(false, ie.Current);
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
      } catch {}
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
                       23
                     };
      SoundData audio = new SoundData(data, 0, 0, 0, 0, 0, 0);

      BinaryData bin = analyzer.ConvertToSquare(audio);
      Assert.Fail("An exception was expected, but not thrown.");
    }
  	
  }

}
