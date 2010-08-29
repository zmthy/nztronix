using System;
using tape.data;

namespace tape.pipeline {
  
  /// <summary>
  /// A class for processing prerecorded sound data into different formats.
  /// It includes utilities for both cleaning audio and converting it into a
  /// binary square wave.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class SignalProcessor {
    
    /// <summary>
    /// Removes ambient noise that has entered a digital audio file due to the
    /// recording process, thereby giving a more pure form of the original
    /// data on the cassette.
    /// </summary>
    /// 
    /// <param name="data">
    /// The data to clean.
    /// </param>
    /// <returns>
    /// The cleaned data.
    /// </returns>
    public SoundData RemoveNoise(SoundData data) {
      return null;
    }
    
    /// <summary>
    /// Converts audio data into a binary format by squaring off the data.
    /// </summary>
    /// 
    /// <param name="data">
    /// The data to convert.
    /// </param>
    /// <returns>
    /// The converted data.
    /// </returns>
    public BinaryData ConvertToSquare(SoundData data) {
      return null;
    }
    
  }
  
}
