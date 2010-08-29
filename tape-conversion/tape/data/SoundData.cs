using System;
using System.IO;

namespace tape.data {

  /// <summary>
  /// Holds the sound data recorded from a cassette tape.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
	public class SoundData {
		
		public Int32[][] Data      { get; private set; }
		
		public int CompressionCode { get; private set; }
		public int ChannelCount    { get; private set; }
		public int SampleRate      { get; private set; }
		public int BytesPerSecond  { get; private set; }
		public int BitsPerSample   { get; private set; }
		public int BlockAlign      { get; private set; }
		public int Frames          { get; private set; }
		public double Duration     { get; private set; }
		
		public SoundData(Int32[][] data, int compressionCode, int channelCount,
                     int sampleRate, int bytesPerSecond, int bitsPerSample,
                     int blockAlign, int frames, double duration) {
      Data = data;
      CompressionCode = compressionCode;
      ChannelCount = channelCount;
      SampleRate = sampleRate;
      BytesPerSecond = bytesPerSecond;
      BitsPerSample = bitsPerSample;
      BlockAlign = blockAlign;
      Frames = frames;
      Duration = duration;
		}
		
	}
  
}
