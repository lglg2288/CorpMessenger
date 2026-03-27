using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public interface IAudioPlayerService
    {
        void Start();
        void PlayChunk(byte[] data);
        void Stop();
    }
}
