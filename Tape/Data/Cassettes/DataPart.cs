using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tape.Data.Cassettes {

  public abstract class DataPart {

    public readonly byte Key, Parity;

    public DataPart(byte key, byte parity) {
      Key = key;
      Parity = parity;
    }

  }

}
