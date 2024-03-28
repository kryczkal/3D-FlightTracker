namespace _3D_FlightTracker_App;

static class Program
{
    public static void Main()
    {
        using (FlightTrackerApp app = new FlightTrackerApp(1280, 720))
        {
            Console.WriteLine("Starting 3D Flight Tracker...");
            app.Run();
            Console.WriteLine("3D Flight Tracker closed.");
        }
    }
}
