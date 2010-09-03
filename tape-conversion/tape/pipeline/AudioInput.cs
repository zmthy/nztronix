using System;
using tape.data;
using System.Collections;
using Microsoft.DirectX.DirectSound;
using System.Reflection;

using System.Threading;


namespace tape.pipeline
 {

    public class AudioInput
    {
        // botch – not sure if these and IsReady are thread safe for multiple threads
        public int _dwCaptureBufferSize, _dwOutputBufferSize, _dwNotifySize;
        public CaptureBuffer _dwCapBuffer;
       // public SecondaryBuffer[] _dwDevBuffers;
        public ArrayList sData;  
        public Thread _dwCaptureThread;
        public bool IsReady = false;

        public bool Record(string captureDescriptor, Control owner)
        {
            // string captureDescriptor – string for eg “Mic”, “Input”
            // Control owner – maybe Window or Form would do for this – was Native.GetDesktopWindow()
            // if windowless application use desktop window as message broker
            // Returns true for setup done and thread started, false for problem

            // Choose a Wave format, calculating BlockAlign and AverageBytesPerSecond
            ConstructorInfo nom = typeof(WaveFormat).GetConstructor(Type.EmptyTypes);
            WaveFormat format = (WaveFormat)nom.Invoke(null);
            format.SamplesPerSecond = 96000;
            format.BitsPerSample = 16;
            format.Channels = 1;
            format.FormatTag = WaveFormatTag.Pcm;

            sData = new ArrayList();

            // Both of these are calculate for All channels
            // BlockAlign = BytesPerSampleAllChannels, AverageBytesPerSecond = BytesPerSecondAllChannels
            format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlign;

            // Set the size of input and output buffers

            // Multiplier of both delay and minimum buffer size in units of 1/16th secs,
            int NUM_BUFFERS = 8;

            // Sets _dwNotifySize to enough bytes for 1/16th of a second, all channels
            // Note that this was 1/8th (ie line ended ‘/ 8);’), and output buffer size = capture size/2
            // But this was changed to allow output buffer size to be a multiple of BlockAlign
            _dwNotifySize = Math.Max(4096, format.AverageBytesPerSecond / (8 * 2));
            // rounds _dwNotifySize to a multiple of BlockAlign (BytesPerSampleAllChannel)
            _dwNotifySize -= _dwNotifySize % format.BlockAlign;

            // Capture buffer is looped – when the end is reached, it starts from the beginning again.
            // Capturing one should be twice as large as output – so that when completed capture
            // is being read to output buffer there is still room to for the buffer to keep filling
            // without overwriting the output. I think.
            _dwCaptureBufferSize = NUM_BUFFERS * _dwNotifySize * 2;
            _dwOutputBufferSize = NUM_BUFFERS * _dwNotifySize;

            // Create CaptureBufferDescriptor and actual capturing buffer
            // Enumerate all devices, choosing one containing the given string (captureDescriptor)
            var cap = default(Capture);
            var cdc = new CaptureDevicesCollection();
            for (int i = 0; i < cdc.Count; i++)
            {
                if (cdc[i].Description.ToLower().Contains(captureDescriptor.ToLower()))
                {
                    cap = new Capture(cdc[i].DriverGuid);
                    break;
                }
            }

            // Check a matching capture device was found
            if (cap == null)
                return false; // no matching sound card/capture device
            {

                // Make the description and create a CaptureBuffer accordingly
                ConstructorInfo capnom = typeof(CaptureBufferDescription).GetConstructor(Type.EmptyTypes);
                var capDesc = (CaptureBufferDescription)capnom.Invoke(null);

                capDesc.Format = format;
                capDesc.BufferBytes = _dwCaptureBufferSize;

                _dwCapBuffer = new CaptureBuffer(capDesc, cap);

                // Create output device and buffers

                // Uses default speakers to output – choose output device in same way as for capturing.
                var dev = new Device();
                // As DirectSound uses any window for a message pump we have to SetCooperativeLevel()
                dev.SetCooperativeLevel(owner, CooperativeLevel.Priority);

                // Set GlobalFocus=True if you want echo even if desktop window is not focused.
                var devDesc = new BufferDescription
                {
                    BufferBytes = _dwOutputBufferSize,
                    Format = format,
                    DeferLocation = true,
                    GlobalFocus = true
                };
                // Create two output buffers – this seems to avoid the buffer being locked and written
                // to while it's still playing, helping to avoid a sound glitch on my machine.
            //    _dwDevBuffers = new SecondaryBuffer[2];
            //    _dwDevBuffers[0] = new SecondaryBuffer(devDesc, dev);
             //   _dwDevBuffers[1] = new SecondaryBuffer(devDesc, dev);

                // Set autoResetEvent to be fired when it's filled and subscribe to buffer notifications

                var _resetEvent = new AutoResetEvent(false);
                var _notify = new Notify(_dwCapBuffer);
                // Half&half – one notification halfway through the output buffer, one at the end
                ConstructorInfo buffnom = typeof(BufferPositionNotify).GetConstructor(Type.EmptyTypes);
                var bpn1 = (BufferPositionNotify)buffnom.Invoke(null);
                bpn1.Offset = _dwCapBuffer.Caps.BufferBytes / 2 - 1;
                bpn1.EventNotifyHandle = _resetEvent.SafeWaitHandle.DangerousGetHandle();
                var bpn2 = (BufferPositionNotify)buffnom.Invoke(null);
                bpn2.Offset = _dwCapBuffer.Caps.BufferBytes - 1;
                bpn2.EventNotifyHandle = _resetEvent.SafeWaitHandle.DangerousGetHandle();

                _notify.SetNotificationPositions(new BufferPositionNotify[] { bpn1, bpn2 });

                IsReady = true; // ready to capture sound

                // Fire worker thread to take care of messages
                // Note that on a uniprocessor, the new thread may not get any processor time
                // until the main thread is preempted or yields, eg by ending button click event or
                // calling Thread.Sleep(0)

                // botch – not sure if these are thread safe for multiple threads
                int offset = 0;
                int devbuffer = 0;

                // Make a new thread – as countained in the { }
                _dwCaptureThread = new Thread((ThreadStart)delegate
                // *********************************************************************
                {
                    _dwCapBuffer.Start(true); // start capture

                    // IsReady – This should be true while you wish to capture and then output the sound.
                    while (IsReady)
                    {
                        _resetEvent.WaitOne(); // blocks thread until _dwCapBuffer is half/totally full
                        // Read the capture buffer into an array, and output it to the next DevBuffer
                        var read = _dwCapBuffer.Read(offset, typeof(byte), LockFlag.None, _dwOutputBufferSize);
                        sData.AddRange(read);
                     
                     //   _dwDevBuffers[devbuffer].Write(0, read, LockFlag.EntireBuffer);

                        // Update offset
                        offset = (offset + _dwOutputBufferSize) % _dwCaptureBufferSize;

                        // Play the sound
                        //    _dwDevBuffers[devbuffer].SetCurrentPosition(0);
                        //    _dwDevBuffers[devbuffer].Play(0, BufferPlayFlags.Default);

                        devbuffer = 1 - devbuffer; // toggle between 0 and 1
                    }
                    _dwCapBuffer.Stop(); // stop capture
                    // *********************************************************************
                });

                _dwCaptureThread.Start(); // start the new Thread

                return true;
            }



        }
    }
}

