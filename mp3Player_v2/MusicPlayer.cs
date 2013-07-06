using System.Text;
using System.Runtime.InteropServices;

namespace mp3Player_v2
{
    class MusicPlayer
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString,
                                                    int uReturnLength, int hwndCallback);

        

        public void Open(string file)
        {
            string command = "open \"" + file + "\" type MPEGVideo alias MyMp3";
            mciSendString(command, null, 0, 0);
        }

        public void Play()
        {
            const string command = "play MyMp3";
            mciSendString(command, null, 0, 0);
        }

        public void Stop()
        {
            string command = "stop MyMp3";
            mciSendString(command, null, 0, 0);

            command = "close MyMp3";
            mciSendString(command, null, 0, 0);
        }

        public int MasterVolume { get { return 0; } 
            set { mciSendString(string.Concat("setaudio MyMp3 volume to ", value), null, 0, 0); } }
    }
}
