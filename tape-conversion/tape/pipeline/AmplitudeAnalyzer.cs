using System;
using System.Collections.Generic;
using tape.data;

namespace tape.pipeline {

  /// <summary>
  /// Analyses sound data using amplitude as a guide. As a result, it can split
  /// up chunks of data in a sound file.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  class AmplitudeAnalyzer {

    /// <summary>
    /// Splits sound data into its loudest chunks, ignoring the ambient noise
    /// surrounding them.
    /// </summary>
    /// 
    /// <remarks>
    /// This method assumes that the data is a lot louder than the noise around
    /// it. The first part of every bit sample has to be a large value, so when
    /// the average on the right of where we're looking jumps radically, we
    /// know we've found something.
    /// </remarks>
    /// 
    /// <param name="data">The data to split up.</param>
    /// <returns>The resultant chunks.</returns>
    public SoundData[] SplitChunks(SoundData data) {
      List<SoundData> chunks = TrimNoise(data.GetEnumerator());

      SoundData[] output = new SoundData[chunks.Count];
      for (int i = 0; i < chunks.Count; ++i) {
        output[i] = chunks[i];
      }

      return output;
    }

    /// <summary>
    /// Trims noise off of the front of the given sound data.
    /// </summary>
    /// 
    /// <remarks>
    /// This is also the first method call in the pipeline, so calling this
    /// method will continue the pipeline, cutting out the desired chunks and
    /// returning all the way back with them.
    /// </remarks>
    /// 
    /// <param name="data">The data to trim.</param>
    private List<SoundData> TrimNoise(IEnumerator<Int16> ie) {
      List<SoundData> chunks = new List<SoundData>();
      float average = 0;
      int count = 0;

      while (true) {
        Int16 i = ie.Current;

        // Eat a little first to even out the average, then start checking.
        if (count >= 20) {
          if (i > 5 * average) {
            // Probably the start of some data! Let's just check a couple more
            // data points to make sure.
            break;
          }
        }

        // Don't let data or spikes screw up our noise muffler.
        if (!(i > 5 * average)) {
          average = (average * count + i) / ++count;
        }

        ie.MoveNext();
      }

      return chunks;
    }

    /// <summary>
    /// Rips the next chunk out of a trimmed sound file.
    /// </summary>
    /// 
    /// <remarks>
    /// This is the second part of the pipeline, which does not continue,
    /// as TrimNoise will ensure the next chunk is ripped.
    /// </remarks>
    /// 
    /// <param name="testData">The data used by the trimmer.</param>
    /// <param name="ie">The rest of the data.</param>
    private SoundData RipChunk(Int16[] testData, IEnumerator<Int16> ie) {
      return null;
    }

  }

}
