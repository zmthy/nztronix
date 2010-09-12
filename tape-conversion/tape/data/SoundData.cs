using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace tape.data {

  /// <summary>
  /// Holds the sound data recorded from a cassette tape.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class SoundData : IEnumerable<Int16> {

    private readonly Int16[] Data;
    public readonly double Duration;
    public readonly int CompressionCode,
                    SampleRate,
                        BytesPerSecond,
                        BitsPerSample,
                        BlockAlign;
    public int Length {
      get {
        return Data.Length;
      }
    }

    public SoundData(Int16[] data, int compressionCode, int sampleRate,
                 int bytesPerSecond, int bitsPerSample, int blockAlign,
                 double duration) {
      CompressionCode = compressionCode;
      SampleRate = sampleRate;
      BytesPerSecond = bytesPerSecond;
      BitsPerSample = bitsPerSample;
      BlockAlign = blockAlign;
      Duration = duration;
      Data = new Int16[data.Length];
      for (int i = 0; i < data.Length; ++i) {
        Data[i] = data[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<Int16> GetEnumerator() {
      return new DataEnumerator(Data);
    }

    public class DataEnumerator : IEnumerator<Int16> {

      private readonly Int16[] Data;
      private int Position = -1;
      private bool Disposed = false;

      public DataEnumerator(Int16[] data) {
        Data = data;
      }

      object IEnumerator.Current {
        get {
          return Current;
        }
      }

      public Int16 Current {
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
          Position = -1;
        }
      }

      public void Dispose() {
        Position = Data.Length;
        Disposed = true;
      }

    }

  }

}
