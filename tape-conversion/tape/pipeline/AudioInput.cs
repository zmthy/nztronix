using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.DirectX.DirectSound;
using tape.data;

namespace tape.pipeline {

  public class AudioInput {

    public static void Main() {
      AudioInput input = new AudioInput();
      input.Record(GetCapture());
    }

    private static Capture GetCapture() {
      CaptureDevicesCollection collection = new CaptureDevicesCollection();
      for (int i = 0; i < collection.Count; ++i) {
        if (collection[i].Description.Contains("Microphone")) {
          return new Capture(collection[i].DriverGuid);
        }
      }

      throw new IOException("Expected input not found.");
    }

    private static object GetAmibiguousType(Type type) {
      ConstructorInfo cInfo = type.GetConstructor(Type.EmptyTypes);
      return cInfo.Invoke(null);
    }

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
