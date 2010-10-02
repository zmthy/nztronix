using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape.Data;
using Tape.IO;

namespace Test.IO {

  [TestFixture]
  public class AudioWriterTest {

    [Test]
    public void TestWrite() {
      AudioReader reader = new AudioReader();
      AudioWriter writer = new AudioWriter();

      SoundData original = reader.ReadSoundFile("../../../data/sine.wav");
      writer.WriteSoundData(original,           "../../../data/written.wav");
      SoundData written = reader.ReadSoundFile( "../../../data/written.wav");

      Assert.AreEqual(original.Length, written.Length, "The written data " +
                      "should be of the same length as the read data.");

      IEnumerator<Int16> enumWritten = written.GetEnumerator();

      foreach (Int16 sample in original) {
        enumWritten.MoveNext();
        Assert.AreEqual(sample, enumWritten.Current,
                        "The written data should match the read data.");
      }

      Assert.IsFalse(enumWritten.MoveNext());
      enumWritten.Dispose();
    }

  }

}
