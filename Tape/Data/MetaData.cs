using System;

namespace Tape.Data {

  public class MetaData : DataPart {

    public readonly byte Key, Parity;
    public readonly string FileName;
    public readonly Int16 ProgramSize;

    public MetaData(byte key, string fileName, Int16 fileSize, byte parity) {
      Key = key;
      FileName = fileName;
      ProgramSize = fileSize;
      Parity = parity;
    }

  }

}
