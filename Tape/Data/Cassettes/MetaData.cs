using System;

namespace Tape.Data.Cassettes {

  public class MetaData : DataPart {

    public readonly string FileName;
    public readonly Int16 ProgramSize;

    public MetaData(byte key, string fileName, Int16 fileSize,
                    byte parity) : base(key, parity) {
      FileName = fileName;
      ProgramSize = fileSize;
    }

  }

}
