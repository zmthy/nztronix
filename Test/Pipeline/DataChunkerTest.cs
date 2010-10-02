using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape.Data;
using Tape.IO;
using Tape.Pipeline;

namespace Test.Pipeline {

  [TestFixture]
  public class DataChunkerTest {

    [Test]
    public void TestLeader() {
      Int16[] leader = new Int16[36000];
      Random random = new Random();
      for (int i = 0; i < leader.Length;) {
        leader[i++] = 10000;
        leader[i++] = 9000;
        leader[i++] = (Int16) (random.Next(2) < 1 ? -8000 : 8000);
        leader[i++] = -12000;
        leader[i++] = -9000;
      }
      IEnumerator<Int16> ie = new List<Int16>(leader).GetEnumerator();
      ie.MoveNext();
      Assert.IsTrue(new DataChunker().IsLeader(ie));

      for (int i = 0; i < leader.Length;) {
        leader[i++] = 10000;
        leader[i++] = 9000;
        leader[i++] = 8000;
        leader[i++] = 12000;
        leader[i++] = 9000;
      }
      ie = new List<Int16>(leader).GetEnumerator();
      ie.MoveNext();
      Assert.IsFalse(new DataChunker().IsLeader(ie));
    }

    [Test]
    public void TestRealData() {
      AudioReader reader = new AudioReader();
      SoundData data = reader.ReadSoundFile("../../../Data/file-system.wav");
      CassetteData[] cassettes = new DataChunker().ChunkData(data);
      Assert.AreEqual(1, cassettes.Length);
      CassetteData cassette = cassettes[0];
      AudioWriter writer = new AudioWriter();
      writer.WriteCassetteData(cassette, "../../../Data/processed.wav");
      data = reader.ReadSoundFile("../../../Data/processed.wav");
      cassettes = new DataChunker().ChunkData(data);
      Assert.AreEqual(1, cassettes.Length);
      CassetteData cassette2 = cassettes[0];
      Assert.AreEqual(cassette.Meta.Key, cassette2.Meta.Key);
      Assert.AreEqual(cassette.Meta.FileName, cassette2.Meta.FileName);
      Assert.AreEqual(cassette.Meta.ProgramSize, cassette2.Meta.ProgramSize);
      Assert.AreEqual(cassette.Meta.Parity, cassette2.Meta.Parity);

    }

  }

}
