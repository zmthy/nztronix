using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape;
using Tape.Data;
using Tape.Data.Cassettes;
using Tape.IO;
using Tape.Pipeline;

namespace Test {

  [TestFixture]
  public class TapeTester {

    [Test]
    public void TestPipeline() {
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
      Assert.AreEqual(cassette.Program.Length, cassette2.Program.Length);
      Assert.AreEqual(cassette.Program.Key, cassette2.Program.Key);
      Assert.AreEqual(cassette.Program.Parity, cassette2.Program.Parity);
      IEnumerator<byte> bytes = cassette.Program.GetEnumerator();
      foreach (byte b in cassette2.Program) {
        bytes.MoveNext();
        Assert.AreEqual(bytes.Current, b);
      }
    }

    [Test]
    public void TestObject() {
      TapeConverter converter = new TapeConverter();
      converter.Process("../../../Data/file-system.wav", "../../../Data");
    }

  }

}
