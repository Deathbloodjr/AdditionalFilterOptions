using AdditionalFilterOptions.Patches;
using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Settings
{
    internal class SaveSettingsManager
    {
        const string DefaultSettingsFileName = "DefaultSettings.json";
        const string LatestSettingsFileName = "LatestSettings.json";

        public static FilterSettings filterSettings;
        public static SortSettings sortSettings;

        public static void LoadDefaultSettings()
        {
            string filePath = Path.Combine(Plugin.Instance.ConfigSettingsLocation.Value, DefaultSettingsFileName);
            LoadSettings(filePath);
        }

        public static void LoadLatestSettings()
        {
            string filePath = Path.Combine(Plugin.Instance.ConfigSettingsLocation.Value, LatestSettingsFileName);
            LoadSettings(filePath);
        }

        public static void SaveDefaultSettings()
        {
            string filePath = Path.Combine(Plugin.Instance.ConfigSettingsLocation.Value, DefaultSettingsFileName);
            SaveSettings(filePath);
        }

        public static void SaveLatestSettings()
        {
            string filePath = Path.Combine(Plugin.Instance.ConfigSettingsLocation.Value, LatestSettingsFileName);
            SaveSettings(filePath);
        }

        private static void LoadSettings(string filePath)
        {
            filterSettings = new FilterSettings();
            sortSettings = new SortSettings();
            try
            {

                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    var json = LWJson.Parse(File.ReadAllText(file.FullName));
                    filterSettings.SetDifficulty(EnsoData.EnsoLevelType.Easy, json["Difficulties"]["Easy"].AsBoolean());
                    filterSettings.SetDifficulty(EnsoData.EnsoLevelType.Normal, json["Difficulties"]["Normal"].AsBoolean());
                    filterSettings.SetDifficulty(EnsoData.EnsoLevelType.Hard, json["Difficulties"]["Hard"].AsBoolean());
                    filterSettings.SetDifficulty(EnsoData.EnsoLevelType.Mania, json["Difficulties"]["Oni"].AsBoolean());
                    filterSettings.SetDifficulty(EnsoData.EnsoLevelType.Ura, json["Difficulties"]["Ura"].AsBoolean());

                    filterSettings.SetGenre(EnsoData.SongGenre.Pops, json["Genres"]["Pops"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Anime, json["Genres"]["Anime"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Vocalo, json["Genres"]["Vocaloid"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Variety, json["Genres"]["Variety"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Classic, json["Genres"]["Classical"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Game, json["Genres"]["GameMusic"].AsBoolean());
                    filterSettings.SetGenre(EnsoData.SongGenre.Namco, json["Genres"]["NamcoOriginal"].AsBoolean());

                    filterSettings.SetCrown(DataConst.CrownType.None, json["Crowns"]["None"].AsBoolean());
                    filterSettings.SetCrown(DataConst.CrownType.Silver, json["Crowns"]["Silver"].AsBoolean());
                    filterSettings.SetCrown(DataConst.CrownType.Gold, json["Crowns"]["Gold"].AsBoolean());
                    filterSettings.SetCrown(DataConst.CrownType.Rainbow, json["Crowns"]["Rainbow"].AsBoolean());

                    filterSettings.MinDifficulty = json["MinDifficulty"].AsInteger();
                    filterSettings.MaxDifficulty = json["MaxDifficulty"].AsInteger();

                    filterSettings.TextFilter = json["TextFilter"].AsString();

                    filterSettings.Bonus = json["Bonus"].AsBoolean();
                    filterSettings.Favorite = json["Favorite"].AsBoolean();

                    sortSettings.Sorts.Clear();
                    var sorts = json["SortType"].AsArray();
                    for (int i = 0; i < sorts.Count; i++)
                    {
                        if (Enum.TryParse(sorts[i].AsString(), out SortType sortEnum))
                        {
                            sortSettings.Sorts.Add(sortEnum);
                        }
                    }

                    // Don't save current playlist in settings right now
                    // This entire mod needs a rewrite, we can deal with it later
                    filterSettings.PlaylistData = PlaylistData.CreatePlaylistDataFromFilePath(json["Playlist"].AsString());
                }

            }
            catch (Exception)
            {
                // This is extremely lazy, but it's how I'm going to do it for now
            }
        }


        private static void SaveSettings(string filePath)
        {
            var saveJsonObject = new LWJsonObject()
                .Add("Difficulties", new LWJsonObject()
                    .Add("Easy", filterSettings.GetDifficulty(EnsoData.EnsoLevelType.Easy))
                    .Add("Normal", filterSettings.GetDifficulty(EnsoData.EnsoLevelType.Normal))
                    .Add("Hard", filterSettings.GetDifficulty(EnsoData.EnsoLevelType.Hard))
                    .Add("Oni", filterSettings.GetDifficulty(EnsoData.EnsoLevelType.Mania))
                    .Add("Ura", filterSettings.GetDifficulty(EnsoData.EnsoLevelType.Ura)))
                .Add("Genres", new LWJsonObject()
                    .Add("Pops", filterSettings.GetGenre(EnsoData.SongGenre.Pops))
                    .Add("Anime", filterSettings.GetGenre(EnsoData.SongGenre.Anime))
                    .Add("Vocaloid", filterSettings.GetGenre(EnsoData.SongGenre.Vocalo))
                    .Add("Variety", filterSettings.GetGenre(EnsoData.SongGenre.Variety))
                    .Add("Classical", filterSettings.GetGenre(EnsoData.SongGenre.Classic))
                    .Add("GameMusic", filterSettings.GetGenre(EnsoData.SongGenre.Game))
                    .Add("NamcoOriginal", filterSettings.GetGenre(EnsoData.SongGenre.Namco)))
                .Add("Crowns", new LWJsonObject()
                    .Add("None", filterSettings.GetCrown(DataConst.CrownType.None))
                    .Add("Silver", filterSettings.GetCrown(DataConst.CrownType.Silver))
                    .Add("Gold", filterSettings.GetCrown(DataConst.CrownType.Gold))
                    .Add("Rainbow", filterSettings.GetCrown(DataConst.CrownType.Rainbow)))
                .Add("MinDifficulty", filterSettings.MinDifficulty)
                .Add("MaxDifficulty", filterSettings.MaxDifficulty)
                .Add("Playlist", filterSettings.PlaylistData.JsonFilePath)
                .Add("SortType", new LWJsonArray())
                .Add("TextFilter", filterSettings.TextFilter)
                .Add("Bonus", filterSettings.Bonus)
                .Add("Favorite", filterSettings.Favorite);
            for (int i = 0; i < sortSettings.Sorts.Count; i++)
            {
                var sort = sortSettings.Sorts[i];
                saveJsonObject["SortType"].AsArray().Add(new LWJsonValue(sort.ToString()));
            }

            FileInfo file = new FileInfo(filePath);
            var dir = file.Directory;
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            File.WriteAllText(filePath, saveJsonObject.ToString());
        }
    }
}
