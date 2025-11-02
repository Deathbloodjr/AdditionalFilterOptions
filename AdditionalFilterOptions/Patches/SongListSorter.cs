using AdditionalFilterOptions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdditionalFilterOptions.Patches.SongData;

namespace AdditionalFilterOptions.Patches
{
    public enum SortType
    {
        //Default, // Is default even needed? maybe just to break out?
        Order,
        Genre,
        Difficulty,
        AlphabeticalTitle,
        AlphabeticalSubtitle,
        AlphabeticalSongId,
        UniqueId, // I don't know why anyone would ever want to sort by UniqueId, but go ahead. Actually this might be closest to an ordered by release date sorting.
        Score,
        Accuracy,
        Goods,
        Oks,
        Bads,
        OksAndBads,
        DrumrollCount,
        MaxCombo,
        PlayCount,
    }

    internal class SongListSorter
    {
        static Dictionary<SortType, Func<SongDifficultyData, string>> SortStringFunctions = new Dictionary<SortType, Func<SongDifficultyData, string>>();
        static Dictionary<SortType, Func<SongDifficultyData, int>> SortIntFunctions = new Dictionary<SortType, Func<SongDifficultyData, int>>();
        static Dictionary<SortType, Func<SongDifficultyData, float>> SortFloatFunctions = new Dictionary<SortType, Func<SongDifficultyData, float>>();

        static List<SortType> RemoveDuplicateSorts = new List<SortType>()
        {
            SortType.AlphabeticalTitle,
            SortType.AlphabeticalSubtitle,
            SortType.AlphabeticalSongId,
            SortType.UniqueId,
            SortType.Order,
            SortType.Genre,
        };

        static void InitializeSortFunctions()
        {
            AddOrReplaceSortFunction(SortType.Order, (x) => x.MusicInfo.Order);
            AddOrReplaceSortFunction(SortType.Genre, (x) => (int)x.Genre);
            AddOrReplaceSortFunction(SortType.Difficulty, (x) => x.Star);
            AddOrReplaceSortFunction(SortType.AlphabeticalTitle, (x) => x.SongTitle);
            AddOrReplaceSortFunction(SortType.AlphabeticalSubtitle, (x) => x.SongSubtitle);
            AddOrReplaceSortFunction(SortType.AlphabeticalSongId, (x) => x.MusicInfo.Id);
            AddOrReplaceSortFunction(SortType.UniqueId, (x) => x.MusicInfo.UniqueId);
            AddOrReplaceSortFunction(SortType.Score, (x) => x.Score);
            AddOrReplaceSortFunction(SortType.Accuracy, (x) => x.Accuracy);
            AddOrReplaceSortFunction(SortType.Goods, (x) => x.Goods);
            AddOrReplaceSortFunction(SortType.Oks, (x) => x.Oks);
            AddOrReplaceSortFunction(SortType.Bads, (x) => x.Bads);
            AddOrReplaceSortFunction(SortType.OksAndBads, (x) => x.OksAndBads);
            AddOrReplaceSortFunction(SortType.DrumrollCount, (x) => x.Drumroll);
            AddOrReplaceSortFunction(SortType.MaxCombo, (x) => x.Combo);
            AddOrReplaceSortFunction(SortType.PlayCount, (x) => x.PlayCount);
        }

        static void AddOrReplaceSortFunction(SortType type, Func<SongDifficultyData, string> func)
        {
            if (!SortStringFunctions.ContainsKey(type))
            {
                SortStringFunctions.Add(type, func);
            }
        }

        static void AddOrReplaceSortFunction(SortType type, Func<SongDifficultyData, int> func)
        {
            if (!SortIntFunctions.ContainsKey(type))
            {
                SortIntFunctions.Add(type, func);
            }
        }

        static void AddOrReplaceSortFunction(SortType type, Func<SongDifficultyData, float> func)
        {
            if (!SortFloatFunctions.ContainsKey(type))
            {
                SortFloatFunctions.Add(type, func);
            }
        }

        static IOrderedEnumerable<SongDifficultyData> SortSongs(List<SongDifficultyData> songs, SortType sortType, bool isDescending = false)
        {
            if (SortStringFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.OrderBy(SortStringFunctions[sortType]);
                }
                else
                {
                    return songs.OrderByDescending(SortStringFunctions[sortType]);
                }
            }
            else if (SortIntFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.OrderBy(SortIntFunctions[sortType]);
                }
                else
                {
                    return songs.OrderByDescending(SortIntFunctions[sortType]);
                }
            }
            else if (SortFloatFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.OrderBy(SortFloatFunctions[sortType]);
                }
                else
                {
                    return songs.OrderByDescending(SortFloatFunctions[sortType]);
                }
            }
            else
            {
                ModLogger.Log("Issue attempting to sort by " + sortType.ToString());
                return null;
            }
        }

        static IOrderedEnumerable<SongDifficultyData> SortSongs(IOrderedEnumerable<SongDifficultyData> songs, SortType sortType, bool isDescending = false)
        {
            if (SortStringFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.ThenBy(SortStringFunctions[sortType]);
                }
                else
                {
                    return songs.ThenByDescending(SortStringFunctions[sortType]);
                }
            }
            else if (SortIntFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.ThenBy(SortIntFunctions[sortType]);
                }
                else
                {
                    return songs.ThenByDescending(SortIntFunctions[sortType]);
                }
            }
            else if (SortFloatFunctions.ContainsKey(sortType))
            {
                if (!isDescending)
                {
                    return songs.ThenBy(SortFloatFunctions[sortType]);
                }
                else
                {
                    return songs.ThenByDescending(SortFloatFunctions[sortType]);
                }
            }
            else
            {
                ModLogger.Log("Issue attempting to sort by " + sortType.ToString());
                return null;
            }
        }

        public static List<SongDifficultyData> SortSongs(List<SongDifficultyData> songs, SortSettings sortSettings)
        {
            if (sortSettings.Sorts.Count == 0)
            {
                return songs;
            }

            InitializeSortFunctions();

            // TODO: Add IsAscending stuff
            var sortedList = SortSongs(songs, sortSettings.PrimarySort);
            for (int i = 1; i < sortSettings.Sorts.Count; i++)
            {
                sortedList = SortSongs(sortedList, sortSettings.Sorts[i]);
            }

            return sortedList.ToList();
        }

        public static List<SongDifficultyData> RemoveDuplicates(List<SongDifficultyData> songs, SortSettings sortSettings)
        {
            List<SongDifficultyData> result = new List<SongDifficultyData>();
            if (RemoveDuplicateSorts.Contains(sortSettings.PrimarySort))
            {
                for (int i = 0; i < songs.Count; i++)
                {
                    if (result.Find((x) => x.MusicInfo.Id == songs[i].MusicInfo.Id) == null)
                    {
                        result.Add(songs[i]);
                    }
                }
            }
            else
            {
                result.AddRange(songs);
            }
            return result;
        }
    }
}
