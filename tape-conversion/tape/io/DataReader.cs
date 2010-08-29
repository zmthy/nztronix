using System;
using System.IO;
using tape.data;

namespace tape.io {

  /// <summary>
  /// Reads Wave files and outputs a SoundData that holds the audio data.
  /// </summary>
  ///
  /// <author>Timothy Jones</author>
  public class DataReader {

    private static readonly int BitsPerByte = 8, MaxBits = 16;

    /// <summary>
    /// Reads a Wave file and outputs the audio data.
    /// </summary>
    ///
    /// <remarks>
    /// See https://ccrma.stanford.edu/courses/422/projects/WaveFormat/ for the
    /// structure of a wave file.
    /// </remarks>
    ///
    /// <param name="source">
    /// The location of the audio file to read.
    /// </param>
    /// <returns>
    /// The audio data as a <see cref="tape.data.SoundData"/> file.
    /// </returns>
    public SoundData ReadSoundFile(string source) {
      Stream stream = new FileStream(source, FileMode.Open);
      if (!stream.CanRead) {
        throw new IOException("File not found.");
      }

      BinaryReader reader = new BinaryReader(stream);

      // The "RIFF" chunk descriptor.
      reader.ReadChars(4);
      reader.ReadInt32();

      if (new string(reader.ReadChars(4)) != "WAVE") {
        throw new IOException("Unexpected audio format.");
      }

      // The "fmt" sub-chunk.
      string chunkName = new string(reader.ReadChars(4));
      int chunkLength  = reader.ReadInt32();
      
      int compressionCode = reader.ReadInt16(),
          channelCount    = reader.ReadInt16(),
          sampleRate      = reader.ReadInt32(),
          bytesPerSecond  = reader.ReadInt32(),
          blockAlign      = reader.ReadInt16(),
          bitsPerSample   = reader.ReadInt16();

      if (MaxBits % bitsPerSample != 0) {
        throw new IOException("The input stream uses an unhandled " +
                            "significant bits per sample parameter.");
      }

      // The "data" sub-chunk
      reader.ReadChars(chunkLength - 16);
      chunkName = new string(reader.ReadChars(4));
      
      try {
        while (chunkName.ToLower() != "data") {
          reader.ReadChars(reader.ReadInt32());
          chunkName = new string(reader.ReadChars(4));
        }
      } catch {
        throw new IOException("Error while reading the data.");
      }
      
      chunkLength = reader.ReadInt32();
      int frames = 8 * chunkLength / bitsPerSample / channelCount;
      double duration = ((double) frames) / (double) sampleRate;
      
      Int32[][] data = new Int32[channelCount][];
      for (int channel = 0; channel < channelCount; ++channel) {
        data[channel] = new Int32[frames];
      }
      
      int readBits = 0, readBitCount = 0;
      for (int frame = 0; frame < frames; ++frame) {
        for (int channel = 0; channel < channelCount; ++channel) {
          while (readBitCount < bitsPerSample) {
            readBits |= Convert.ToInt32(reader.ReadByte()) << readBitCount;
            readBitCount += BitsPerByte;
          }
          
          int excessBitCount = readBitCount - bitsPerSample;
          data[channel][frame] = readBits >> excessBitCount;
          readBits %= 1 << excessBitCount;
          readBitCount = excessBitCount;
        }
      }

      return new SoundData(data, compressionCode, channelCount, sampleRate,
                           bytesPerSecond, bitsPerSample, blockAlign, frames,
                           duration);
    }
    
  }
}
