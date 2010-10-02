using System;
using System.Collections;
using System.Collections.Generic;

namespace Tape.Data {

  /// <summary>
  /// Holds byte data converted from binary data.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class ByteData : IEnumerable<byte> {

    private readonly byte[] Data;
    public int Length {
      get {
        return Data.Length;
      }
    }

    /// <summary>
    /// Converts the given binary data into byte-based data.
    /// </summary>
    /// 
    /// <param name="data">The binary data to convert.</param>
    public ByteData(BinaryData data) {
      if (data.Length % 11 != 0) {
        throw new Exception("Binary length does not match byte length.");
      }

      Data = new byte[data.Length / 11];
      IEnumerator<bool> ie = data.GetEnumerator();

      for (int index = 0; ie.MoveNext(); ++index) {
        if (ie.Current != false) {
          throw new Exception("Start bit does not match specification.");
        }

        byte b = 0;
        for (int i = 0; i < 8; ++i) {
          MoveNext(ie);
          if (ie.Current) {
            b += (byte) (1 << i);
          }
        }

        Data[index] = b;

        for (int i = 0; i < 2; ++i) {
          MoveNext(ie);
          if (ie.Current != true) {
            throw new Exception("Start bit does not match specification.");
          }
        }
      }
    }

    private void MoveNext(IEnumerator<bool> ie) {
      if (!ie.MoveNext()) {
        throw new Exception("Enumerator ended prematurely.");
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<byte> GetEnumerator() {
      return new ByteEnumerator(Data);
    }

  }

}
