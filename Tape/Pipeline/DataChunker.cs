using System;
using System.Collections.Generic;
using Tape.Data;
using Tape.Data.Cassettes;
using Tape.Pipeline.Parsers;

namespace Tape.Pipeline {

  public class DataChunker {

    private readonly int IgnoreHead = 10000;

    public DataChunker() {}

    public DataChunker(int ignoreHead) {
      IgnoreHead = ignoreHead;
    }

    public CassetteData[] ChunkData(SoundData data) {
      IEnumerator<Int16> ie = data.GetEnumerator();
      float average = 0;
      int count = 0;

      ie.MoveNext();

      List<CassetteData> chunks = new List<CassetteData>();
      MetaData meta = null;

      do {
        Int16 level = ie.Current, abs = Math.Abs(level);
        int rate;

        // Eat a little first to even out the average, then start checking.
        if (count >= IgnoreHead) {
          if (level > 5 * average && (rate = FindLeader(ie)) > 0) {
            if (meta == null) {
              MetaParser parser = new MetaParser(ie, rate);
              meta = (MetaData) parser.Parse();
            } else {
              ProgramParser parser = new ProgramParser(ie, rate,
                                                       meta.ProgramSize);
              ProgramData program = (ProgramData) parser.Parse();
              chunks.Add(new CassetteData(meta, program));
              meta = null;
            }
            rate = count = 0;
          }
        }

        // Don't let data or spikes screw up our noise muffler.
        if (count < IgnoreHead || !(abs > 5 * average)) {
          average = (average * count + abs) / ++count;
        } else {
          count++;
        }

      } while (ie.MoveNext());

      CassetteData[] output = new CassetteData[chunks.Count];
      for (int i = 0; i < output.Length; ++i) {
        output[i] = chunks[i];
      }
      return output;
    }

    public bool IsLeader(IEnumerator<Int16> ie) {
      return FindLeader(ie) > 0;
    }

    private int FindLeader(IEnumerator<Int16> ie) {
      float averageRate = 0;
      int failureCount = 0;
      int successCount = 0;

      // This should actually be to 3600, but we'll be a bit paranoid about
      // running into the data. The parser can ignore the end of the leader.
      for (int i = 0; i < 3550; ++i) {
        int rate = NextRate(ie);
        if (rate < 1 && averageRate == 0) {
          return 0;
        }
        float range = 0;
        if (rate < 1 || averageRate > 0 &&
            (range = Math.Abs(averageRate - rate)) > Math.Max(rate / 5, 2)) {
          if (++failureCount > 99) {
            return 0;
          }
          successCount = 0;
          if (averageRate > rate && range > averageRate * 1.8) {
            // Be careful of overreading big sections.
            i += (int) Math.Max(range / averageRate, 1);
          }
        } else {
          // Leader fields don't need to be perfect. Every consecutive good 100
          // bits will restore our faith.
          if (++successCount > 99) {
            failureCount = 0;
          }
        }

        averageRate = (averageRate * i + rate) / (i + 1);
      }

      return (int) (averageRate + 1);
    }

    private int NextRate(IEnumerator<Int16> ie) {
      int upRate1 = Rate(ie, true),
          downRate1 = Rate(ie, false),
          upRate2 = Rate(ie, true),
          downRate2 = Rate(ie, false);

      if (upRate1 < 1 || downRate1 < 1 || upRate2 < 1 || downRate2 < 1) {
        return 0;
      }

      int biggest = Math.Max(Math.Max(upRate1, downRate1),
                             Math.Max(upRate2, downRate2)),
          smallest = Math.Min(Math.Min(upRate1, downRate1),
                              Math.Min(upRate2, downRate2));

      int range = biggest - smallest;

      if (range > Math.Max(smallest / 5, 2)) {
        return 0;
      }

      return (upRate1 + downRate1 + upRate2 + downRate2) / 4;
    }

    private int Rate(IEnumerator<Int16> ie, bool up) {
      int rate = 0;
      if (ie.Current == 0) {
        ie.MoveNext();
        return 0;
      }
      while (up ? ie.Current > 0 : ie.Current < 0) {
        rate += 1;
        if (!ie.MoveNext()) {
          return 0;
        }
      }
      return rate;
    }

  }

}
