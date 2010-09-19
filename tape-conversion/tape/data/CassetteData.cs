using System;

namespace tape.data {
  
  /// <summary>
  /// Represents the data on the cassette in object form.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class CassetteData {

    public readonly String Filename;
    public readonly int Length;
    public readonly ByteData Program;

    public CassetteData(String filename, int length, ByteData program) {
      Filename = filename;
      Length = length;
      Program = program;
    }

  }
  
}
