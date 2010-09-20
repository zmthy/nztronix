using System;
using System.IO;
using tape.data;

namespace tape.io {

  public class AudioWriter {

    private const Int16 LOWER = 50, UPPER = 5000;
    private static readonly byte[] BITS = {
                                            1, 2, 4, 8, 16, 32, 64, 128
                                          };

    public void WriteSoundData(SoundData data, string location) {
      Stream stream = GetStream(location);

      BinaryWriter writer = new BinaryWriter(stream);
      int size = data.Length * data.BitsPerSample / 8;

      // The "RIFF" chunk descriptor.
      writer.Write("RIFF".ToCharArray());
      writer.Write((Int32) (36 + size));
      writer.Write("WAVE".ToCharArray());

      // The "fmt" sub-chunk.
      writer.Write("fmt ".ToCharArray());
      writer.Write((Int32) 16);
      writer.Write((Int16) data.CompressionCode);
      writer.Write((Int16) 1);
      writer.Write((Int32) data.SampleRate);
      writer.Write((Int32) data.BytesPerSecond);
      writer.Write((Int16) data.BlockAlign);
      writer.Write((Int16) data.BitsPerSample);

      // The "data" sub-chunk.
      writer.Write("data".ToCharArray());
      writer.Write((Int32) size);

      foreach (Int16 sample in data) {
        writer.Write(sample);
      }

      writer.Close();
      stream.Close();
      stream.Dispose();
    }

    public void WriteCassetteData(CassetteData data, string location) {
      Stream stream = GetStream(location);

      BinaryWriter writer = new BinaryWriter(stream);
      int size = 0;

      // The "RIFF" chunk descriptor.
      writer.Write("RIFF".ToCharArray());
      writer.Write((Int32) (36 + size));
      writer.Write("WAVE".ToCharArray());

      // The "fmt" sub-chunk.
      writer.Write("fmt ".ToCharArray());
      writer.Write((Int32) 16);
      // writer.Write((Int16) data.CompressionCode);
      writer.Write((Int16) 1);
      // writer.Write((Int32) data.SampleRate);
      // writer.Write((Int32) data.BytesPerSecond);
      // writer.Write((Int16) data.BlockAlign);
      // writer.Write((Int16) data.BitsPerSample);

      // The "data" sub-chunk.
      writer.Write("data".ToCharArray());
      writer.Write((Int32) size);

      foreach (byte b in data) {
        Write(writer, false);
        foreach (byte bit in BITS) {
          Write(writer, (b & bit) > 0);
        }
        Write(writer, true);
        Write(writer, true);
      }

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

    private void Write(BinaryWriter writer, bool bit) {
      writer.Write((Int16) UPPER);
      if (bit) {
        writer.Write((Int16) LOWER);
        writer.Write((Int16) UPPER);
      } else {
        writer.Write((Int16) UPPER);
        writer.Write((Int16) LOWER);
      }
      writer.Write((Int16) LOWER);
    }

  }

}
