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
        public List<PlaylistSongData> Songs { get; set; }

        public PlaylistData(string jsonFilePath)
        {
            if (!InitializeJsonData(jsonFilePath))
            {
                InitializeJsonData(Path.Combine(Plugin.Instance.ConfigPlaylistLocation.Value, jsonFilePath));
            }
        }

        private bool InitializeJsonData(string jsonFilePath)
        {
            if (jsonFilePath == "")
            {
                Name = "None";
                JsonFilePath = "";
                return true;
            }

            if (File.Exists(jsonFilePath))
            {
                FileInfo file = new FileInfo(jsonFilePath);
                LWJson node = LWJson.Parse(File.ReadAllText(file.FullName));
                JsonFilePath = file.FullName.Replace(Path.GetFullPath(Plugin.Instance.ConfigPlaylistLocation.Value) + "\\", "");
                InitializeData(node);
            }
            else
            {
                Name = "None";
                JsonFilePath = "";
            }
            return false;
        }

        private void InitializeData(LWJson node)
        {
            Name = node["playlistName"].AsString();
            Order = node["order"].AsInteger();
            Songs = new List<PlaylistSongData>();
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
            LWJson node = LWJson.Parse(File.ReadAllText(JsonFilePath));
            Songs = new List<PlaylistSongData>();
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
