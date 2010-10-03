using System;
using System.IO;
using Tape.Data;
using Tape.Data.Cassettes;

namespace Tape.IO {

  public class AudioWriter {

    private const Int16 LOWER = -25000, UPPER = 25000;
    private const int SAMPLE_RATE = 48000, WRITE_RATE = 10;
    private static readonly byte[] BITS = {
                                            1, 2, 4, 8, 16, 32, 64, 128
                                          };

    public void WriteSoundData(SoundData data, string location) {
      Stream stream = GetStream(location);

      BinaryWriter writer = new BinaryWriter(stream);
      int size = (data.Length) * data.BitsPerSample / 8;

      // The "RIFF" chunk descriptor.
      writer.Write("RIFF".ToCharArray());
      writer.Write((Int32) (36 + size));
      writer.Write("WAVE".ToCharArray());

      // The "fmt" sub-chunk.
      writer.Write("fmt ".ToCharArray());
      writer.Write((Int32) 16);
      writer.Write((Int16) 1);
      writer.Write((Int16) 1);
      writer.Write((Int32) data.SampleRate);
      writer.Write((Int32) data.SampleRate * 2);
      writer.Write((Int16) 16);
      writer.Write((Int16) 16);

      // The "data" sub-chunk.
      writer.Write("data".ToCharArray());
      writer.Write((Int32) size);

      foreach (Int16 sample in data) {
        writer.Write((Int16) sample);
      }

      writer.Close();
      stream.Close();
      stream.Dispose();
    }

    public void WriteCassetteData(CassetteData data, string location) {
      Stream stream = GetStream(location);

      BinaryWriter writer = new BinaryWriter(stream);
      Console.WriteLine(SAMPLE_RATE);
      int size = SAMPLE_RATE * 22 +
                 (88 * data.Program.Length + 59888) * WRITE_RATE;

      // The "RIFF" chunk descriptor.
      writer.Write("RIFF".ToCharArray());
      writer.Write((Int32) (36 + size));
      writer.Write("WAVE".ToCharArray());

      // The "fmt" sub-chunk.
      writer.Write("fmt ".ToCharArray());
      writer.Write((Int32) 16);
      writer.Write((Int16) 1);
      writer.Write((Int16) 1);
      writer.Write((Int32) SAMPLE_RATE);
      writer.Write((Int32) SAMPLE_RATE * 2);
      writer.Write((Int16) 16);
      writer.Write((Int16) 16);

      // The "data" sub-chunk.
      writer.Write("data".ToCharArray());
      writer.Write((Int32) size);

      // 10 Seconds
      for (int i = 0; i < SAMPLE_RATE * 10; ++i) {
        writer.Write((Int16) 0);
      }

      // 3600 Bits
      for (int i = 0; i < 3600; ++i) {
        Write(writer, true);
      }

      // 1 Byte.
      Write(writer, data.Meta.Key);
      char[] name = data.Meta.FileName.ToCharArray();

      if (name.Length > 16) {
        throw new Exception("Filename of unexpected length.");
      }

      foreach (char c in name) {
        Write(writer, (byte) c);
      }

      // 16 Bytes.
      for (int i = name.Length; i < 16; ++i) {
        Write(writer, (byte) ' ');
      }

      // 3 Bytes.
      Write(writer, data.Meta.ProgramSize);
      Write(writer, data.Meta.Parity);

      // 2 Bytes.
      Write(writer, (byte) 0);
      Write(writer, (byte) 0);

      // 1 Second.
      for (int i = 0; i < SAMPLE_RATE; ++i) {
        writer.Write((Int16) 0);
      }

      // 3600 Bits.
      for (int i = 0; i < 3600; ++i) {
        Write(writer, true);
      }

      // 1 Byte.
      Write(writer, data.Program.Key);

      // Program.Length Bytes.
      foreach (byte b in data.Program) {
        Write(writer, b);
      }

      // 1 Byte.
      Write(writer, data.Program.Parity);

      // 2 Bytes.
      Write(writer, (byte) 0);
      Write(writer, (byte) 0);

      writer.Close();
      stream.Close();
      stream.Dispose();
    }

    private Stream GetStream(string location) {
      Stream stream = new FileStream(location, FileMode.Create);
      if (!stream.CanWrite) {
        throw new IOException("File not created successfully.");
      }
      return stream;
    }

    private void Write(BinaryWriter writer, byte b) {
      Write(writer, false);
      foreach (byte bit in BITS) {
        Write(writer, (b & bit) > 0);
      }
      Write(writer, true);
      Write(writer, true);
    }

    private void Write(BinaryWriter writer, Int16 i) {
      Write(writer, (byte) (i >> 8));
      Write(writer, (byte) i);
    }

    private void Write(BinaryWriter writer, bool bit) {
      WriteUpper(writer);
      if (bit) {
        WriteLower(writer);
        WriteUpper(writer);
      } else {
        WriteUpper(writer);
        WriteLower(writer);
      }
      WriteLower(writer);
    }
    
    private void WriteUpper(BinaryWriter writer) {
      for (int i = 0; i < WRITE_RATE; ++i) {
        writer.Write((Int16) UPPER);
      }
    }

    private void WriteLower(BinaryWriter writer) {
      for (int i = 0; i < WRITE_RATE; ++i) {
        writer.Write((Int16) LOWER);
      }
    }

  }

}
