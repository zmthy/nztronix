using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tape.Data;
using Tape.IO;
using Tape.Pipeline;

namespace Test.Pipeline {

  [TestFixture]
  public class DataChunkerTest {

    [Test]
    public void TestLeader() {
      Int16[] leader = new Int16[36000];
      Random random = new Random();
      for (int i = 0; i < leader.Length;) {
        leader[i++] = 10000;
        leader[i++] = 9000;
        leader[i++] = (Int16) (random.Next(2) < 1 ? -8000 : 8000);
        leader[i++] = -12000;
        leader[i++] = -9000;
      }
      IEnumerator<Int16> ie = new List<Int16>(leader).GetEnumerator();
      ie.MoveNext();
      Assert.IsTrue(new DataChunker().IsLeader(ie));

      for (int i = 0; i < leader.Length;) {
        leader[i++] = 10000;
        leader[i++] = 9000;
        leader[i++] = 8000;
        leader[i++] = 12000;
        leader[i++] = 9000;
      }
      ie = new List<Int16>(leader).GetEnumerator();
      ie.MoveNext();
      Assert.IsFalse(new DataChunker().IsLeader(ie));
    }

  }

}
