using System;
using Microsoft.DirectX.DirectSound;
using Tape.Data;
using Tape.Data.Cassettes;
using Tape.IO;
using Tape.Pipeline;

namespace Tape {
  
  public class TapeConverter {

    private CaptureDevicesCollection devices;
    private AudioRecorder recorder = new AudioRecorder();
    private DataChunker chunker = new DataChunker();

    /// <summary>
    /// Obtains a list of the audio input devices attached to the computer.
    /// </summary>
    /// 
    /// <remarks>
    /// Note that the collection is reobtained here everytime, on the quite
    /// possible event that, when prompted with a list of devices, a user plugs
    /// in a new device, and then reloads the list.
    /// </remarks>
    /// 
    /// <returns>The input devices found.</returns>
    public string[] GetAudioInputDeviceNames() {
      devices = new CaptureDevicesCollection();
      string[] names = new string[devices.Count];
      for (int i = 0; i < devices.Count; ++i) {
        names[i] = devices[i].Description;
      }
      return names;
    }

    public void Record(string device) {
      Capture capture = GetAudioInputDevice(device);
      if (capture == null) {
        throw new ArgumentException("Invalid input device specified.");
      }
      recorder.Record(capture);
    }

    public void Stop(string outputDir, bool process) {
      SoundData data = recorder.Stop();
      if (process) {
        Process(data, outputDir);
      }
    }

    public void Cancel() {
      recorder.Stop();
    }

    public void Process(string master, string outputDir) {
      AudioReader reader = new AudioReader();
      SoundData data = reader.ReadSoundFile(master);
      Process(data, outputDir);
    }

    private void Process(SoundData data, string outputDir) {
      AudioWriter writer = new AudioWriter();
      string dir = NormalizeDirectory(outputDir);
      writer.WriteSoundData(data, dir + "master.wav");
      CassetteData[] cassettes = chunker.ChunkData(data);
      string[] names = new string[cassettes.Length];
      for (int i = 0; i < names.Length; ++i) {
        string name = cassettes[0].Meta.FileName;
        int adjust = 0;
        for (int j = 0; j < i; ++j) {
          if (names[j] == name) {
            if (adjust == 0) {
              name += "1";
              adjust = 1;
            } else {
              name = name.Substring(0, name.Length - 1) + (++adjust);
            }
          }
        }
        names[i] = name;
      }
      for (int i = 0; i < names.Length; ++i) {
        writer.WriteCassetteData(cassettes[i], dir + names[i] + ".wav");
      }
    }

    /// <summary>
    /// Gets the audio input device with the given name.
    /// </summary>
    /// 
    /// <param name="name">The name of the device.</param>
    /// <returns>The named device.</returns>
    private Capture GetAudioInputDevice(string name) {
      if (devices == null) {
        devices = new CaptureDevicesCollection();
      }
      for (int i = 0; i < devices.Count; ++i) {
        if (devices[i].Description == name) {
          return new Capture(devices[i].DriverGuid);
        }
      }

      throw new ArgumentException("No input device with that name was found.");
    }

    private string NormalizeDirectory(string dir) {
      dir = dir.Replace('\\', '/');
      if (!dir.EndsWith("/")) {
        dir += "/";
      }
      return dir;
    }

  }

}
