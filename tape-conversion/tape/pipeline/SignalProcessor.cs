using System;
using tape.data;
using System.Collections;

namespace tape.pipeline
{

    /// <summary>
    /// A class for processing prerecorded sound data into different formats.
    /// It includes utilities for both cleaning audio and converting it into a
    /// binary square wave.
    /// </summary>
    /// 
    /// <author>Timothy Jones</author>
    public class SignalProcessor
    {
        //KeyCode 26H = Program, 16H = TextFile

        //So far feilds only setup for program reading
        //3600 bits of "1"
        Int16[] LeaderFeild = new Int16[3600]; //3 seconds

        //1 byte long
        Int16[] KeyCode = new Int16[8];

        //16 bytes long (Shorter names followed by spaces)
        Int16[] FileName = new Int16[128];

        //2 bytes
        Int16[] ProgramLength = new Int16[16];

        //2 bytes long
        Int16[] StartAddy = new Int16[16];

        //1 bytes long
        Int16[] Parity = new Int16[8];

        //2 bytes long
        Int16[] DummyData = new Int16[16];

        //3600 bits of "1"
        Int16[] LeaderField2 = new Int16[3600];

        //27H = Program, 17H = Text
        //1 byte long
        Int16[] KeyCode2 = new Int16[8];

        //Length of program length
        Int16[] Program;

        //1 byte long
        Int16[] Parity2 = new Int16[8];

        //2 bytes long
        Int16[] DummyData2 = new Int16[16];

        //Keep track of where we are up to in the data
        IEnumerator BinData = null;

        /// <summary>
        /// Converts audio data into a binary format by squaring off the data.
        /// </summary>
        /// 
        /// <param name="data">
        /// The data to convert.
        /// </param>
        /// <returns>
        /// The converted data.
        /// </returns>
        //"1" = 2 cycles of 2400hz
        //"0" = 1 cycle of 1200hz
        public BinaryData ConvertToSquare(SoundData data)
        {
            return null;
        }

        //Returns the type of program
        public String SectionData(BinaryData data)
        {
            BinData = FindDataStart(data);
            GetSection(LeaderFeild);
            GetSection(KeyCode);


        }

        private String GetType()
        {
        }

        private IEnumerator FindDataStart(BinaryData data)
        {
            IEnumerator BinData = data.GetEnumerator();
            while ((Int16)BinData.Current != data.One)
            {
                BinData.MoveNext();
            }

            return BinData;
        }

        private void GetSection(Int16[] array)
        {
            if (BinData == null)
            {
                return;
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (Int16)BinData.Current;
                BinData.MoveNext();
            }
        }

    }

}
