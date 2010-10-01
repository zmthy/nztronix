using System;
using System.IO;
using Tape.Data;

namespace Tape.IO {

  public class FileWriter {

    public void WriteBinFile(BinaryData Data, String Location) {
      StreamWriter writer = new StreamWriter(Location);
      foreach (bool i in Data) {
        writer.Write(i ? '1' : '0');
      }
      writer.Close();
    }
  }
}
