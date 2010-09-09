using System;
using System.IO;
using tape.data;

namespace tape.io
{

    public class DataWriter
    {

        public void WriteSoundFile(SoundData data, string location)
        {
            Stream stream = new FileStream(location, FileMode.Create);
            if (!stream.CanWrite)
            {
                throw new IOException("File not created successfully.");
            }

            BinaryWriter writer = new BinaryWriter(stream);
            int size = data.Length * data.BitsPerSample / 8;

            // The "RIFF" chunk descriptor.
            writer.Write("RIFF".ToCharArray());
            writer.Write((Int32)(36 + size));
            writer.Write("WAVE".ToCharArray());

            // The "fmt" sub-chunk.
            writer.Write("fmt ".ToCharArray());
            writer.Write((Int32)16);
            writer.Write((Int16)data.CompressionCode);
            writer.Write((Int16)1);
            writer.Write((Int32)data.SampleRate);
            writer.Write((Int32)data.BytesPerSecond);
            writer.Write((Int16)data.BlockAlign);
            writer.Write((Int16)data.BitsPerSample);

            // The "data" sub-chunk
            writer.Write("data".ToCharArray());
            writer.Write((Int32)size);

            foreach (Int16 sample in data)
            {
                writer.Write(sample);
            }

            writer.Close();
            stream.Close();
            stream.Dispose();
        }

        public void WriteArchiveImage(ImageData data, string location)
        {

        }

    }

}
