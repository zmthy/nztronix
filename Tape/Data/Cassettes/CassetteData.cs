using System;
using System.Collections.Generic;

namespace Tape.Data.Cassettes {

  public class CassetteData {

    public readonly MetaData Meta;
    public readonly ProgramData Program;

    public CassetteData(MetaData meta, ProgramData program) {
      Meta = meta;
      Program = program;
    }

  }

}
