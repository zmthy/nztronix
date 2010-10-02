using System;
using System.Collections.Generic;
using Tape.Data;

namespace Tape.Pipeline.Parsers {

  public class MetaParser : Parser {

    public MetaParser(IEnumerator<Int16> ie, int rate) : base(ie, rate) {}

    public override DataPart Parse() {
      // Ignore the rest of the Leader, if we still caught some.
      EatLeader();

      // We've already read the start bit here, so just ignore it.
      byte keyCode = PostNextByte();

      string fileName = "";
      for (int i = 0; i < 16; ++i) {
        fileName += (char) NextByte();
      }
      
      fileName = fileName.Trim();

      Int16 fileSize = NextInt16();
      byte parity = NextByte();

      return new MetaData(keyCode, fileName, fileSize, parity);
    }

  }

}
