using BP.GameConsole;
using Godot;
using System.Collections.Generic;

public partial class VoiceManager : Node
{
    public static VoiceManager Instance { get; private set; }

    [ExportGroup("Voice Settings")]
    [Export] public bool recording = false;
    [Export] public bool listen = false;
    [Export(PropertyHint.Range, "0, 1, 0.001")] public float threshold = 0.005f;

    [ExportGroup("References")]
    [Export] public AudioStreamPlayer audioStreamPlayer;
    [Export] VoiceGrabber _voiceGrabber;

    private AudioEffectCapture _audioEffectCapture;
    private AudioStreamGeneratorPlayback _generatorPlayback;
    private List<float> _receivedAudioBuffer = new();
    private bool _canClearBuffer = false;
    public override void _EnterTree()
    {
        Instance = this;
    }
    public override void _Process(double delta)
    {
        if (_generatorPlayback != null)
            CaptureVoice();

        ListenToMicrophone();
    }
    private void GetMicrophone()
    {
        _audioEffectCapture = AudioServer.GetBusEffect(AudioServer.GetBusIndex(_voiceGrabber.Bus), 0) as AudioEffectCapture;
    }
    private void CreateVoice()
    {
        AudioStreamGenerator audioStreamGenerator = new AudioStreamGenerator();
        audioStreamGenerator.BufferLength = 0.1f;

        audioStreamPlayer.Stream = audioStreamGenerator;
        audioStreamPlayer.Play();

        _generatorPlayback = audioStreamPlayer.GetStreamPlayback() as AudioStreamGeneratorPlayback;
    }
    private void ListenToMicrophone()
    {
        if (!recording) return;

        if (_audioEffectCapture == null) GetMicrophone();
        if (!_canClearBuffer) _audioEffectCapture.ClearBuffer();

        int frames = _audioEffectCapture.GetFramesAvailable();
        Vector2[] stereoBuffer = _audioEffectCapture.GetBuffer(frames);

        if (stereoBuffer.Length > 0)
        {
            float[] data = new float[stereoBuffer.Length];
            float maxValue = 0.0f;
            for (int i = 0, k = 0; i < stereoBuffer.Length; i++, k++)
            {
                float value = (stereoBuffer[i].X + stereoBuffer[i].Y) / 2f;
                maxValue = Mathf.Max(value, maxValue);
                data[k] = value;
            }
            if (maxValue < threshold) return;
            if (listen) SynchronizeVoice(data, Multiplayer.GetUniqueId());

            GameConsole.Instance.DebugWarning($"data.Length: {data.Length}, maxThreshold: {maxValue}, frame: {frames}");

            Rpc(MethodName.SynchronizeVoice, data, Multiplayer.GetUniqueId());
        }
        _canClearBuffer = recording;
    }
    private void CaptureVoice()
    {
        if (_generatorPlayback.GetFramesAvailable() < 1) return;

        for (int i = 0; i < Mathf.Min(_generatorPlayback.GetFramesAvailable(), _receivedAudioBuffer.Count); i++)
        {
            _generatorPlayback.PushFrame(new Vector2(_receivedAudioBuffer[i], _receivedAudioBuffer[i]));
        }
        _receivedAudioBuffer.Clear();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 0, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    private void SynchronizeVoice(float[] data, int id)
    {
        if (_generatorPlayback == null) CreateVoice();

        _receivedAudioBuffer.AddRange(data);
    }
}