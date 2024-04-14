using OpenTK.Mathematics;

namespace _3D_FlightTracker_App.App;

public static class Controls
{
    public static class Zoom
    {
        // Variables to hold the current zoom level
        public static float CurrentZoomLevel = Settings.Camera.InitialZoomLevel;

        /// Update the zoom Matrix based on scroll input
        public static void UpdateZoom(float scrollDelta, ref Matrix4 view)
        {
            CurrentZoomLevel += scrollDelta;
            CurrentZoomLevel = Math.Clamp(CurrentZoomLevel, Settings.Controls.MinZoomLevel, Settings.Controls.MaxZoomLevel);

            // Calculate the sigmoid factor to adjust the zoom
            float sigmoid = Settings.Camera.MaxDistance / (1 + MathF.Exp(1/Settings.Controls.ZoomSigmoidFactor * (CurrentZoomLevel)));

            // Apply the zoom transformation
            view = Matrix4.LookAt(new Vector3(0,0, Settings.Camera.MinDistance + sigmoid), Vector3.Zero, Vector3.UnitY);
        }
    }

    public static class Rotation
    {
        /// Rotate a Matrix4 based on delta input
        public static void UpdateRotation(float deltaX, float deltaY, float deltaZ, ref Matrix4 model)
        {
            float rotationFactor = 1 / (1 + MathF.Exp(1.1f/Settings.Controls.ZoomSigmoidFactor * (Zoom.CurrentZoomLevel)));
            float rotationX = deltaX * rotationFactor;
            float rotationY = deltaY * rotationFactor;
            float rotationZ = deltaZ * rotationFactor;

            // Apply the rotation transformation
            model *= Matrix4.CreateRotationX(rotationX) * Matrix4.CreateRotationY(rotationY) * Matrix4.CreateRotationZ(rotationZ);
        }
    }
}