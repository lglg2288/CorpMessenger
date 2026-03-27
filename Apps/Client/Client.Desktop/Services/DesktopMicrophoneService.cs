using Client.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Client.Desktop.Services
{
    public class DesktopMicrophoneService : IMicrophoneService
    {

        private WasapiCapture? _capture;
        private bool _isRecording;
        private string _status;
        public bool IsRecording
        {
            get
            {
                return _isRecording;
            }
        }

        public async Task StartRecordingAsync(Action<Memory<byte>> onAudioChunk)
        {
            if (_isRecording) return;

            try
            {
                _capture = new WasapiCapture();
                _capture.WaveFormat = new WaveFormat(48000, 16, 1); // 48kHz, 16-bit, mono

                _capture.DataAvailable += (s, e) =>
                {
                    if (e.BytesRecorded > 0)
                        onAudioChunk(e.Buffer.AsMemory(0, e.BytesRecorded));
                };


                _capture.RecordingStopped += (s, e) => _isRecording = false;

                _capture.StartRecording();
                _isRecording = true;
                _status = "Запись...";
            }
            catch (Exception ex)
            {
                _status = $"Error from StartRecording: {ex.Message}";
            }

            await Task.CompletedTask;
        }

        public async Task StopRecordingAsync()
        {
            if (!_isRecording)
                return;
            _capture?.StopRecording();
            _capture?.Dispose();
            _capture = null;
            _isRecording = false;
            _status = "Recording stopping";

            await Task.CompletedTask;
        }
    }
}
