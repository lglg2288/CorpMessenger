using Android.Content;
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
        private readonly AudioManager _audioManager;

        private const int SampleRate = 48000;

        public AndroidAudioPlayerService(Context context)
        {
            _audioManager = (AudioManager)context.GetSystemService(Context.AudioService)!;
        }

        public void Start()
        {
            _audioManager.Mode = Mode.InCommunication;
            _audioManager.SpeakerphoneOn = false;

            int bufferSize = AudioTrack.GetMinBufferSize(
                SampleRate,
                ChannelOut.Mono,
                Encoding.Pcm16bit);

            _audioTrack = new AudioTrack(
                Stream.VoiceCall,
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

            _audioManager.SpeakerphoneOn = false;
            _audioManager.Mode = Mode.Normal;
        }
    }
}
