using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdditionalFilterOptions.Patches;

namespace AdditionalFilterOptions.Settings
{
    internal class FilterSettings
    {
        public PlaylistData PlaylistData { get; set; }
        Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties { get; set; } = new Dictionary<EnsoData.EnsoLevelType, bool>();
        public Dictionary<EnsoData.SongGenre, bool> EnabledGenres { get; set; } = new Dictionary<EnsoData.SongGenre, bool>();
        public Dictionary<DataConst.CrownType, bool> EnabledCrowns { get; set; } = new Dictionary<DataConst.CrownType, bool>();
        public bool Bonus { get; set; }
        public bool Favorite { get; set; }
        public int MinDifficulty { get; set; }
        public int MaxDifficulty { get; set; }
        public string TextFilter { get; set; }

        public FilterSettings()
        {
            ResetValues();
        }

        public void ResetValues()
        {
            PlaylistData = PlaylistData.CreatePlaylistDataFromFilePath("");
            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var diff = (EnsoData.EnsoLevelType)i;
                SetDifficulty(diff, true);
            }
            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var genre = (EnsoData.SongGenre)i;
                if (genre == EnsoData.SongGenre.Children)
                {
                    continue;
                }

                SetGenre(genre, true);
            }
            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                var crown = (DataConst.CrownType)i;
                if (crown == DataConst.CrownType.Off ||
                    crown == DataConst.CrownType.Bronze)
                {
                    continue;
                }

                SetCrown(crown, true);
            }
            Bonus = false;
            Favorite = false;
            MinDifficulty = 1;
            MaxDifficulty = 10;
            TextFilter = string.Empty;
        }

        public bool GetDifficulty(EnsoData.EnsoLevelType diff)
        {
            if (!EnabledDifficulties.ContainsKey(diff))
            {
                return false;
            }
            else
            {
                return EnabledDifficulties[diff];
            }
        }
        public void SetDifficulty(EnsoData.EnsoLevelType diff, bool value)
        {
            if (!EnabledDifficulties.ContainsKey(diff))
            {
                EnabledDifficulties.Add(diff, value);
            }
            else
            {
                EnabledDifficulties[diff] = value;
            }
        }

        public bool GetGenre(EnsoData.SongGenre genre)
        {
            if (!EnabledGenres.ContainsKey(genre))
            {
                return false;
            }
            else
            {
                return EnabledGenres[genre];
            }
        }
        public void SetGenre(EnsoData.SongGenre genre, bool value)
        {
            if (!EnabledGenres.ContainsKey(genre))
            {
                EnabledGenres.Add(genre, value);
            }
            else
            {
                EnabledGenres[genre] = value;
            }
        }

        public bool GetCrown(DataConst.CrownType crown)
        {
            if (!EnabledCrowns.ContainsKey(crown))
            {
                return false;
            }
            else
            {
                return EnabledCrowns[crown];
            }
        }
        public void SetCrown(DataConst.CrownType crown, bool value)
        {
            if (!EnabledCrowns.ContainsKey(crown))
            {
                EnabledCrowns.Add(crown, value);
            }
            else
            {
                EnabledCrowns[crown] = value;
            }
        }
    }
}
