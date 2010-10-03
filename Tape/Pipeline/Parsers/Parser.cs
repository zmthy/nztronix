using System;
using System.Collections.Generic;
using Tape.Data;
using Tape.Data.Cassettes;

namespace Tape.Pipeline.Parsers {

  public abstract class Parser {

    private readonly int Rate;
    private bool LastPeak = false;
    private IEnumerator<Int16> Data;
    protected bool Verbose = false;
    protected bool Leading = true;

    public Parser(IEnumerator<Int16> ie, int rate) {
      Rate = rate;
      Data = ie;
    }

    public abstract DataPart Parse();

    protected void EatLeader() {
      bool peak = false;
      while (GetRelativeSize() == (peak = !peak));
      GetRelativeSize();
      GetRelativeSize();
    }

    protected bool NextBit() {
      bool[] sizes = new bool[4];

      for (int i = 0; i < 4; ++i) {
        sizes[i] = GetRelativeSize();
      }

      if (Normalize(sizes)) {
        if (Normalize(sizes)) {
          if (GetNormalizerIndex(sizes) > -1) {
            // The attempt to compensate hasn't worked.
            throw new Exception("Invalid bit read - " + GetSizes(sizes) + ".");
          }
        }
      }

      return sizes[2];
    }

    protected byte NextByte() {
      if (NextBit()) {
        throw new Exception("Invalid byte - missing start bit.");
      }
      return PostNextByte();
    }

    protected byte PostNextByte() {
      byte b = 0;
      for (int i = 0; i < 8; ++i) {
        if (NextBit()) {
          b += (byte) (1 << i);
        }
      }
      if (!NextBit() || !NextBit()) {
        throw new Exception("Invalid byte - missing stop bit.");
      }
      return b;
    }

    protected Int16 NextInt16() {
      Int16 num = (Int16) (NextByte() << 8);
      num = (Int16) (num | (Int16) NextByte());
      return num;
    }

    private string GetSizes(bool[] sizes) {
      string o = "";
      foreach (bool b in sizes) {
        o += b ? 'B' : 'S';
      }
      return o;
    }

    private bool GetRelativeSize() {
      int up = 0;
      int down = 0;
      while (true) {
        if (Data.Current > 0) {
          up += 1;
        } else if (Data.Current < 0) {
          down += 1;
        }

        if (up > Rate / 1.5 && down > Rate / 1.5) {
          throw new Exception("Ambiguous read - Up: " + up +
                                             ", Down: " + down + ".");
        }

        if (Verbose) {
         Console.WriteLine(up + " " + down + " " + Data.Current);
        }

        // We can be quite liberal with our acceptance rate here, as the read
        // rate will pull us back in line if we sample in a weird way.
        if (up > (Rate / 1.6)) {
          if (LastPeak) {
            for (int i = 0; i < Rate && Data.Current > 0; ++i) {
              Data.MoveNext();
            }
          } else {
            Data.MoveNext();
          }
          return LastPeak = true;
        }

        if (down > (Rate / 1.6)) {
          if (!LastPeak) {
            for (int i = 0; i < Rate && Data.Current < 0; ++i) {
              Data.MoveNext();
            }
          } else {
            Data.MoveNext();
          }
          return LastPeak = false;
        }

        if (!Data.MoveNext()) {
          throw new Exception("Premature end of data.");
        }
      }
    }

    private bool Normalize(bool[] sizes) {
      int index = GetNormalizerIndex(sizes);

      if (index > -1) {
        for (int i = index; i < 3; ++i) {
          sizes[i] = sizes[i + 1];
        }

        sizes[3] = GetRelativeSize();

        return true;
      }

      return false;
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
    private int GetNormalizerIndex(bool[] sample) {
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

  }

}
