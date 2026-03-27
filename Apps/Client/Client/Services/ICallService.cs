using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MessengerAvalonia.Shared.CallsGrpc;

namespace Client.Services
{
    public interface ICallService
    {
        public Task StartCallAsync(string myLogin, string friendLogin);
        public Task EndCallAsync();
        public event Action<byte[]>? onAudioChunkReceived;
    }
}
