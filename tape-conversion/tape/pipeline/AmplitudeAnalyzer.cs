using System;
using tape.data;

namespace tapeconversion.tape.pipeline {

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
    /// <param name="data">The data to split up.</param>
    /// <returns>The resultant chunks.</returns>
    public SoundData[] SplitChunks(SoundData data) {
      return null;
    }

  }

}
