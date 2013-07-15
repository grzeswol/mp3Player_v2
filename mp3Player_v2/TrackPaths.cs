using System.Collections.Generic;
using System.IO;

namespace mp3Player_v2
{
    class TrackPaths
    {
        private readonly List<string> _trackListPaths = new List<string>();
        private string _path;


        public string TracklistPath
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                CreateTrackListPaths();
            }
        }

        public List<string> GetTrackListPaths
        {
            get
            {
                return _trackListPaths;
            }
        }

        private void CreateTrackListPaths()
        {
            _trackListPaths.Clear();
            _trackListPaths.AddRange(Directory.EnumerateFiles(_path, "*.mp3", SearchOption.AllDirectories));
        }

        
    }
}
