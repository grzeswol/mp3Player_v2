﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Linq;


namespace mp3Player_v2
{
    public partial class MainForm : Form
    {
        private const string CurrentPlaylist = "current.playlist";

        private readonly MusicPlayer _mp = new MusicPlayer();
        private readonly TrackPaths _trackPaths = new TrackPaths();

        private EditTagsForm _editTagsForm;
        private int _sortColumn = -1;
        private List<Track> _playList = new List<Track>();
        private Track[] _copyList; 
        private int _focusedTrackIndex;
        private LibraryForm _libraryForm;


        public MainForm()
        {
            InitializeComponent();
            
            if (!LoadPlaylist(CurrentPlaylist))
            {
                EnableButtons(false);
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
                EnableButtons(true);
                return true;
            }
            catch (TagLib.UnsupportedFormatException)
            {
                MessageBox.Show(@"Damaged playlist file",@"Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
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

       private void AddFileToPlaylistAndListView()
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = @"mp3 files (*.mp3)|*.mp3";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Track tempTrack = new Track(openFileDialog1.FileName);
                _playList.Add(tempTrack);
                AddTrackToListView(tempTrack);
                EnableButtons(true);
            }
        }

        private void EnableButtons(bool enabled)
        {
            play.Enabled = enabled;
            stop.Enabled = enabled;
            playToolStripMenuItem.Enabled = enabled;
            playNextToolStripMenuItem.Enabled = enabled;
            playPreviousToolStripMenuItem.Enabled = enabled;
            stopToolStripMenuItem.Enabled = enabled;
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

            EnableButtons(true);
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

        private void PlayNextSong()
        {
            _focusedTrackIndex++;
            if (CheckListViewBoundaries())
            {
                listView1.SelectedItems.Clear();
                listView1.Items[_focusedTrackIndex].Selected = true;
                listView1.Items[_focusedTrackIndex].Focused = true;
                
                PlaySong();
            }
            else
            {
                _mp.Stop();
            }
        }

        private bool CheckListViewBoundaries()
        {
            if (_focusedTrackIndex > listView1.Items.Count -1 || _focusedTrackIndex < 0)
            {
                return false;
            }
            return true;
        }

        private void PlayPreviousSong()
        {
            _focusedTrackIndex--;
            if (CheckListViewBoundaries())
            {
                listView1.SelectedItems.Clear();
                listView1.Items[_focusedTrackIndex].Selected = true;
                listView1.Items[_focusedTrackIndex].Focused = true;

                PlaySong(); 
            }
            else
            {
                _mp.Stop();
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
            if (e.Button == MouseButtons.Right)
            {
            
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Bounds.Contains(new Point(e.X, e.Y)))
                {
                    contextMenuStrip1.Show(listView1, new Point(e.X, e.Y));
                    break;
                }
            }
            
            }
    }

        private void RemoveFromListViewAt(int index)
        {
            listView1.Items.RemoveAt(index);
        }

        private void RemoveFromPlaylistAt(int index)
        {
            _playList.RemoveAt(index);
        }

        void EditTagsFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            AddTrackToPlaylistAt(_editTagsForm.Track, _focusedTrackIndex);
            AddTrackToListView(_editTagsForm.Track,_focusedTrackIndex);
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
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = @"playlist files |*.playlist";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                listView1.Items.Clear();
                LoadPlaylist(openFileDialog1.FileName);
            }
        }

        private void editTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTagsOfSelectedTrack();
        }

        private void EditTagsOfSelectedTrack()
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            _editTagsForm = new EditTagsForm(_playList[_focusedTrackIndex]);
            _editTagsForm.Show();
            _editTagsForm.Closing += EditTagsFormClosing;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contextMenuStrip1.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedTracks();
        }

        private void RemoveSelectedTracks()
        {
            int count = listView1.SelectedIndices.Count;
            if (count > 1)
            {
                int index = listView1.SelectedIndices[0];
                for (int i = 0; i < count; i++)
                {
                    RemoveFromPlaylistAt(index);
                    RemoveFromListViewAt(index);
                }
            }
            else
            {
                RemoveFromPlaylistAt(_focusedTrackIndex);
                RemoveFromListViewAt(_focusedTrackIndex);
            }
            
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllTracks();
        }

        private void RemoveAllTracks()
        {
            _playList.Clear();
            listView1.Items.Clear();

            EnableButtons(false);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySong();
        }

        private void playNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        private void playPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayPreviousSong();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mp.Stop();
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveSelectedTracks();
        }

        private void removeAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveAllTracks();
        }

        private void byArtistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_sortColumn == -1)
            {
                SortListAscByParam(SortingParams.Artist);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(SortingParams.Artist);
                _sortColumn = -1;
            }
        }

        private void byTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_sortColumn == -1)
            {
                SortListAscByParam(SortingParams.Title);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(SortingParams.Title);
                _sortColumn = -1;
            }
        }

        private void byAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_sortColumn == -1)
            {
                SortListAscByParam(SortingParams.Album);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(SortingParams.Album);
                _sortColumn = -1;
            }
        }

        private void byLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_sortColumn == -1)
            {
                SortListAscByParam(SortingParams.Length);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(SortingParams.Length);
                _sortColumn = -1;
            }
        }

        private void byYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_sortColumn == -1)
            {
                SortListAscByParam(SortingParams.Year);
                _sortColumn = 1;
            }
            else
            {
                SortListDescByParam(SortingParams.Year);
                _sortColumn = -1;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedTracks();
        }

        private void CopySelectedTracks()
        {
            int count = listView1.SelectedIndices.Count;
            int index = listView1.SelectedIndices[0];
            _copyList = new Track[count];

            _playList.CopyTo(index, _copyList, 0, count);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteTracks();
        }

        private void PasteTracks()
        {
            _focusedTrackIndex = listView1.FocusedItem.Index;
            _playList.InsertRange(_focusedTrackIndex, _copyList);
            UpdateListView();
        }

        private void editTagsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditTagsOfSelectedTrack();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopySelectedTracks();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteTracks();
        }

        private void libraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _libraryForm = new LibraryForm();
            _libraryForm.Show();
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
