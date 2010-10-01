using System;
using System.Collections.Generic;
using System.Collections;

namespace Tape.Data {

  /// <summary>
  /// Holds binary data converted from audio input.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  /// <author>Casey Orr</author>
  public class BinaryData : IEnumerable<bool> {

    private readonly bool[] Data;
    public int Length {
      get {
        return Data.Length;
      }
    }

    public BinaryData(List<bool> data) {
      Data = new bool[data.Count];
      for (int i = 0; i < data.Count; ++i) {
        Data[i] = data[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<bool> GetEnumerator() {
      return new BinaryEnumerator(Data);
    }

  }

  public class BinaryEnumerator : IEnumerator<bool> {

    private readonly bool[] Data;
    private int Position = -1;
    private bool Disposed = false;

    public BinaryEnumerator(bool[] data) {
      Data = data;
    }

    object IEnumerator.Current {
      get {
        return Current;
      }
    }

    public bool Current {
      get {
        return Data[Position];
      }
    }

    public bool MoveNext() {
      if (Position < Data.Length - 1) {
        Position += 1;
        return true;
      } else {
        return false;
      }
    }

    public void Reset() {
      if (!Disposed) {
        Position = 0;
      }
    }

    public void Dispose() {
      Position = Data.Length;
      Disposed = true;
    }

  }

}
