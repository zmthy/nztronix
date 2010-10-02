using System;
using System.Collections;
using System.Collections.Generic;

namespace Tape.Data {

  public class ProgramData : DataPart, IEnumerable<byte> {

    public readonly byte Key;
    private readonly byte[] Program;
    public int Length {
      get {
        return Program.Length;
      }
    }

    public ProgramData(byte key, byte[] program) {
      Key = key;
      Program = program;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<byte> GetEnumerator() {
      return new ByteEnumerator(Program);
    }

  }

  public class ByteEnumerator : IEnumerator<byte> {

    private readonly byte[] Data;
    private int Position = -1;
    private bool Disposed = false;

    public ByteEnumerator(byte[] data) {
      Data = data;
    }

    object IEnumerator.Current {
      get {
        return Current;
      }
    }

    public byte Current {
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
