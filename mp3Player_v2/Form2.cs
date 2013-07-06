using System.Windows.Forms;

namespace mp3Player_v2
{
    public partial class Form2 : Form
    {
        private Track _track;

        public Form2(Track track)
        {
            InitializeComponent();
            Track = track;
            GetTrackTags();
        }

        public Track Track
        {
            get { return _track; }
            private set { _track = value; }
        }

        private void GetTrackTags()
        {
            dataGridView1.Rows.Add("Artist", _track.Artist);
            dataGridView1.Rows.Add("Title", _track.Title);
            dataGridView1.Rows.Add("Album", _track.Album);
            dataGridView1.Rows.Add("Year", _track.Year);
            
        }

        private void applyButton_Click(object sender, System.EventArgs e)
        {
            SetTags();
        }

        private void SetTags()
        {
            Track.SetArtistTag(new[] {dataGridView1.Rows[0].Cells[1].Value.ToString()});
            Track.SetTitleTag(dataGridView1.Rows[1].Cells[1].Value.ToString());
            Track.SetAlbumTag(dataGridView1.Rows[2].Cells[1].Value.ToString());
            SetYearTag(dataGridView1.Rows[3].Cells[1].Value.ToString());
        }

        private void SetYearTag(string s)
        {
            uint yearTag;

            if (uint.TryParse(s, out yearTag))
            {
                Track.SetYearTag(yearTag);
            }

            else
            {
                MessageBox.Show(@"Please enter valid value.");
            }
        }

        private void cancButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            SetTags();
            Close();
        }
    }

    
}
