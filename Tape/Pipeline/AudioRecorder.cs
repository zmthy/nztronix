using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.DirectX.DirectSound;
using Tape.Data;

namespace Tape.Pipeline {

  /// <summary>
  /// Contains methods for the capturing of audio from an input.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  /// <author>Casey Orr</author>
  public class AudioRecorder {

    private bool Recording = false;
    private List<Int16> Data = null;

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
    public void Record(Capture capture) {
      if (Recording) {
        throw new Exception("Already recording.");
      }

      WaveFormat format = (WaveFormat) GetAmibiguousType(typeof(WaveFormat));
      format.SamplesPerSecond = 96000;
      format.BitsPerSample = 16;
      format.Channels = 1;
      format.FormatTag = WaveFormatTag.Pcm;
      format.BlockAlign = (Int16) (format.Channels * (format.BitsPerSample / 8));
      format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlign;

      int notifySize = Math.Max(4096, format.AverageBytesPerSecond / 16);
      notifySize -= notifySize % format.BlockAlign;

      // This is a fairly arbitrary choice.
      int inputSize = notifySize * 16;
      // Output is half of input, as every two bytes is a piece of sound data.
      int outputSize = inputSize / 2;

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

      AutoResetEvent reset = new AutoResetEvent(false);
      Notify notify = new Notify(buffer);

      BufferPositionNotify bpn1 = (BufferPositionNotify)
          GetAmibiguousType(typeof(BufferPositionNotify));
      
      bpn1.Offset = buffer.Caps.BufferBytes / 2 - 1;
      bpn1.EventNotifyHandle = reset.SafeWaitHandle.DangerousGetHandle();
      BufferPositionNotify bpn2 = (BufferPositionNotify)
          GetAmibiguousType(typeof(BufferPositionNotify));
      bpn2.Offset = buffer.Caps.BufferBytes - 1;
      bpn2.EventNotifyHandle = reset.SafeWaitHandle.DangerousGetHandle();

      notify.SetNotificationPositions(new BufferPositionNotify[] {
        bpn1, bpn2
      });

      int offset = 0;
      Data = new List<Int16>();

      Recording = true;
      new Thread((ThreadStart) delegate {
        buffer.Start(true);

        while (Recording) {
          // Let the buffer fill up from the last read.
          reset.WaitOne();

          byte[] read;
          try {
            read = (byte[]) buffer.Read(offset, typeof(byte), LockFlag.None,
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
              old = (Int16) b;
            } else {
              old = (Int16) (old | (((Int16) (b << 8))));
              Data.Add(old);
            }
            written = !written;
          }
        }

        buffer.Stop();
      }).Start();
    }

    /// <summary>
    /// Stops the recording and compiles the recorded data.
    /// </summary>
    /// 
    /// <returns>The recorded data.</returns>
    public SoundData Stop() {
      if (!Recording) {
        throw new Exception("Not currently recording.");
      }
      Recording = false;
      return new SoundData(Data);
    }

  }

}
