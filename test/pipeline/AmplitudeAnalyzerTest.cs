using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape.Data;
using Tape.Pipeline;

namespace Test.Pipeline {

  [TestFixture]
  public class AmplitudeAnalyzerTest {

    private AmplitudeAnalyzer Analyzer = new AmplitudeAnalyzer();
    private readonly Int16[] Dirty = new Int16[12000],
                             Leader = new Int16[14400],
                             Data = new Int16[8000];
    private static readonly Int16[] Value1 = { 5000, -5000, 5000, -5000 },
                                    Value2 = { 5000, 5000, -5000, -5000 };
    private static readonly Int16[][] DataValues = { Value1, Value2 };

    public AmplitudeAnalyzerTest() {
      Random random = new Random();
      for (int i = 0; i < Dirty.Length; ++i) {
        Dirty[i] = (Int16) (random.Next(20) *
            (random.NextDouble() - 0.5 < 0 ? -1 : 1));
      }

      for (int i = 0; i < Leader.Length; i += 4) {
        for (int j = 0; j < 4; ++j) {
          Leader[i + j] = Value1[j];
        }
      }

      for (int i = 0; i < Data.Length; i += 4) {
        Int16[] value = DataValues[(int) (random.NextDouble() * 2)];
        for (int j = 0; j < 4; ++j) {
          Data[i + j] = value[j];
        }
      }
    }

    [Test]
    public void TestChunking() {
      List<Int16> data = new List<Int16>();
      data.AddRange(Dirty);
      data.AddRange(Leader);
      data.AddRange(Data);
      data.AddRange(Dirty);
      // Note that this won't be read, as there is no leader.
      data.AddRange(Data);
      data.AddRange(Dirty);
      data.AddRange(Leader);
      data.AddRange(Data);
      data.AddRange(Dirty);
      SoundData audio = new SoundData(data);
      SoundData[] chunks = Analyzer.SplitChunks(audio);
      Assert.AreEqual(2, chunks.Length, "The test should produce 2 chunks.");

      for (int i = 0; i < 2; ++i) {
        IEnumerator<Int16> ie = chunks[i].GetEnumerator();
        for (int j = 0; j < Data.Length; ++j) {
          ie.MoveNext();
          Assert.AreEqual(Data[j], ie.Current);
        }
        Assert.IsFalse(ie.MoveNext());
      }
    }

  }

}
