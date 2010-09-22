using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.DirectX.DirectSound;
using tape.data;

namespace tape.pipeline {

  /// <summary>
  /// Contains methods for the capturing of audio from an input.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class AudioRecorder {

    /// <summary>
    /// Visual Studio doesn't like the constructors of some types that we need
    /// in this class. This method will return an instance of the given type by
    /// calling its constructor with no parameters.
    /// </summary>
    /// 
    /// <param name="type">The type of object to return.</param>
    /// <returns>A new object with the given type.</returns>
    private static object GetAmibiguousType(Type type) {
      ConstructorInfo cInfo = type.GetConstructor(Type.EmptyTypes);
      return cInfo.Invoke(null);
    }

    /// <summary>
    /// Records sound data from the given audio input.
    /// </summary>
    ///
    /// <remarks>
    /// Note that this method will block forever. Threading will be required
    /// to get the data back.
    /// </remarks>
    /// 
    /// <param name="capture">The input to record from.</param>
    /// <returns>The audio data recorded from the input.</returns>
    public SoundData Record(Capture capture) {
      WaveFormat format = (WaveFormat) GetAmibiguousType(typeof(WaveFormat));
      format.SamplesPerSecond = 96000;
      format.BitsPerSample = 16;
      format.Channels = 1;
      format.FormatTag = WaveFormatTag.Pcm;
      format.BlockAlign = (Int16) (format.Channels * (format.BitsPerSample / 8));
      format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlign;

      int notifySize = Math.Max(4096, format.AverageBytesPerSecond / 16);
      notifySize -= notifySize % format.BlockAlign;
      int inputSize = notifySize * 16;
      int outputSize = notifySize * 8;

      CaptureBufferDescription description = (CaptureBufferDescription)
          GetAmibiguousType(typeof(CaptureBufferDescription));
      description.Format = format;
      description.BufferBytes = inputSize;

      CaptureBuffer buffer;
      try {
        buffer = new CaptureBuffer(description, capture);
      } catch {
        throw new IOException(
            "An error occurred attempting to set up a read buffer.");
      }

      int offset = 0;
      List<Int16> data = new List<Int16>();

      buffer.Start(true);

      // for (int i = 0; i < 10000; ++i) {
      Array read;
      try {
        read = buffer.Read(offset, typeof(byte), LockFlag.None,
                           outputSize);
      } catch {
        throw new IOException(
            "An error occurred attempting to read the input data.");
      }
      offset = (offset + outputSize) % inputSize;

      bool written = false;
      Int16 old = 0;
      foreach (byte b in read) {
        if (!written) {
          old = (Int16) (b << 8);
        } else {
          old = (Int16) (old & b);
          data.Add(old);
        }
        written = !written;
      }
      // }

      foreach (Int16 d in data) {
        Console.Write(d + " ");
      }

      buffer.Stop();

      return null;
    }

  }

}
