using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace mp3Player_v2
{
    public partial class LibraryForm : Form
    {
        private const string LibraryName = "Library.sdf";
        private const string ConnString = @"Data Source = " + LibraryName;
        private const string FolderPaths = "paths";

        private List<string> _trackPath = new List<string>();
        private List<Track> _trackList = new List<Track>(); 

        public LibraryForm()
        {
            InitializeComponent();

            if (File.Exists(LibraryName))
            {
                ShowTable();
            }
        }

        private void FillDatabase()
        {
            foreach (Track track in _trackList)
            {
                AddTrackData(track);
            }
        }

        private void SetTrackList()
        {
            
            foreach (string s in _trackPath)
            {
                _trackList.Add(new Track(s));
            }
        }

        private void ScanForMp3Files()
        {
            _trackPath.Clear();
            
            try
            {
                using (StreamReader stream = new StreamReader(FolderPaths))
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        _trackPath.AddRange(Directory.EnumerateFiles(line, "*.mp3", SearchOption.AllDirectories));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Damaged file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void CreateDatabase()
        {
           SqlCeEngine engine = new SqlCeEngine(ConnString);
           engine.CreateDatabase();
        }

        private void CreateTable()
        {
            using (var connection = new SqlCeConnection(ConnString))
            {
                connection.Open();
                const string query = "CREATE TABLE Library (L_Id int NOT NULL IDENTITY PRIMARY KEY, " +
                                     "Artist nvarchar(100), " +
                                     "Title nvarchar(100), " +
                                     "Album nvarchar(100), " +
                                     "Length nvarchar(100), " +
                                     "Year nvarchar(100))";
                                     

                 
                SqlCeCommand comm = new SqlCeCommand(query, connection);
                comm.ExecuteNonQuery();
            }
        }

        private void ShowTable()
        {
            using (var connection = new SqlCeConnection(ConnString))
            {
                connection.Open();

                using (SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter("SELECT * FROM Library", connection))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }

            }
        }

        private void AddTrackData(Track track)
        {
            using (var connection = new SqlCeConnection(ConnString))
            {
                connection.Open();

                const string stmt = "INSERT INTO Library(Artist, Title, Album, Length, Year) " +
                                    "VALUES(@Artist, @Title, @Album, @Length, @Year)";

                SqlCeCommand cmd = new SqlCeCommand(stmt, connection);
                cmd.Parameters.Add("@Artist", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@Album", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@Length", SqlDbType.NVarChar, 100);
                cmd.Parameters.Add("@Year", SqlDbType.NVarChar, 100);

                if (track.Artist == null)
                {
                    cmd.Parameters["@Artist"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@Artist"].Value = track.Artist;
                }

                if (track.Album == null)
                {
                    cmd.Parameters["@Album"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@Album"].Value = track.Album;
                }

                if (track.Title == null)
                {
                    cmd.Parameters["@Title"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@Title"].Value = track.Title;
                }

                if (track.Title == null)
                {
                    cmd.Parameters["@Length"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@Length"].Value = track.Length;
                }

                if (track.Title == null)
                {
                    cmd.Parameters["@Year"].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters["@Year"].Value = track.Year;
                }

                
                cmd.ExecuteNonQuery();
                
            }
            
        }

        private void DeleteTable()
        {
            using (var connection = new SqlCeConnection(ConnString))
            {
                connection.Open();

                const string stmt = "DROP TABLE Library";

                SqlCeCommand cmd = new SqlCeCommand(stmt, connection);
                cmd.ExecuteNonQuery();

            }

        }

        private void createLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectionForm folderSelectionForm = new FolderSelectionForm();
            folderSelectionForm.ShowDialog();
            
            if (!File.Exists(LibraryName))
            {
                CreateDatabase();
                CreateTable();
                ScanForMp3Files();
                SetTrackList();
                FillDatabase();
                ShowTable();
            }

            else
            {
                MessageBox.Show(@"Do you want to overwrite database?", @"Create database", MessageBoxButtons.OKCancel);
                if (DialogResult == DialogResult.OK)
                {
                    DeleteTable();
                    CreateTable();
                    ScanForMp3Files();
                    SetTrackList();
                    FillDatabase();
                    ShowTable();
                }
            }
            
        }
    }
}
