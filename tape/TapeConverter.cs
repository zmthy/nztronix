using System;
using Microsoft.DirectX.DirectSound;

namespace tape {
  
  public class TapeConverter {

    private CaptureDevicesCollection devices;

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

    public static void Main() {}

  }

}
