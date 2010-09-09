using System;
using System.IO;
using tape.data;

namespace tape.io
{

    /// <summary>
    /// Reads Wave files and outputs a SoundData that holds the audio data.
    /// </summary>
    ///
    /// <author>Timothy Jones</author>
    public class DataReader
    {

        public static readonly int BITS_PER_BYTE = 8, MAX_BITS = 16;

        /// <summary>
        /// Reads a Wave file and outputs the audio data.
        /// </summary>
        ///
        /// <remarks>
        /// See https://ccrma.stanford.edu/courses/422/projects/WaveFormat/ for the
        /// structure of a wave file.
        /// </remarks>
        ///
        /// <param name="source">
        /// The location of the audio file to read.
        /// </param>
        /// <returns>
        /// The audio data as a <see cref="tape.data.SoundData"/> file.
        /// </returns>
        public SoundData ReadSoundFile(string source)
        {
            Stream stream = new FileStream(source, FileMode.Open);
            if (!stream.CanRead)
            {
                throw new IOException("File not found.");
            }

            BinaryReader reader = new BinaryReader(stream);

            // The "RIFF" chunk descriptor.
            ValidateFormat(reader, "RIFF");
            reader.ReadInt32();
            ValidateFormat(reader, "WAVE");

            // The "fmt" sub-chunk.
            ValidateFormat(reader, "fmt ");
            int chunkLength = reader.ReadInt32();

            int compressionCode = reader.ReadInt16(),
                channelCount = reader.ReadInt16(),
                sampleRate = reader.ReadInt32(),
                bytesPerSecond = reader.ReadInt32(),
                blockAlign = reader.ReadInt16(),
                bitsPerSample = reader.ReadInt16();

            if (channelCount > 1)
            {
                throw new IOException("Unexpected audio format. Expected 1 channel," +
                                      "got " + channelCount + ".");
            }

            if (MAX_BITS % bitsPerSample != 0)
            {
                throw new IOException("The input stream uses an unhandled " +
                                    "significant bits per sample parameter.");
            }

            // The "data" sub-chunk
            string name = "";
            while (name != "data")
            {
                name = new string(reader.ReadChars(4));
            }

            chunkLength = reader.ReadInt32();
            int frames = 8 * chunkLength / bitsPerSample / channelCount;
            double duration = ((double)frames) / (double)sampleRate;

            Int16[] data = new Int16[frames];

            for (int i = 0; i < frames; ++i)
            {
                data[i] = reader.ReadInt16();
            }

            reader.Close();
            stream.Close();
            stream.Dispose();

            return new SoundData(data, compressionCode, sampleRate, bytesPerSecond,
                                 bitsPerSample, blockAlign, duration);
        }

        private void ValidateFormat(BinaryReader reader, string expected)
        {
            string actual = new string(reader.ReadChars(4));
            if (expected != actual)
            {
                throw new IOException("Unexpected audio format. Expected '" + expected
                                      + "', got '" + actual + "'.");
            }
        }

    }

}
