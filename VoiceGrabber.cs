using Godot;
using System;

public partial class VoiceGrabber: AudioStreamPlayer
{
    [Export] private string _busName = "Record";
    [Export] private bool _isBusMute = true;
    public override void _Ready()
    {
        int id = AudioServer.BusCount;

        AudioServer.AddBus(id);
        AudioServer.SetBusName(id, _busName);
        AudioServer.AddBusEffect(id, new AudioEffectCapture());
        AudioServer.SetBusMute(id, _isBusMute);

        Bus = _busName;
        Stream = new AudioStreamMicrophone();

        Play();
    }
}