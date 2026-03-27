using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public interface IMicrophoneService
    {
        Task StartRecordingAsync(Action<Memory<byte>> onData);
        Task StopRecordingAsync();
        bool IsRecording { get; }
    }
}
