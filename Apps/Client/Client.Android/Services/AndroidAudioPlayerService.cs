using Android.Media;
using Client.Services;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Android.Services
{
    public class AndroidAudioPlayerService : IAudioPlayerService
    {
        private AudioTrack? _audioTrack;
        private readonly ConcurrentQueue<byte[]> _queue = new();
        private CancellationTokenSource? _cts;

        private const int SampleRate = 48000;

        public void Start()
        {
            int bufferSize = AudioTrack.GetMinBufferSize(
                SampleRate,
                ChannelOut.Mono,
                Encoding.Pcm16bit);

            _audioTrack = new AudioTrack(
                Stream.Music,
                SampleRate,
                ChannelOut.Mono,
                Encoding.Pcm16bit,
                bufferSize * 4,
                AudioTrackMode.Stream);

            _audioTrack.Play();

            _cts = new CancellationTokenSource();

            Task.Run(() => PlaybackLoop(_cts.Token));
        }

        public void PlayChunk(byte[] data)
        {
            _queue.Enqueue(data);
        }

        private void PlaybackLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var chunk))
                {
                    _audioTrack?.Write(chunk, 0, chunk.Length);
                }
                else
                {
                    Thread.Sleep(2); // небольшая пауза если буфер пуст
                }
            }
        }

        public void Stop()
        {
            _cts?.Cancel();

            _audioTrack?.Stop();
            _audioTrack?.Release();
            _audioTrack = null;
        }
    }
}
