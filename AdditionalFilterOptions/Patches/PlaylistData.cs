using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace AdditionalFilterOptions.Patches
{
    internal class PlaylistData
    {
        public string Name { get; set; }
        public string JsonFilePath { get; set; }
        public int Order { get; set; }
        public List<PlaylistSongData> Songs { get; set; } = new List<PlaylistSongData>();

        PlaylistData()
        {

        }

        public static PlaylistData CreatePlaylistDataFromFilePath(string filePath)
        {
            PlaylistData data = new PlaylistData();

            if (!File.Exists(filePath))
            {
                ModLogger.Log("Playlist file not found: " + filePath, LogType.Warning);
                data.Name = "None";
                return data;
            }

            data.InitializeJsonData(filePath);

            return data;
        }

        private void InitializeJsonData(string jsonFilePath)
        {
            if (jsonFilePath == "")
            {
                Name = "None";
                JsonFilePath = "";
                return;
            }

            if (File.Exists(jsonFilePath))
            {
                FileInfo file = new FileInfo(jsonFilePath);
                LWJson node = LWJson.Parse(File.ReadAllText(file.FullName));
                JsonFilePath = file.FullName;
                InitializeData(node);
            }
            else
            {
                Name = "None";
            }
        }

        private void InitializeData(LWJson node)
        {
            Name = node["playlistName"].AsString();
            Order = node["order"].AsInteger();
            var songsArray = node["songs"].AsArray();
            for (int i = 0; i < songsArray.Count; i++)
            {
                var song = PlaylistSongData.CreatePlaylistSongData(songsArray[i]);
                if (song is not null)
                {
                    Songs.Add(song);
                }
            }
        }

        public void ReloadSongList()
        {
            if (File.Exists(JsonFilePath))
            {
                LWJson node = LWJson.Parse(File.ReadAllText(JsonFilePath));
                Songs.Clear();
                var songsArray = node["songs"].AsArray();
                for (int i = 0; i < songsArray.Count; i++)
                {
                    var song = PlaylistSongData.CreatePlaylistSongData(songsArray[i]);
                    if (song is not null)
                    {
                        Songs.Add(song);
                    }
                }
            }
        }
    }
}
