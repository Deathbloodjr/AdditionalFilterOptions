using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class PlaylistSongData
    {
        public string SongId { get; set; }
        public int GenreNo { get; set; }
        public bool IsDlc { get; set; } = false;

        PlaylistSongData()
        {

        }

        public static PlaylistSongData? CreatePlaylistSongData(LWJson node)
        {
            PlaylistSongData data = new PlaylistSongData();
            if (node["songId"] is not null)
            {
                data.SongId = node["songId"].AsString();
            }
            else
            {
                return null;
            }

            if (node["genreNo"] is not null)
            {
                data.GenreNo = node["genreNo"].AsInteger();
            }
            else
            {
                // Namco Original
                data.GenreNo = 7;
            }

            if (node["isDlc"] is not null)
            {
                data.IsDlc = node["isDlc"].AsBoolean();
            }
            else
            {
                data.IsDlc = false;
            }

            return data;
        }
    }
}
