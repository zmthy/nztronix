using System;
using System.IO;

namespace tape.data {

  /// <summary>
  /// Holds the sound data recorded from a cassette tape.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
	public class SoundData {
		
		private static readonly int BitsPerByte = 8, MaxBits = 16;
		
		public Int32[][] Data      { get; private set; }
		
		public int CompressionCode { get; private set; }
		public int ChannelCount    { get; private set; }
		public int SampleRate      { get; private set; }
		public int BytesPerSecond  { get; private set; }
		public int BitsPerSample   { get; private set; }
		public int BlockAlign      { get; private set; }
		public int Frames          { get; private set; }
		public double Duration     { get; private set; }
		
    /// <summary>
    /// Uses the given stream to read the sound data, partitioning it across
    /// the relevant fields for public access.
    /// </summary>
    ///
    /// <remarks>
    /// See https://ccrma.stanford.edu/courses/422/projects/WaveFormat/ for the
    /// structure of a wave file.
    /// </remarks>
    /// 
    /// <param name="stream">
    /// A stream which will return the contents of a WAV-format sound file.
    /// </param>
		public SoundData(Stream stream) {
			BinaryReader reader = new BinaryReader(stream);

      // The "RIFF" chunk descriptor.
			reader.ReadChars(4);
			reader.ReadInt32();

			if (new string(reader.ReadChars(4)) != "WAVE") {
        throw new Exception("Unexpected audio format.");
      }

      // The "fmt" sub-chunk.
			string chunkName = new string(reader.ReadChars(4));
			int chunkLength  = reader.ReadInt32();
			
			CompressionCode = reader.ReadInt16();
			ChannelCount    = reader.ReadInt16();
			SampleRate      = reader.ReadInt32();
			BytesPerSecond  = reader.ReadInt32();
			BlockAlign      = reader.ReadInt16();
			BitsPerSample   = reader.ReadInt16();

			if (MaxBits % BitsPerSample != 0) {
				throw new Exception("The input stream uses an unhandled " +
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
				throw new Exception("Error while reading the data.");
			}
			
			chunkLength = reader.ReadInt32();
			Frames = 8 * chunkLength / BitsPerSample / ChannelCount;
			Duration = ((double) Frames) / (double) SampleRate;
			
			Data = new Int32[ChannelCount][];
			for (int channel = 0; channel < ChannelCount; ++channel) {
				Data[channel] = new Int32[Frames];
			}
			
			int readBits = 0, readBitCount = 0;
			for (int frame = 0; frame < Frames; ++frame) {
				for (int channel = 0; channel < ChannelCount; ++channel) {
					while (readBitCount < BitsPerSample) {
						readBits |= Convert.ToInt32(reader.ReadByte()) << readBitCount;
						readBitCount += BitsPerByte;
					}
					
					int excessBitCount = readBitCount - BitsPerSample;
					Data[channel][frame] = readBits >> excessBitCount;
					readBits %= 1 << excessBitCount;
					readBitCount = excessBitCount;
				}
			}
		}
		
	}
  
}
