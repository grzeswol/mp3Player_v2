using System;
using System.Globalization;

namespace mp3Player_v2
{
    public class Track
    {
        private TagLib.File _tagFile;
        private readonly string _path;
        

        
        public string Artist { get;  private set; }
        public string Title { get;  private set; }
        public string Album { get;  private set; }
        public string Length { get; private set; }
        public string Year { get; private set; }

        public Track(string pathOfTheFile)
        {
            _path = pathOfTheFile;
            _tagFile = TagLib.File.Create(_path);
            SetTrackFromFileFields();
        }

        public void SetArtistTag(string[] artistName)
        {
            _tagFile.Tag.AlbumArtists = artistName;
            _tagFile.Save();
            SetTrackFromFileFields();
        }

        public void SetTitleTag(string title)
        {
            _tagFile.Tag.Title = title;
            _tagFile.Save();
            SetTrackFromFileFields();

        }

        public void SetAlbumTag(string albumName)
        {
            _tagFile.Tag.Album = albumName;
            _tagFile.Save();
            SetTrackFromFileFields();
        }

        public void SetYearTag(uint year)
        {
            _tagFile.Tag.Year = year;
            _tagFile.Save();
            SetTrackFromFileFields();
        }

        private void SetTrackFromFileFields()
        {
            GetArtistsTagFromFile();
            GetTitleTagFromFile();
            GetAlbumTagFromFile();
            GetLengthTagFromFile();
            GetYearTagFromFile();
        }

        public string GetTrackPath()
        {
            return _path;
        }

        private void GetYearTagFromFile()
        {
            if (_tagFile.Tag.Year == 0)
            {
                Year = "";
            }
            else Year = _tagFile.Tag.Year.ToString(CultureInfo.InvariantCulture);
        }

        private void GetLengthTagFromFile()
        {
            TimeSpan ts = _tagFile.Properties.Duration;
            Length = String.Format("{0}:{1}", ts.Minutes, ts.Seconds);
        }

        private void GetAlbumTagFromFile()
        {
            Album = _tagFile.Tag.Album;
        }

        private void GetArtistsTagFromFile()
        {
            string[] artists = new string[] {};
            if (_tagFile.Tag.AlbumArtists == null)
            {
                if (_tagFile != null) artists = new[] {_tagFile.Tag.JoinedPerformers};
            }
            else artists = _tagFile.Tag.AlbumArtists;
            Artist = String.Concat(artists);
        }

        private void GetTitleTagFromFile()
        {
            Title = _tagFile.Tag.Title;
        }
    }
}
