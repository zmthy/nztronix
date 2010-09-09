using System;
using NUnit.Framework;
using tape;
using tape.pipeline;

namespace test.pipeline {

  [TestFixture]
  class AudioInputTest {

    [Test]
    public void TestInput() {
      AudioRecorder input = new AudioRecorder();
      TapeConverter converter = new TapeConverter();
      foreach (string s in converter.GetAudioInputDeviceNames()) {
        if (s.Contains("Microphone")) {
          input.Record(converter.GetAudioInputDevice(s));
        }
      }
    }

  }

}
