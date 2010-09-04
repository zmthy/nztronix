using System;
using tape.data;
using tape.io;
using tape.pipeline;

namespace tape {

  /// <summary>
  /// An overlord class for the tape conversion process.
  /// </summary>
  /// 
  /// <author>Timothy Jones</author>
  public class TapeConverter {
  	
    private DataReader reader = new DataReader();
    private DataWriter writer = new DataWriter();
    
    private AudioInput input = new AudioInput();
    private SignalProcessor processor = new SignalProcessor();
    private ArchiveImageGenerator generator = new ArchiveImageGenerator();
    
    /// <summary>
    /// Runs the tape conversion pipeline, starting from a manual recording
    /// entry and ending with a saved game image.
    /// </summary>
    /// 
    /// <param name="destination">
    /// The location where the converted image should be outputted to.
    /// </param>
    public void RunPipeline(string destination) {
      SoundData master = input.Record();
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
      ImageData image   = generator.CreateImage(binary);
      writer.WriteArchiveImage(image, "");
    }
  	
    // Supplied solely to satisfy the builder in the absence of the rest of the
    // project code. To be removed during project-wide integration.
    public static void Main() {}
  	
  }
  
}
