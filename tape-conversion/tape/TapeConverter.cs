using System;
using Microsoft.DirectX.DirectSound;
using tape.data;
using tape.io;
using tape.pipeline;
using test.data;
using test.io;
using test.pipeline;

namespace tape {

  /// <summary>
  /// An overlord class for the tape conversion process.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class TapeConverter {

    private AudioReader reader = new AudioReader();
    private AudioWriter writer = new AudioWriter();

    private AudioRecorder input = new AudioRecorder();
    private FrequencyAnalyzer processor = new FrequencyAnalyzer();
    private WavGenerator generator = new WavGenerator();

    /// <summary>
    /// Runs the tape conversion pipeline, starting from a manual recording
    /// entry and ending with a saved game image.
    /// </summary>
    /// 
    /// <param name="destination">
    /// The location where the converted image should be outputted to.
    /// </param>
    public void RunPipeline(string destination) {
      SoundData master = input.Record(null);
      writer.WriteSoundFile(master, "");
      RunPipeline(master);
    }

    /// <summary>
    /// Runs the tape conversion pipeline, starting from a saved audio file and
    /// ending with a saved game image.
    /// </summary>
    /// 
    /// <param name="source">
    /// The location of the saved audio to convert.
    /// </param>
    /// <param name="destination">
    /// The location where the converted image should be outputted to.
    /// </param>
    public void RunPipeline(string source, string destination) {
      SoundData master = reader.ReadSoundFile(source);
      RunPipeline(master);
    }

    /// <summary>
    /// Runs the rest of the pipeline on the master recording.
    /// </summary>
    ///
    /// <param name="master">
    /// The master recording sound data.
    /// </param>
    private void RunPipeline(SoundData master) {
      BinaryData binary = processor.ConvertToSquare(master);
      // ImageData image = generator.CreateImage(binary);
      // writer.WriteArchiveImage(image, "");
    }

    private CaptureDevicesCollection devices = new CaptureDevicesCollection();

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

    /// <summary>
    /// Gets the audio input device with the given name.
    /// </summary>
    /// 
    /// <param name="name">The name of the device.</param>
    /// <returns>The named device.</returns>
    public Capture GetAudioInputDevice(string name) {
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

    // Supplied solely to satisfy the builder in the absence of the rest of the
    // project code. To be removed during project-wide integration.
    public static void Main() {
      try {
          /*
        new SoundDataTest().TestEnumerator();
        new BinaryDataTest().TestEnumerator();
        new AmplitudeAnalyzerTest().TestChunking();
        Console.WriteLine("Amplitude tested successfully.");
        FrequencyAnalyzerTest test = new FrequencyAnalyzerTest();
        test.TestGoodData();
        Console.WriteLine("Good data tested successfully.");
        test.TestBadData();
        Console.WriteLine("Bad data tested successfully.");
        test.TestLowQualityData();
        Console.WriteLine("LQ data tested successfully.");
          */

        FileWriterTest test = new FileWriterTest();
        test.TestWrite();

      } catch (Exception e) {
        Console.WriteLine(e);
      }
    }

  }

}
