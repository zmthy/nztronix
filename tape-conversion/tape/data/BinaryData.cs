using System;
using System.Collections.Generic;
using System.Collections;

namespace tape.data
{

    public class BinaryData : IEnumerable<Int16>
    {
        private readonly Int16[] Data;
        public readonly Int16 One = 200;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<Int16> GetEnumerator()
        {
            return new DataEnumerator(Data);
        }

        public class DataEnumerator : IEnumerator<Int16>
        {

            private readonly Int16[] Data;
            private int Position = -1;
            private bool Disposed = false;

            public DataEnumerator(Int16[] data)
            {
                Data = data;
                if (data != null)
                {
                    Position = 0;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Int16 Current
            {
                get
                {
                    return Data[Position];
                }
            }

            public bool MoveNext()
            {
                if (Position < Data.Length - 1)
                {
                    Position += 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                if (!Disposed)
                {
                    Position = 0;
                }
            }

            public void Dispose()
            {
                Position = Data.Length;
                Disposed = true;
            }

        }
    }

}
