using System;

namespace tape.data {
  
  public class CassetteData {

    public readonly BinaryData Meta, Data;

    public CassetteData(BinaryData meta, BinaryData data) {
      Meta = meta;
      Data = data;
    }

  }
  
}
