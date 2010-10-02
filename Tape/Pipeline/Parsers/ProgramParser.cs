using System;
using System.Collections.Generic;
using Tape.Data;

namespace Tape.Pipeline.Parsers {
  
  public class ProgramParser : Parser {

    private readonly int Size;

    public ProgramParser(IEnumerator<Int16> ie, int rate,
                         int programSize) : base(ie, rate) {
      Size = programSize;
    }

    public override DataPart Parse() {
      // Ignore the rest of the Leader, if we still caught some.
      EatLeader();

      //Verbose = true;
      //for (int i = 0; i < 8; ++i) {
      //  Console.Write(Data.ToString());
      //  Console.Write(" " + NextBit() + " ");
      //  Console.WriteLine(Data.ToString());
      //}

      // We've already read the start bit here, so just ignore it.
      byte keyCode = PostNextByte();

      byte[] program = new byte[Size];
      for (int i = 0; i < program.Length; ++i) {
        program[i] = NextByte();
      }
      
      return new ProgramData(keyCode, program);
    }

  }

}
