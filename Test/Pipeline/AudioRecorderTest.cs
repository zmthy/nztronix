using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectSound;
using NUnit.Framework;
using Tape;
using Tape.Data;
using Tape.IO;
using Tape.Pipeline;

namespace Test.Pipeline {

  [TestFixture]
  public class AudioRecorderTest {

    [Test]
    public void TestInput() {
      TapeConverter converter = new TapeConverter();
      foreach (string s in converter.GetAudioInputDeviceNames()) {
        if (s.Contains("Microphone")) {
          converter.Record(s);
          System.Threading.Thread.Sleep(5000);
          converter.Stop("../../../data", false);
          return;
        }
      }
      Assert.Fail("No input device found.");
    }

  }

}
