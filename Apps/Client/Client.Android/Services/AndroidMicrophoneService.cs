using Android.Media;
using Client.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Android.Services
{
    public class AndroidMicrophoneService : IMicrophoneService
    {
        private AudioRecord? _recorder;
        private CancellationTokenSource? _cts;
        private bool _isRecording;

        public bool IsRecording => _isRecording;

        public async Task StartRecordingAsync(Action<Memory<byte>> onAudioChunk)
        {
            if (_isRecording) return;

            // Предполагается, что permission уже запрошен (в MainActivity или через Essentials)
            const int sampleRate = 48000;
            const ChannelIn channel = ChannelIn.Mono;
            const Encoding encoding = Encoding.Pcm16bit;

            int bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channel, encoding) * 2;

            _recorder = new AudioRecord(AudioSource.Mic, sampleRate, channel, encoding, bufferSize);

            _recorder.StartRecording();
            _isRecording = true;

            _cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                byte[] buffer = new byte[bufferSize];
                while (!_cts.Token.IsCancellationRequested)
                {
                    int read = _recorder.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        onAudioChunk(buffer.AsMemory(0, read));
                    }
                    await Task.Delay(5, _cts.Token); // небольшой анти-спам
                }
            }, _cts.Token);
        }

        public async Task StopRecordingAsync()
        {
            if (!_isRecording) return;

            _cts?.Cancel();
            _recorder?.Stop();
            _recorder?.Release();
            _recorder = null;
            _isRecording = false;

            await Task.CompletedTask;
        }
    }
}
