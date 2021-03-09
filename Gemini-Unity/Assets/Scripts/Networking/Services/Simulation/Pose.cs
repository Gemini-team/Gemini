namespace Gemini.Networking.Services
{
    public class Pose
    {
        private float _north = 0.0f;
        private float _east = 0.0f;
        private float _heading = 0.0f;

        public float North
        {
            get => _north;
            set => _north = value;
                    
        }

        public float East
        {
            get => _east;
            set => _east = value;
                    
        }

        public float Heading
        {
            get => _heading;
            set => _heading = value;
                    
        }

        public Pose(float north, float east, float heading)
        {
            this._north = north;
            this._east = east;
            this._heading = heading;
        }
    }
}
