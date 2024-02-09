using Godot;

public partial class VoiceGrabber: AudioStreamPlayer
{
    public override void _Ready()
    {
        Stream = new AudioStreamMicrophone();
        Bus = "Capture";
        Play();
    }
}