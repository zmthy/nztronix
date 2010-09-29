using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectSound;
using NUnit.Framework;
using tape;
using tape.data;
using tape.pipeline;

namespace test.pipeline {

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
          IEnumerator<Int16> data = audio.GetEnumerator();
          for (int i = 0; i < 10; ++i) {
            data.MoveNext();
            Console.WriteLine(data.Current);
          }
          return;
        }
      }
      Assert.Fail("No input device found.");
    }

  }

}
