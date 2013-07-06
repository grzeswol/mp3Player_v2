using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Linq;


namespace mp3Player_v2
{
    public partial class Form1 : Form
    {
        private const string CurrentPlaylist = "current.playlist";

        private readonly MusicPlayer _mp = new MusicPlayer();
        private readonly TrackPaths _trackPaths = new TrackPaths();

        private Form2 _form2;
        private int _sortColumn = -1;
        private List<Track> _playList = new List<Track>();
        private int _focusedTrackIndex;


        public Form1()
        {
            InitializeComponent();

            if (!LoadPlaylist(CurrentPlaylist))
            {
                play.Enabled = false;
                stop.Enabled = false;
            }
        }

        private bool LoadPlaylist(string playlistName)
        {
            _playList.Clear();
            try
            {
                using (StreamReader stream = new StreamReader(playlistName))
                {
                    
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        _playList.Add(new Track(line));
                    }
                }
                UpdateListView();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        private void AddTrackToListView(Track trackToAdd)
        {
           ListViewItem lv = new ListViewItem();
           lv.Text = trackToAdd.Artist;
           lv.SubItems.Add(trackToAdd.Title);
           lv.SubItems.Add(trackToAdd.Album);
           lv.SubItems.Add(trackToAdd.Length);
           lv.SubItems.Add(trackToAdd.Year.ToString(CultureInfo.InvariantCulture));
           listView1.Items.Add(lv);
        }

        private void AddTrackToListView(Track trackToAdd, int index)
        {
            listView1.Items.RemoveAt(index);

            ListViewItem lv = new ListViewItem();
            lv.Text = trackToAdd.Artist;
            lv.SubItems.Add(trackToAdd.Title);
            lv.SubItems.Add(trackToAdd.Album);
            lv.SubItems.Add(trackToAdd.Length);
            lv.SubItems.Add(trackToAdd.Year.ToString(CultureInfo.InvariantCulture));
            listView1.Items.Insert(index, lv);
        }

        private void AddTrackToPlaylistAt(Track track, int index)
        {
            _playList.RemoveAt(index);
            _playList.Insert(index, track);
        }

        private void SortListAscByParam(SortingParams sortingParams)
        {
            switch (sortingParams)
            {
                    case SortingParams.Album:
                    _playList = _playList.OrderBy(o => o.Album).ToList();
                    break;

                    case SortingParams.Artist:
                    _playList = _playList.OrderBy(o => o.Artist).ToList();
                    break;

                    case SortingParams.Title:
                    _playList = _playList.OrderBy(o => o.Title).ToList();
                    break;

                    case SortingParams.Length:
                    _playList = _playList.OrderBy(o => o.Length).ToList();
                    break;

                    case SortingParams.Year:
                    _playList = _playList.OrderBy(o => o.Year).ToList();
                    break;
            }

            UpdateListView();
            
        }

        private void SortListDescByParam(SortingParams sortingParams)
        {
            switch (sortingParams)
            {
                case SortingParams.Album:
                    _playList = _playList.OrderByDescending(o => o.Album).ToList();
                    break;

                case SortingParams.Artist:
                    _playList = _playList.OrderByDescending(o => o.Artist).ToList();
                    break;

                case SortingParams.Title:
                    _playList = _playList.OrderByDescending(o => o.Title).ToList();
                    break;

                case SortingParams.Length:
                    _playList = _playList.OrderByDescending(o => o.Length).ToList();
                    break;

                case SortingParams.Year:
                    _playList = _playList.OrderByDescending(o => o.Year).ToList();
                    break;
            }
            UpdateListView();

        }

        private void UpdateListView()
        {
            listView1.Items.Clear();

            foreach (Track track in _playList)
            {
                AddTrackToListView(track);
            }
        }

        /* private void SQLTest()
        {
            string str = @"Data Source=Library.sdf";
            SqlCeConnection connection = new SqlCeConnection();
            connection.ConnectionString = str;
            try
            {
                connection.Open();
                MessageBox.Show("Opened");
            }
            catch (Exception)
            {
                MessageBox.Show("Problem");
                throw;
            }
            

        }

        */

        private void AddFileToPlaylistAndListView()
        {
            openFileDialog1.Filter = @"mp3 files (*.mp3)|*.mp3";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Track tempTrack = new Track(openFileDialog1.FileName);
                _playList.Add(tempTrack);
                AddTrackToListView(tempTrack);
                EnableButtons();
            }
        }

        private void EnableButtons()
        {
            play.Enabled = true;
            stop.Enabled = true;
        }

        private void AddFolderToPlaylistAndListView()
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            _trackPaths.TracklistPath = folderBrowserDialog1.SelectedPath;
            List<string> tempPaths = _trackPaths.GetTrackListPaths;

            foreach (string s in tempPaths)
            {
                Track tempTrack = new Track(s);
                _playList.Add(tempTrack);
                AddTrackToListView(tempTrack);
            }

            EnableButtons();
        }

        private void play_Click(object sender, EventArgs e)
        {
            PlaySong();
        }

        private void PlaySong()
        {
            try
            {
                _mp.Stop();
                _focusedTrackIndex = listView1.FocusedItem.Index;
                _mp.Open(_playList[_focusedTrackIndex].GetTrackPath());
                _mp.Play();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(@"Please select a song to play");
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            _mp.Stop();
        }

        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            _mp.MasterVolume = volumeTrackBar.Value * 100;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            PlaySong();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortingParams sortingParams = SortingParams.Artist;
            int columnIndex = e.Column;

            switch (columnIndex)
            {
                    case 0:
                    sortingParams = SortingParams.Artist;
                    break;

                    case 1:
                    sortingParams = SortingParams.Title;
                    break;

                    case 2:
                    sortingParams = SortingParams.Album;
                    break;

                    case 3:
                    sortingParams = SortingParams.Length;
                    break;

                    case 4:
                    sortingParams = SortingParams.Year;
                    break;
            }

            if (_sortColumn == -1)
            {
                SortListAscByParam(sortingParams);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(sortingParams);
                _sortColumn = -1;
            }
        }




        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            bool match = false;

            if (e.Button == MouseButtons.Right)
            {
            
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Bounds.Contains(new Point(e.X, e.Y)))
                {
                    MenuItem[] mi = new[] {new MenuItem("Edit tags", OnClickEditTags), new MenuItem("Remove", OnClickRemove) };
                    listView1.ContextMenu = new ContextMenu(mi);
                    match = true;
                    break;
                }
            }
            if (match)
            {
                listView1.ContextMenu.Show(listView1, new Point(e.X, e.Y));
            }
            }
    }

        private void OnClickRemove(object sender, EventArgs e)
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            RemoveFromPlaylistAt(_focusedTrackIndex);
            RemoveFromListViewAt(_focusedTrackIndex);
        }

        private void RemoveFromListViewAt(int index)
        {
            listView1.Items.RemoveAt(index);
        }

        private void RemoveFromPlaylistAt(int index)
        {
            _playList.RemoveAt(index);
        }

        private void OnClickEditTags(object sender, EventArgs eventArgs)
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            _form2 = new Form2(_playList[_focusedTrackIndex]);
            _form2.Show();
            _form2.Closing += form2_Closing;
        }

        void form2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            AddTrackToPlaylistAt(_form2.Track, _focusedTrackIndex);
            AddTrackToListView(_form2.Track,_focusedTrackIndex);
        }

        private void SavePlaylist(string playlistName)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(playlistName))
                {
                    foreach (Track track in _playList)
                    {
                        stream.WriteLine(track.GetTrackPath());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Problem " + ex.Message);
                throw;
            }
            
            
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            SavePlaylist(CurrentPlaylist);
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFileToPlaylistAndListView();
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFolderToPlaylistAndListView();
        }

        private void savePlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = @"playlist files |*.playlist";
            saveFileDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                SavePlaylist(saveFileDialog1.FileName);
            }
            else
            {
                MessageBox.Show(@"Please specify name for your playlist");
            }
        }

        private void loadPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = @"playlist files |*.playlist";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                listView1.Items.Clear();
                LoadPlaylist(openFileDialog1.FileName);
            }
        }
    }

    enum SortingParams
    {
        Artist,
        Title,
        Album,
        Length,
        Year,
    }
}
