using AdditionalFilterOptions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicDataInterface;

namespace AdditionalFilterOptions.Patches
{
    internal class SongData
    {
        static Dictionary<MusicDataInterface.MusicInfoAccesser, SongData> SongDataDictionary = new Dictionary<MusicDataInterface.MusicInfoAccesser, SongData>();

        List<SongDifficultyData> DifficultyData { get; set; } = new List<SongDifficultyData>();

        SongData(MusicDataInterface.MusicInfoAccesser musicInfo, SongSelectManager.Song song)
        {
            if (musicInfo.Debug)
            {
                return;
            }

            DifficultyData.Add(new SongDifficultyData(musicInfo, EnsoData.EnsoLevelType.Easy, song));
            DifficultyData.Add(new SongDifficultyData(musicInfo, EnsoData.EnsoLevelType.Normal, song));
            DifficultyData.Add(new SongDifficultyData(musicInfo, EnsoData.EnsoLevelType.Hard, song));
            DifficultyData.Add(new SongDifficultyData(musicInfo, EnsoData.EnsoLevelType.Mania, song));
            if (musicInfo.Stars[(int)EnsoData.EnsoLevelType.Ura] != 0)
            {
                DifficultyData.Add(new SongDifficultyData(musicInfo, EnsoData.EnsoLevelType.Ura, song));
            }
        }

        public static SongData GetSongData(MusicDataInterface.MusicInfoAccesser musicInfo, SongSelectManager.Song song)
        {
            if (SongDataDictionary.ContainsKey(musicInfo))
            {
                return SongDataDictionary[musicInfo];
            }

            var songData = new SongData(musicInfo, song);
            SongDataDictionary.Add(musicInfo, songData);
            return songData;
        }

        public static SongData GetSongData(SongSelectManager.Song song)
        {
            var musicInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.GetInfoById(song.Id);
            return GetSongData(musicInfo, song);
        }

        public static List<SongDifficultyData> InitializeFullSongList(List<SongSelectManager.Song> songs)
        {
            List<SongDifficultyData> result = new List<SongDifficultyData>();
            for (int i = 0; i < songs.Count; i++)
            {
                var songData = GetSongData(songs[i]);
                foreach (var diffData in songData.DifficultyData)
                {
                    result.Add(diffData);
                }
            }
            return result;
        }

        public List<SongDifficultyData> GetValidSongDifficulties(FilterSettings filters)
        {
            List<SongDifficultyData> validDifficulties = new List<SongDifficultyData>();
            for (int i = 0; i < DifficultyData.Count; i++)
            {
                if (DifficultyData[i].IsValidWithFilter(filters))
                {
                    validDifficulties.Add(DifficultyData[i]);
                }
            }
            return validDifficulties;
        }

        public bool IsValidWithFilter(FilterSettings filters)
        {
            for (int i = 0; i < DifficultyData.Count; i++)
            {
                if (DifficultyData[i].IsValidWithFilter(filters))
                {
                    return true;
                }
            }

            return false;
        }

        internal class SongDifficultyData
        {
            public bool IsEnabled { get; set; }
            public MusicDataInterface.MusicInfoAccesser MusicInfo { get; set; } = null;
            public EnsoData.EnsoLevelType EnsoLevelType { get; set; } = EnsoData.EnsoLevelType.Num;

            public string SongTitle { get; set; }
            public string SongSubtitle { get; set; }
            public string SongDetail { get; set; }

            public bool IsFavorite { get; set; }
            public bool IsBonus { get; set; }

            public EnsoRecordInfo Record 
            { 
                get
                {
                    if (IsEnabled)
                    {
                        TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.GetPlayerRecordInfo(0, MusicInfo.UniqueId, EnsoLevelType, out var result);
                        return result;
                    }
                    else
                    {
                        return new EnsoRecordInfo();
                    }
                }
            }
            public EnsoData.SongGenre Genre
            {
                get
                {
                    if (MusicInfo != null)
                    {
                        return (EnsoData.SongGenre)MusicInfo.GenreNo;
                    }
                    else
                    {
                        return EnsoData.SongGenre.Namco;
                    }
                }
            }
            public int Star { 
                get
                {
                    if (MusicInfo != null && EnsoLevelType != EnsoData.EnsoLevelType.Num)
                    {
                        return MusicInfo.Stars[(int)EnsoLevelType];
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            public DataConst.CrownType Crown
            {
                get
                {
                    return Record.crown;
                }
            }
            public int Score
            {
                get
                {
                    return Record.normalHiScore.score;
                }
            }
            int NoteCount
            {
                get
                {
                    return Record.normalHiScore.excellent + Record.normalHiScore.good + Record.normalHiScore.bad;
                }
            }
            public int Goods
            {
                get
                {
                    // This isn't really needed for Goods, but it is necessary for OKs and Bads
                    return Record.normalHiScore.excellent;
                }
            }
            public int Oks
            {
                get
                {
                    if (NoteCount != 0)
                    {
                        return Record.normalHiScore.good;
                    }
                    else if (NoteCount != 0)
                    {
                        return Record.normalHiScore.good;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
            }
            public int Bads
            {
                get
                {
                    if (NoteCount != 0)
                    {
                        return Record.normalHiScore.bad;
                    }
                    else if (NoteCount != 0)
                    {
                        return Record.normalHiScore.bad;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
            }
            public int OksAndBads
            {
                get
                {
                    if (NoteCount != 0)
                    {
                        return Record.normalHiScore.bad + Record.normalHiScore.good;
                    }
                    else if (NoteCount != 0)
                    {
                        return Record.normalHiScore.bad + Record.normalHiScore.good;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
            }
            public int Drumroll
            {
                get
                {
                    return Record.normalHiScore.renda;
                }
            }
            public int Combo
            {
                get
                {
                    return Record.normalHiScore.combo;
                }
            }
            public int PlayCount
            {
                get
                {
                    return Record.playCount;
                }
            }
            /// <summary>
            /// Float from 0.0 to 1.0
            /// </summary>
            public float Accuracy
            {
                get
                {
                    return NormalAccuracy;
                }
            }
            float NormalAccuracy
            {
                get
                {
                    int numNotes = Record.normalHiScore.excellent + Record.normalHiScore.good + Record.normalHiScore.bad;
                    if (numNotes == 0)
                    {
                        return 0f;
                    }
                    return (Record.normalHiScore.excellent + (Record.normalHiScore.good / 2f)) / numNotes;
                }
            }
            public SongDifficultyData(MusicDataInterface.MusicInfoAccesser musicInfo, EnsoData.EnsoLevelType level, SongSelectManager.Song song)
            {
                IsEnabled = true;
                MusicInfo = musicInfo;
                EnsoLevelType = level;
                if (Star == 0)
                {
                    IsEnabled = false;
                }

                SongTitle = song.TitleText;
                SongSubtitle = song.SubText;
                SongDetail = song.DetailText;

                IsFavorite = song.Favorite;
                IsBonus = song.DailyBonus;
            }

            public bool IsValidWithFilter(FilterSettings filters)
            {
                if (!IsEnabled)
                {
                    return false;
                }

                bool result = true;

                result &= filters.GetDifficulty(EnsoLevelType);
                result &= filters.MinDifficulty <= Star && Star <= filters.MaxDifficulty;
                result &= filters.GetCrown(Crown);
                result &= filters.GetGenre(Genre);

                if (filters.Favorite)
                {
                    result &= IsFavorite;
                }
                if (filters.Bonus)
                {
                    result &= IsBonus;
                }


                return result;
            }
        }
    }
}
