namespace _3D_FlightTracker_App;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class FlightTrackerApp : GameWindow
{
    public FlightTrackerApp(int width, int height) : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            Size = (width, height),
            Title = "3D Flight Tracker"
        })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        // Initialization code goes here
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // Drawing code goes here

        // Swap the front and back buffers to present the new frame
        SwapBuffers();
    }
}
