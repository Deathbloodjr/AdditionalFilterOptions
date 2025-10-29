using AdditionalFilterOptions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    public enum SortType
    {
        Default,
        Genre,
        Difficulty,
        AlphabeticalTitle,
        AlphabeticalSubtitle,
        AlphabeticalSongId,
        Accuracy,
    }

    internal class Sort
    {
        #region SongSelectManager.Song Sorting
        static public List<SongSelectManager.Song> SortSongList(List<SongSelectManager.Song> SongList, List<SortType> SortList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            bool sortedByDifficulty = false;
            if (SortList.Count == 0)
            {
                return SongList;
            }
            IOrderedEnumerable<SongSelectManager.Song> OrderedList = null;
            switch (SortList[0])
            {
                case SortType.Default:
                    OrderedList = SortByDefault(SongList);
                    break;
                case SortType.Genre:
                    OrderedList = SortByGenre(SongList);
                    break;
                case SortType.Difficulty:
                    OrderedList = SortByDifficulty(SongList, EnabledDifficulties);
                    sortedByDifficulty = true;
                    break;
                case SortType.AlphabeticalTitle:
                    OrderedList = SortByAlphabeticalTitle(SongList);
                    break;
                case SortType.AlphabeticalSubtitle:
                    OrderedList = SortByAlphabeticalSubtitle(SongList);
                    break;
                case SortType.AlphabeticalSongId:
                    OrderedList = SortByAlphabeticalSongId(SongList);
                    break;
                case SortType.Accuracy:
                    OrderedList = SortByAccuracy(SongList, EnabledDifficulties);
                    break;
            }
            for (int i = 1; i < SortList.Count; i++)
            {
                switch (SortList[i])
                {
                    case SortType.Default:
                        OrderedList = SortByDefault(OrderedList);
                        break;
                    case SortType.Genre:
                        OrderedList = SortByGenre(OrderedList);
                        break;
                    case SortType.Difficulty:
                        OrderedList = SortByDifficulty(OrderedList, EnabledDifficulties);
                        sortedByDifficulty = true;
                        break;
                    case SortType.AlphabeticalTitle:
                        OrderedList = SortByAlphabeticalTitle(OrderedList);
                        break;
                    case SortType.AlphabeticalSubtitle:
                        OrderedList = SortByAlphabeticalSubtitle(OrderedList);
                        break;
                    case SortType.AlphabeticalSongId:
                        OrderedList = SortByAlphabeticalSongId(OrderedList);
                        break;
                    case SortType.Accuracy:
                        OrderedList = SortByAccuracy(OrderedList, EnabledDifficulties, sortedByDifficulty);
                        break;
                }
            }
            if (OrderedList == null)
            {
                return SongList;
            }
            return OrderedList.ToList();
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDefault(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.SongGenre)
                           .ThenBy((x) => x.Order)
                           .ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDefault(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.SongGenre)
                           .ThenBy((x) => x.Order)
                           .ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByGenre(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.SongGenre);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByGenre(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.SongGenre);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDifficulty(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            return SongList.OrderBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? x.Stars[0] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? x.Stars[1] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? x.Stars[2] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? x.Stars[3] : 0,
                                                    EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? x.Stars[4] : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDifficulty(IOrderedEnumerable<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            return SongList.ThenBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? x.Stars[0] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? x.Stars[1] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? x.Stars[2] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? x.Stars[3] : 0,
                                                   EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? x.Stars[4] : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalTitle(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalTitle(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalSubtitle(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.SubText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalSubtitle(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.SubText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalSongId(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.Id);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabeticalSongId(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.Id);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAccuracy(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            return SongList.OrderBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? CalculateAccuracy(x.HighScores[0]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? CalculateAccuracy(x.HighScores[1]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? CalculateAccuracy(x.HighScores[2]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? CalculateAccuracy(x.HighScores[3]) : 0,
                                                    EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? CalculateAccuracy(x.HighScores[4]) : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAccuracy(IOrderedEnumerable<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties, bool sortedByDifficulty)
        {
            if (sortedByDifficulty)
            {
                return SongList.ThenBy((x) =>
                {
                    var acc = 0.0f;
                    var highestLevel = 0;
                    for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
                    {
                        var iDiff = (EnsoData.EnsoLevelType)i;
                        // x.Stars[i] >= highestLevel because we want ura over oni if they are both 8s (as an example)
                        if (EnabledDifficulties[iDiff] && x.Stars[i] >= highestLevel)
                        {
                            if (x.Stars[i] == highestLevel)
                            {
                                acc = Math.Max(acc, CalculateAccuracy(x.HighScores[i]));
                            }
                            else
                            {
                                highestLevel = x.Stars[i];
                                acc = CalculateAccuracy(x.HighScores[i]);
                            }
                        }
                    }
                    return acc;
                });
            }
            else
            {
                return SongList.ThenBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? CalculateAccuracy(x.HighScores[0]) : 0,
                                              Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? CalculateAccuracy(x.HighScores[1]) : 0,
                                              Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? CalculateAccuracy(x.HighScores[2]) : 0,
                                              Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? CalculateAccuracy(x.HighScores[3]) : 0,
                                                       EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? CalculateAccuracy(x.HighScores[4]) : 0)))));
            }

        }

        static float CalculateAccuracy(SongSelectManager.Score score)
        {
            var record = score.hiScoreRecordInfos;
            if ((record.excellent + record.good + record.bad) == 0)
            {
                return 0.0f;
            }
            else
            {
                return ((record.excellent + (record.good / 2.0f)) / (record.excellent + record.good + record.bad)) * 100;
            }
        }
        #endregion

        #region SongFilterData Sorting

        static public List<SongFilterData> SortSongList(List<SongFilterData> SongList, SortSettings SortList, bool allowDuplicates)
        {
            IOrderedEnumerable<SongFilterData> OrderedList = null;
            switch (SortList.PrimarySort)
            {
                case SortType.Default:
                    OrderedList = SortByOrder(SongList);
                    break;
                case SortType.Genre:
                    OrderedList = SortByGenre(SongList);
                    break;
                case SortType.Difficulty:
                    OrderedList = SortByDifficulty(SongList);
                    break;
                case SortType.AlphabeticalTitle:
                    OrderedList = SortByAlphabeticalTitle(SongList);
                    break;
                case SortType.AlphabeticalSubtitle:
                    OrderedList = SortByAlphabeticalSubtitle(SongList);
                    break;
                case SortType.AlphabeticalSongId:
                    OrderedList = SortByAlphabeticalSongId(SongList);
                    break;
                case SortType.Accuracy:
                    OrderedList = SortByAccuracy(SongList);
                    break;
            }
            for (int i = 1; i < SortList.Sorts.Count; i++)
            {
                switch (SortList.Sorts[i])
                {
                    case SortType.Default:
                        OrderedList = SortByOrder(OrderedList);
                        break;
                    case SortType.Genre:
                        OrderedList = SortByGenre(OrderedList);
                        break;
                    case SortType.Difficulty:
                        OrderedList = SortByDifficulty(OrderedList);
                        break;
                    case SortType.AlphabeticalTitle:
                        OrderedList = SortByAlphabeticalTitle(OrderedList);
                        break;
                    case SortType.AlphabeticalSubtitle:
                        OrderedList = SortByAlphabeticalSubtitle(OrderedList);
                        break;
                    case SortType.AlphabeticalSongId:
                        OrderedList = SortByAlphabeticalSongId(OrderedList);
                        break;
                    case SortType.Accuracy:
                        OrderedList = SortByAccuracy(OrderedList);
                        break;
                }
            }
            if (OrderedList == null)
            {
                return SongList;
            }
            var orderedListAsList = OrderedList.ToList();
            var newSongList = new List<SongFilterData>();
            for (int i = 0; i < orderedListAsList.Count; i++)
            {
                if (allowDuplicates)
                {
                    newSongList.Add(orderedListAsList[i]);
                }
                else
                {
                    var index = newSongList.FindIndex((x) => x.SongId == orderedListAsList[i].SongId);
                    if (index != -1)
                    {
                        newSongList.RemoveAt(index);
                    }
                    newSongList.Add(orderedListAsList[i]);
                }
            }
            Plugin.LogInfo("orderedListAsList.Count: " + orderedListAsList.Count);
            Plugin.LogInfo("newSongList.Count: " + newSongList.Count);
            return newSongList;
        }

        static IOrderedEnumerable<SongFilterData> SortByOrder(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.Order)
                           .ThenBy((x) => x.SongTitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByOrder(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.Order)
                           .ThenBy((x) => x.SongTitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByGenre(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.GenreNo);
        }

        static IOrderedEnumerable<SongFilterData> SortByGenre(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.GenreNo);
        }

        static IOrderedEnumerable<SongFilterData> SortByDifficulty(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.Star);
        }

        static IOrderedEnumerable<SongFilterData> SortByDifficulty(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.Star);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalTitle(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.SongTitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalTitle(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.SongTitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalSubtitle(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.SongSubtitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalSubtitle(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.SongSubtitle);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalSongId(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.SongId);
        }

        static IOrderedEnumerable<SongFilterData> SortByAlphabeticalSongId(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.SongId);
        }

        static IOrderedEnumerable<SongFilterData> SortByAccuracy(List<SongFilterData> SongList)
        {
            return SongList.OrderBy((x) => x.Accuracy);
        }

        static IOrderedEnumerable<SongFilterData> SortByAccuracy(IOrderedEnumerable<SongFilterData> SongList)
        {
            return SongList.ThenBy((x) => x.Accuracy);
        }


        #endregion

    }
}
