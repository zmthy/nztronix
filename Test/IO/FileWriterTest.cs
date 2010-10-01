using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Text;
using Tape.Data;
using Tape.IO;

namespace Test.IO {

  [TestFixture]
  public class FileWriterTest {

    [Test]
    public void TestWrite() {
      FileWriter writer = new FileWriter();
      FileReader reader = new FileReader();

      List<bool> TestData = new List<bool>();
      bool[] genData = { true, true, true, false, true };
      TestData.AddRange(genData);
      BinaryData OriginalData = new BinaryData(TestData);

      String location = "../../../data/test.bin";

      writer.WriteBinFile(OriginalData, location);
      BinaryData ReadData = reader.ReadBinFile(location);

      IEnumerator<bool> OrigDataEnum = OriginalData.GetEnumerator();
      IEnumerator<bool> ReadDataEnum = ReadData.GetEnumerator();

      Assert.AreEqual(OriginalData.Length, ReadData.Length,
          "Written and read should be of the same length.");

      for (int i = 0; i < OriginalData.Length; i++) {
        OrigDataEnum.MoveNext();
        ReadDataEnum.MoveNext();
        Assert.AreEqual(OrigDataEnum.Current, ReadDataEnum.Current,
            "The written data must match the read data.");
      }
    }
  }
}
