using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Tape.Data {

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

    public SoundData(List<Int16> data) {
      Data = new Int16[data.Count];
      for (int i = 0; i < data.Count; ++i) {
        Data[i] = data[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return (IEnumerator) GetEnumerator();
    }

    public IEnumerator<Int16> GetEnumerator() {
      return new SoundEnumerator(Data);
    }

    public SoundEnumerator GetSoundEnumerator() {
      return new SoundEnumerator(Data);
    }

    public class SoundEnumerator : IEnumerator<Int16> {

      private readonly Int16[] Data;
      private int Position = -1;
      private bool Disposed = false;

      public SoundEnumerator(Int16[] data) {
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

      public Int16[] CurrentFour {
        get {
          if (Position > Data.Length - 4) {
            return null;
          }
          Int16[] data = new Int16[4];
          for (int i = 0; i < 4; ++i) {
            data[i] = Data[Position + i];
          }
          return data;
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

      public bool MoveNextFour() {
        if (Position < Data.Length - 4) {
          Position += 4;
          return true;
        } else {
          Position = Data.Length;
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
