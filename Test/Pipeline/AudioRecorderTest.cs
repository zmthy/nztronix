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
      AudioRecorder input = new AudioRecorder();
      TapeConverter converter = new TapeConverter();
      foreach (string s in converter.GetAudioInputDeviceNames()) {
        if (s.Contains("Microphone")) {
          input.Record(converter.GetAudioInputDevice(s));
          System.Threading.Thread.Sleep(5000);
          SoundData audio = input.Stop();
          AudioWriter writer = new AudioWriter();
          writer.WriteSoundData(audio, "../../../data/recorded.wav");
          return;
        }
      }
      Assert.Fail("No input device found.");
    }

  }

}
