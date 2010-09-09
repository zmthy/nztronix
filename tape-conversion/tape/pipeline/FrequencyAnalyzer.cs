using System;
using System.Collections.Generic;
using System.IO;
using tape.data;

namespace tape.pipeline {

  /// <summary>
  /// A class for processing prerecorded sound data into different formats.
  /// It includes utilities for both cleaning audio and converting it into a
  /// binary square wave.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  /// <author>Casey Orr</author>
  public class FrequencyAnalyzer {

    /// <summary>
    /// Converts audio data into a binary format by squaring off the data.
    /// </summary>
    /// 
    /// <param name="data">The data to convert.</param>
    /// <returns>The converted data.</returns>
    public BinaryData ConvertToSquare(SoundData data) {
      Int16[] sample = new Int16[4];
      List<bool> binData = new List<bool>();

      IEnumerator<Int16> audio = data.GetEnumerator();

      bool end = false;
      while (!end) {
        for (int i = 0; i < 4; ++i) {
          if (audio.MoveNext()) {
            sample[i] = audio.Current;
          } else {
            end = true;
            break;
          }
        }

        // Convert the numbers into big and small values on a wave.
        bool[] sizes = GetRelativeSizes(sample);

        // Fix any oversampling that may have occurred.
        int index = NormalizeSample(sizes);
        if (index > -1) {
          for (int i = index; i < 3; ++i) {
            sample[i] = sample[i + 1];
          }
          audio.MoveNext();
          sample[3] = audio.Current;

          // Redo the relative calculation, to ensure the reading is correct.
          sizes = GetRelativeSizes(sample);

          // We're not fixing it this time, just checking everything is good.
          if (NormalizeSample(sizes) > -1) {
            // The attempt to compensate hasn't worked.
            throw new IOException("Incorrect sample rate, or corrupted data.");
          }
        }

        // After all that, all we really need is the third value.
        binData.Add(sizes[2]);
      }

      return new BinaryData(binData);
    }

    /// <summary>
    /// Ensures that oversampling doesn't lead to incorrect readings.
    /// </summary>
    /// 
    /// <remarks>
    /// Chances are that the recording is a little oversamples, meaning we're
    /// reading slightly too often. This means that the data points are going
    /// to start slipping around, and we need to prevent this. This method uses
    /// non-absolutes (because the sound be recorded at any volume) to work
    /// out where the readings are slipping to and corrects for these problems.
    /// </remarks>
    /// 
    /// <param name="sample">The 4-part sample to normalise.</param>
    /// <param name="audio">The rest of the audio samples.</param>
    /// <returns>From which index to push up in the sample.</returns>
    private int NormalizeSample(bool[] sample) {
      // Using `B` to represent a big value, and `S` to represent a small one.

      // The oversampling can cause a couple of problems here.

      // S???
      if (!sample[0]) {
        return 0;
      }

      // BBSB
      if (sample[1] && sample[3]) {
        return 1;
      }

      // BBBS or BSSB
      if (sample[1] == sample[2]) {
        return 2;
      }

      // This has to be last, as all of the other cases can screw it up.
      // BBSB or BSBB
      if (sample[3]) {
        return 3;
      }

      // We're ok!
      return -1;
    }

    /// <summary>
    /// Analyses a sample of integers to calculate their relationships to one
    /// another - whether they represent big or small values.
    /// </summary>
    /// 
    /// <param name="sample">The sample to get the relative sizes of.</param>
    /// <returns>The relative sizes of the sample.</returns>
    private bool[] GetRelativeSizes(Int16[] sample) {
      // Using `B` to represent a big value, and `S` to represent a small one.

      bool[] sizes = { false, false, false, false };

      // By the specification, 2 values should be S, and 2 B. Oversampling can
      // break this rule, which is what we're here to check for. We assume that
      // the oversampling isn't so great that we're getting 4 of one type.

      // By the above assumption, these two must be on different portions.
      // Compare the outer values (which must always be on different samples).
      bool outerLeft = sample[0] > sample[3];
      Int16 outerBig = outerLeft ? sample[0] : sample[3];
      Int16 outerSmall = outerLeft ? sample[3] : sample[0];

      // Compare the inner values (which can be the same sample).
      bool innerLeft = sample[1] > sample[2];
      Int16 innerBig = innerLeft ? sample[1] : sample[2];
      Int16 innerSmall = innerLeft ? sample[2] : sample[1];

      // We'll start by testing three in a row (the most we can have).
      if (ThreeInARow(outerLeft, sample[0], sample[3], sample[1], sample[2],
                      innerLeft)) {
        if (outerLeft) {
          // BBBS
          sizes[0] = sizes[1] = sizes[2] = true;
        } else {
          // SSSB
          // Special case - this can't ever happen!
          throw new IOException("Incorrect sample rate, or corrupted data.");
        }

        return sizes;
      } else if (ThreeInARow(!outerLeft, sample[3], sample[0], sample[1],
                             sample[2], innerLeft)) {
        if (!outerLeft) {
          // SBBB
          sizes[1] = sizes[2] = sizes[3] = true;
        } else {
          // BSSS
          // Another impossible scenario.
          throw new IOException("Incorrect sample rate, or corrupted data.");
        }

        return sizes;
      }

      // Same again, but not in a row this time.
      if (Math.Abs(sample[0] - sample[3]) < Math.Abs(sample[1] - sample[2])) {
        // We MIGHT have a match on the outside.
        if (outerSmall > innerSmall &&
            Math.Abs(outerBig - innerBig) < innerBig - innerSmall) {
          // BSBB or BBSB
          sizes[0] = sizes[3] = sizes[innerLeft ? 1 : 2] = true;
        } else if (outerBig < innerBig &&
                   Math.Abs(outerSmall - innerSmall) < innerBig - innerSmall) {
          // SBSS or SSBS
          if (innerLeft) {
            sizes[1] = true;
          } else {
            // Another impossible scenario.
            throw new IOException("Incorrect sample rate, or corrupted data.");
          }
        }
      }

      // On to the twos - inners or outers matching here.
      if (innerSmall > outerBig) {
        // SBBS
        sizes[1] = sizes[2] = true;
      } else if (outerSmall > innerBig) {
        // BSSB
        sizes[0] = sizes[3] = true;
      }

      // And lastly, mixed values.
      if (outerLeft && innerLeft) {
        // BBSS
        sizes[0] = sizes[1] = true;
      } else if (!outerLeft && !innerLeft) {
        // SSBB
        // Another impossible scenario.
        throw new IOException("Incorrect sample rate, or corrupted data.");
      } else if (outerLeft) {
        // BSBS
        sizes[0] = sizes[2] = true;
      } else {
        // SBSB
        sizes[1] = sizes[3] = true;
      }

      return sizes;
    }

    /// <summary>
    /// Calculates whether a sample has three of the same values in a row.
    /// </summary>
    /// 
    /// <param name="big">Whether the check is the big value.</param>
    /// <param name="check">The outer value to check for a row.</param>
    /// <param name="other">The other outer value.</param>
    /// <param name="middle1">One of the middle values.</param>
    /// <param name="middle2">The other middle value.</param>
    /// <param name="checker">Which middle value to pick.</param>
    /// <returns>Whether three in a row of the same values is found.</returns>
    private bool ThreeInARow(bool big, Int16 check, Int16 other,
                             Int16 middle1, Int16 middle2, bool checker) {
      if (big) {
        return middle1 > other && middle2 > other &&
               Math.Abs(check - (checker ? middle1 : middle2)) < check - other;
      } else {
        return middle1 < other && middle2 < other &&
               Math.Abs(check - (checker ? middle2 : middle1)) < other - check;
      }
    }

  }

}
