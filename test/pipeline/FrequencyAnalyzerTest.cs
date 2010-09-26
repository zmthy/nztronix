using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.io;
using tape.pipeline;

namespace test.pipeline {

  [TestFixture]
  public class FrequencyAnalyzerTest {

    private Int16[] bigs = {5000, 5000, 5000, 5000};
    private Int16[] smalls = {-5000, -5000, -5000, -5000};

    FrequencyAnalyzer analyzer = new FrequencyAnalyzer();

    [Test]
    public void TestAllRelatives() {
      RunAll();
      Random random = new Random();
      for (int i = 0; i < 5000; ++i) {
        for (int j = 0; j < 4; ++j) {
          bigs[j] = (Int16) (random.NextDouble() * 2000 + 4000);
          smalls[j] = (Int16) ((random.NextDouble() * 2000 + 4000) * -1);
        }
        RunAll();
      }
    }

    private void RunAll() {
      ConfirmResults(GenerateData(true, false, false, false));
      ConfirmResults(GenerateData(false, true, false, false));
      ConfirmResults(GenerateData(false, false, true, false));
      ConfirmResults(GenerateData(false, false, false, true));
      ConfirmResults(GenerateData(true, true, false, false));
      ConfirmResults(GenerateData(true, false, true, false));
      ConfirmResults(GenerateData(true, false, false, true));
      ConfirmResults(GenerateData(false, true, true, false));
      ConfirmResults(GenerateData(false, true, false, true));
      ConfirmResults(GenerateData(false, false, true, true));
      ConfirmResults(GenerateData(true, true, true, false));
      ConfirmResults(GenerateData(true, true, false, true));
      ConfirmResults(GenerateData(true, false, true, true));
      ConfirmResults(GenerateData(false, true, true, true));
    }

    private Int16[] GenerateData(bool one, bool two, bool three, bool four) {
      Int16[] data = {
                       one ? bigs[0] : smalls[0],
                       two ? bigs[1] : smalls[1],
                       three ? bigs[2] : smalls[2],
                       four ? bigs[3] : smalls[3]
                     };
      return data;
    }

    private void ConfirmResults(Int16[] data) {
      bool[] results;
      try {
        results = analyzer.GetRelativeSizes(data);
      } catch {
        return;
      }

      for (int i = 0; i < 4; ++i) {
        if (data[i] > 0) {
          Assert.IsTrue(results[i], ConvertOutput(data, results));
        } else {
          Assert.IsFalse(results[i], ConvertOutput(data, results));
        }
      }
    }

    private string ConvertOutput(Int16[] data, bool[] results) {
      string o = "Was ";
      foreach (Int16 i in data) {
        o += i + ", ";
      }
      o += " (";
      foreach (Int16 i in data) {
        o += i > 0 ? 'B' : 'S';
      }
      o += "), got ";
      foreach (bool b in results) {
        o += b ? 'B' : 'S';
      }
      return o;
    }

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

    [Test]
    public void TestActualData() {
      AudioReader reader = new AudioReader();
      SoundData audio = reader.ReadSoundFile("../../../data/sample.wav");
      BinaryData bin = analyzer.ConvertToSquare(audio);
      IEnumerator<bool> ie = bin.GetEnumerator();

      while (ie.MoveNext()) {
        Assert.IsTrue(ie.Current, "Data values.");
      }
    }

  }

}
