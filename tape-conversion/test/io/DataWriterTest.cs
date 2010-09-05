using System;
using System.Collections.Generic;
using NUnit.Framework;
using tape.data;
using tape.io;

namespace test.io {

  [TestFixture]
  public class DataWriterTest {

    [Test]
    public void TestWrite() {
      DataReader reader = new DataReader();
      DataWriter writer = new DataWriter();

      SoundData original = reader.ReadSoundFile("test/sine.wav");
      writer.WriteSoundFile(original, "test/written.wav");
      SoundData written = reader.ReadSoundFile("test/written.wav");

      Assert.AreEqual(original.Length, written.Length, "The written data " +
                      "should be of the same length as the read data.");

      IEnumerator<Int16> enumWritten  = written.GetEnumerator();

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
