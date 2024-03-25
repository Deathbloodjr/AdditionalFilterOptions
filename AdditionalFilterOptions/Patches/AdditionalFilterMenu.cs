using AdditionalFilterOptions.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static ControllerManager;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterMenu : MonoBehaviour
    {
        const string FilterParentName = "AdditionalFilterMenuParent";

        GameObject realParent;
        GameObject parent;

        Dictionary<EnsoData.SongGenre, GameObject> GenreFilterButtons = new Dictionary<EnsoData.SongGenre, GameObject>();
        Dictionary<DataConst.CrownType, GameObject> CrownFilterButtons = new Dictionary<DataConst.CrownType, GameObject>();
        Dictionary<EnsoData.EnsoLevelType, GameObject> DifficultyFilterButtons = new Dictionary<EnsoData.EnsoLevelType, GameObject>();
        TMP_InputField inputField;
        Slider MinDifficulty;
        Slider MaxDifficulty;

        UIButton BonusButton;
        UIButton FavoriteButton;

        SongSelectManager songSelectManager { get; set; }
        static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();


        TextMeshProUGUI numSongsDisplay;

        List<PlaylistData> playlistDataObjects = new List<PlaylistData>();


        void Start()
        {
            realParent = AssetUtility.GetOrCreateEmptyObject(null, FilterParentName, Vector2.zero);
            realParent.layer = 5;
            //parent = GameObject.Find(FilterParentName);
            //if (parent == null)
            //{
            //    parent = new GameObject(FilterParentName);
            //    parent.layer = 5;
            //}

            transform.SetParent(realParent.transform);

            Canvas parentCanvas = realParent.GetComponent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = realParent.AddComponent<Canvas>();
                parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                parentCanvas.worldCamera = null;
                parentCanvas.overrideSorting = true;
                parentCanvas.sortingOrder = 2;


                var parentCanvasScaler = realParent.AddComponent<CanvasScaler>();
                parentCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                parentCanvasScaler.referenceResolution = new Vector2(1920, 1080);
                parentCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                parentCanvasScaler.matchWidthOrHeight = 0;
            }

            FullSongList = new List<SongSelectManager.Song>();
            //Plugin.Log.LogInfo("AdditionalFilterMenu Created");

            if (AdditionalFilterOptionsPatch.isFirstStartup)
            {
                SaveSettingsManager.LoadDefaultSettings();
                AdditionalFilterOptionsPatch.isFirstStartup = false;
            }
            else
            {
                SaveSettingsManager.LoadLatestSettings();
            }

            //sortSettings = new SortSettings();
            //sortSettings.Sorts.Add(SortType.Difficulty);
            //sortSettings.Sorts.Add(SortType.Accuracy);

            //filterSettings = new FilterSettings();
            //for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            //{
            //    if ((EnsoData.SongGenre)i == EnsoData.SongGenre.Children ||
            //        (EnsoData.SongGenre)i == EnsoData.SongGenre.Num)
            //    {
            //        continue;
            //    }
            //    if (!filterSettings.EnabledGenres.ContainsKey((EnsoData.SongGenre)i))
            //    {
            //        filterSettings.EnabledGenres.Add((EnsoData.SongGenre)i, true);
            //    }
            //}
            //for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            //{
            //    // Available crowns are None, Silver, Gold, and Rainbow
            //    if ((DataConst.CrownType)i == DataConst.CrownType.Bronze ||
            //        (DataConst.CrownType)i == DataConst.CrownType.Off ||
            //        (DataConst.CrownType)i == DataConst.CrownType.Num)
            //    {
            //        continue;
            //    }
            //    if (!filterSettings.EnabledCrowns.ContainsKey((DataConst.CrownType)i))
            //    {
            //        filterSettings.EnabledCrowns.Add((DataConst.CrownType)i, true);
            //    }
            //}
            //for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            //{
            //    var diff = (EnsoData.EnsoLevelType)i;
            //    if (diff == EnsoData.EnsoLevelType.Num)
            //    {
            //        continue;
            //    }
            //    if (!filterSettings.EnabledDifficulties.ContainsKey(diff))
            //    {
            //        filterSettings.EnabledDifficulties.Add(diff, true);
            //    }
            //}
            //filterSettings.MinDifficulty = 1;
            //filterSettings.MaxDifficulty = 10;

            SetFilterMenuActive(false);
            InitializeUI();
            TaikoSingletonMonoBehaviour<ControllerManager>.Instance.usedType.type = ControllerManager.ControllerType.Keyboard;
        }

        void Update()
        {
            TaikoSingletonMonoBehaviour<ControllerManager>.Instance.usedType.type = ControllerManager.ControllerType.Keyboard;
        }

        static public int previousIndex = -1;
        public void SongStarted()
        {
            previousIndex = songSelectManager.SelectedSongIndex;
            SaveSettingsManager.SaveLatestSettings();
        }

        //void Update()
        //{
        //    if (gameObject.activeInHierarchy)
        //    {
        //        if (HasFilterChanged())
        //        {
        //            FilterSongList();
        //        }
        //    }
        //}

        //private bool HasFilterChanged()
        //{
        //    // Only set hasChanged to true, but look through everything to find changes
        //    bool hasChanged = false;
        //    if (playlistButton != null)
        //    {
        //        if (playlistButton.HasChanged())
        //        {
        //            hasChanged = true;
        //        }
        //    }

        //    return hasChanged;
        //}

        public void SetFilterMenuActive(bool isActive)
        {
            realParent.SetActive(isActive);
        }

        private void InitializeUI()
        {
            // What elements are planned for this mod?

            // A dropdown for selecting a playlist
            //  - Also some way to create a new platlist in-game
            //  - This would include:
            //     - Naming the playlist
            //     - Reordering the playlists
            //     - Adding songs to the playlist
            //     - Reordering the songs in the playlist

            // A slider for Min/Max difficulty (I'd like for this to be sorta 2 sliders in 1 if possible
            // A bunch of checkboxes for Crown (None, Silver, Gold, Rainbow)
            //  - This could probably also be a slider, assuming people would probably be wanting None-Silver, or None-Gold, and not something like None AND Rainbow
            // A bunch of checkboxes for Genre
            //  - This can't be a slider, so shouldn't be a slider
            // A text field for Song Search

            // Maybe a slider for min/max accuracy?

            // Maybe a section for sorting?
            // This could include stuff like:
            // Alphabetical (Title) sorting
            // Alphabetical (Subtitle) sorting
            // Difficulty (Max of Oni/Ura) sorting
            // Accuracy sorting
            // Accuracy then Difficulty sorting
            // It'd be neat to be able to sort by number of plays, but I don't think those stats are saved in TakoTako
            try
            {

                parent = UnityObjectUtility.CreateImage("FilterBackground", new Color32(0, 0, 0, (int)(255 * 0.698f)), new Rect(0, 0, 1920, 1080), realParent.transform);

                InitializeGenreFilter();

                InitializeCrownFilter();

                InitializeDifficultyFilter();

                InitializeDifficultySliders();

                InitializeSetDefaultButton();

                InitializeBonusFilterButton();

                InitializeFavoriteFilterButton();

                for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
                {
                    var genre = (EnsoData.SongGenre)i;
                    if (GenreFilterButtons.ContainsKey(genre))
                    {
                        AssetUtility.ChangeButtonTransparency(GenreFilterButtons[genre], SaveSettingsManager.filterSettings.GetGenre(genre));
                    }
                }

                var searchInput = AssetUtility.CreateInputField(parent, "SearchInput", new Rect(100, 100, 1000, 200));
                inputField = searchInput.GetComponent<TMP_InputField>();
                inputField.onValueChanged.AddListener((string x) => SearchInputChanged(x));
                inputField.onDeselect.AddListener((string x) => ActivateInputField());

                DirectoryInfo playlistDirInfo = new DirectoryInfo(Plugin.Instance.ConfigPlaylistLocation.Value);
                var playlistFiles = playlistDirInfo.GetFiles("*.json", SearchOption.AllDirectories).ToList();

                playlistDataObjects = new List<PlaylistData>();
                for (int i = 0; i < playlistFiles.Count; i++)
                {
                    var playlistDataObject = new PlaylistData(playlistFiles[i].FullName);
                    playlistDataObjects.Add(playlistDataObject);
                }

                playlistDataObjects.Sort((x, y) => x.Order > y.Order ? 1 : -1);


                List<string> playlistOptions = new List<string>() { "None" };
                for (int i = 0; i < playlistDataObjects.Count; i++)
                {
                    playlistOptions.Add(playlistDataObjects[i].Name);
                }

                var dropdownObject = AssetUtility.CreateDropdown(parent, "Playlist", new Rect(1500, 500, 300, 100), playlistOptions);
                var dropdown = dropdownObject.GetOrAddComponent<TMP_Dropdown>();
                dropdown.onValueChanged.AddListener((int i) => PlaylistDropdownChanged(i));



                List<string> sortOptions = new List<string>();
                foreach (var item in Enum.GetValues(typeof(SortType)))
                {
                    sortOptions.Add(item.ToString());
                }

                var sortDropdownObject = AssetUtility.CreateDropdown(parent, "Sort", new Rect(1500, 300, 300, 100), sortOptions);
                var sortDropdown = sortDropdownObject.GetOrAddComponent<TMP_Dropdown>();
                sortDropdown.onValueChanged.AddListener((int i) => SortDropdownChanged(i));

                //var playlistObject = UnityObjectUtility.CreateCycleButton("PlaylistCycle", playlistOptions, playlistData, new Rect(50, 400, 500, 50), parent.transform);
                //playlistButton = playlistObject.GetComponent<CycleButton>();


                //List<string> sortingOptions = new List<string>()
                //{
                //    "Default",
                //    "Difficulty",
                //    "Accuracy",
                //    "Difficulty -> Accuracy",
                //    "Alphabetical (Title)",
                //    "Alphabetical (Subtitle)",
                //};

                //List<string> sortingData = new List<string>()
                //{

                //};

                //var sortingObject = UnityObjectUtility.CreateCycleButton("SortingCycle", sortingOptions, sortingData, new Rect(50, 325, 500, 50), parent.transform);
                //sortingButton = playlistObject.GetComponent<CycleButton>();

                numSongsDisplay = AssetUtility.CreateTextChild(parent, "NumSongs", new Rect(1700, 1000, 100, 100), "x/y").GetOrAddComponent<TextMeshProUGUI>();

                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
                var titleFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
                var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.KanbanSelect);
                AssetUtility.SetTextFontAndMaterial(numSongsDisplay, titleFont, titleFontMaterial);
                AssetUtility.SetTextAlignment(numSongsDisplay, HorizontalAlignmentOptions.Right);

                //UnityObjectUtility.CreateDropdown("PlaylistDropdown", dropdownOptions, new Rect(50, 400, 500, 200), parent.transform);

                var resetObject = AssetUtility.CreateButton(parent, "Reset", new Rect(1800, 100, 100, 50), "Reset", new Color32(255, 255, 255, 255));
                var resetButton = resetObject.GetOrAddComponent<UIButton>();
                resetButton.onClick.AddListener(() => ResetFilters());

                UpdateUiElements();
            }
            catch (Exception e)
            {
                Plugin.LogError(e.Message);
            }
        }

        private void ResetFilters()
        {
            SaveSettingsManager.filterSettings.ResetValues();
            SaveSettingsManager.sortSettings.ResetValues();

            UpdateUiElements();

            FilterSongList();
        }

        /// <summary>
        /// This updates the filter ui elements to what the current settings are.
        /// </summary>
        private void UpdateUiElements()
        {
            inputField.text = "";

            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var iGenre = (EnsoData.SongGenre)i;
                if (GenreFilterButtons.ContainsKey(iGenre))
                {
                    AssetUtility.ChangeButtonTransparency(GenreFilterButtons[iGenre], SaveSettingsManager.filterSettings.GetGenre(iGenre));
                }
            }
            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                var iCrown = (DataConst.CrownType)i;
                if (CrownFilterButtons.ContainsKey(iCrown))
                {
                    AssetUtility.ChangeButtonTransparency(CrownFilterButtons[iCrown], SaveSettingsManager.filterSettings.GetCrown(iCrown));
                }
            }
            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var iDiff = (EnsoData.EnsoLevelType)i;
                if (DifficultyFilterButtons.ContainsKey(iDiff))
                {
                    AssetUtility.ChangeButtonTransparency(DifficultyFilterButtons[iDiff], SaveSettingsManager.filterSettings.GetDifficulty(iDiff));
                }
            }

            MinDifficulty.value = SaveSettingsManager.filterSettings.MinDifficulty;
            MaxDifficulty.value = SaveSettingsManager.filterSettings.MaxDifficulty;

            AssetUtility.ChangeButtonTransparency(BonusButton.gameObject, SaveSettingsManager.filterSettings.Bonus);
            AssetUtility.ChangeButtonTransparency(FavoriteButton.gameObject, SaveSettingsManager.filterSettings.Favorite);
        }

        private void InitializeGenreFilter()
        {
            var genreParent = AssetUtility.CreateEmptyObject(parent, "GenreFilters", new Vector2(200, 900));

            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var genre = (EnsoData.SongGenre)i;
                if (!GenreFilterButtons.ContainsKey(genre))
                {
                    GameObject buttonObject = null;
                    Vector2 rect = new Vector2(50, 100);
                    int yDifference = 50;
                    Color32 color = new Color32(0, 0, 0, 255);
                    Sprite sprite = null;
                    string genreText = string.Empty;
                    switch (genre)
                    {
                        case EnsoData.SongGenre.Pops:
                            rect = new Vector2(rect.x, rect.y);
                            var genreObj = GameObject.Find("Genre1");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Anime:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 1));
                            genreObj = GameObject.Find("Genre2");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Vocalo:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 2));
                            genreObj = GameObject.Find("Genre3");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Variety:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 3));
                            genreObj = GameObject.Find("Genre4");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Classic:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 4));
                            genreObj = GameObject.Find("Genre5");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Game:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 5));
                            genreObj = GameObject.Find("Genre6");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Namco:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 6));
                            genreObj = GameObject.Find("Genre7");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Num:
                        case EnsoData.SongGenre.Children:
                        default:
                            continue;
                    }

                    AssetUtility.SetRect(buttonObject, rect);

                    var button = AssetUtility.AddButtonComponent(buttonObject);
                    button.onClick.AddListener(() => ClickGenreButton(genre));
                    button.onRightClick.AddListener(() => RightClickGenreButton(genre));
                    GenreFilterButtons.Add(genre, buttonObject);
                }
            }
        }

        private void InitializeCrownFilter()
        {
            var crownParent = AssetUtility.CreateEmptyObject(parent, "CrownFilters", new Vector2(730, 750));
            var crownScale = crownParent.transform.localScale;
            crownScale = new Vector3(0.4f, 0.4f, 0.4f);
            crownParent.transform.localScale = crownScale;

            var existingCrownObject = GameObject.Find("IconCrown1P");
            var existingCrownAnimation = existingCrownObject.GetComponent<Animator>();
            var existingCrownImage = existingCrownObject.GetComponent<Image>();

            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                var crown = (DataConst.CrownType)i;
                if (!CrownFilterButtons.ContainsKey(crown))
                {
                    Vector2 pos = new Vector2(0, 100);
                    int xDifference = 300;
                    Texture2D tex = null;
                    Sprite sprite = null;
                    string genreText = string.Empty;
                    GameObject buttonObject = null;
                    switch (crown)
                    {
                        case DataConst.CrownType.None:
                            existingCrownAnimation.Play("None");
                            var crownObj = GameObject.Find("IconCrown1");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            var buttonImage = buttonObject.GetComponent<Image>();
                            buttonImage.sprite = existingCrownImage.sprite;
                            AssetUtility.GetChildByName(buttonObject, "Text").GetComponent<TextMeshProUGUI>().text = "No Clear";
                            pos = new Vector2(xDifference * 0, pos.y);
                            break;
                        case DataConst.CrownType.Silver:
                            crownObj = GameObject.Find("IconCrown1");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 1, pos.y);
                            break;
                        case DataConst.CrownType.Gold:
                            crownObj = GameObject.Find("IconCrown2");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 2, pos.y);
                            break;
                        case DataConst.CrownType.Rainbow:
                            crownObj = GameObject.Find("IconCrown3");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 3, pos.y);
                            break;
                        default:
                            continue;
                    }

                    AssetUtility.AddButtonComponent(buttonObject);
                    AssetUtility.SetRect(buttonObject, pos);

                    var button = AssetUtility.AddButtonComponent(buttonObject);
                    button.onClick.AddListener(() => ClickCrownButton(crown));
                    button.onRightClick.AddListener(() => RightClickCrownButton(crown));
                    CrownFilterButtons.Add(crown, buttonObject);
                }
            }
        }

        private void InitializeDifficultyFilter()
        {
            var difficultyParent = AssetUtility.GetOrCreateEmptyObject(parent, "DifficultyFilters", new Vector2(700, 950));
            var difficultyScale = difficultyParent.transform.localScale;
            var scale = 0.5f;
            difficultyScale = new Vector3(scale, scale, scale);
            difficultyParent.transform.localScale = difficultyScale;

            var songFilterSetting = GameObject.Find("SongSelectSub").GetComponent<SongFilterSetting>();

            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var diff = (EnsoData.EnsoLevelType)i;
                if (DifficultyFilterButtons.ContainsKey(diff))
                {
                    continue;
                }
                GameObject icon = null;
                GameObject text;
                Vector2 pos = new Vector2(0, 0);
                int xDifference = 200;
                switch (diff)
                {
                    case EnsoData.EnsoLevelType.Easy:
                        text = GameObject.Find("TextDiff1");
                        pos = new Vector2(xDifference * 0, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Normal:
                        text = GameObject.Find("TextDiff2");
                        pos = new Vector2(xDifference * 1, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Hard:
                        text = GameObject.Find("TextDiff3");
                        pos = new Vector2(xDifference * 2, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Mania:
                        text = GameObject.Find("TextDiff4");
                        pos = new Vector2(xDifference * 3, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Ura:
                        text = GameObject.Find("TextDiff4");
                        pos = new Vector2(xDifference * 4, pos.y);
                        break;
                    default:
                        continue;
                }
                var diffObject = AssetUtility.GetOrCreateEmptyChild(difficultyParent, diff.ToString(), pos);
                icon = AssetUtility.CreateImageChild(diffObject, "IconDiff", Vector2.zero, songFilterSetting.difficultyIconSprites[(int)diff]);
                text = GameObject.Instantiate(text, diffObject.transform);
                text.name = "Text";
                var iconPos = icon.transform.localPosition;
                var textPos = text.transform.localPosition;
                iconPos = new Vector3(-60, iconPos.y - 84, iconPos.z);
                textPos = new Vector3(0, textPos.y, textPos.z);
                icon.transform.localPosition = iconPos;
                text.transform.localPosition = textPos;
                diffObject.transform.localScale = Vector3.one;
                icon.transform.localScale = new Vector3(2, 2, 2);
                text.transform.localScale = Vector3.one;

                var button = AssetUtility.AddButtonComponent(diffObject);
                button.onClick.AddListener(() => ClickDifficultyButton(diff));
                button.onRightClick.AddListener(() => RightClickDifficultyButton(diff));
                DifficultyFilterButtons.Add(diff, diffObject);
            }
        }

        public void InitializeDifficultySliders()
        {
            var difficultyParent = AssetUtility.GetOrCreateEmptyObject(parent, "DifficultySliderFilters", new Vector2(1500, 200));

            MinDifficulty = AssetUtility.CreateSlider(difficultyParent, "MinDifficulty", new Rect(0, 50, 300, 30), 1, 10).GetComponent<Slider>();
            MaxDifficulty = AssetUtility.CreateSlider(difficultyParent, "MaxDifficulty", new Rect(0, 0, 300, 30), 1, 10).GetComponent<Slider>();

            MinDifficulty.value = 1;
            MaxDifficulty.value = 10;

            MinDifficulty.onValueChanged.AddListener((float f) => DifficultySliderChanged(true));
            MaxDifficulty.onValueChanged.AddListener((float f) => DifficultySliderChanged(false));
        }

        public void InitializeSetDefaultButton()
        {
            var resetObject = AssetUtility.CreateButton(parent, "SetDefault", new Rect(1800, 200, 100, 50), "Set Default", new Color32(255, 255, 255, 255));
            var resetButton = resetObject.GetOrAddComponent<UIButton>();
            resetButton.onClick.AddListener(() => SaveSettingsManager.SaveDefaultSettings());
        }

        public void InitializeBonusFilterButton()
        {
            GameObject bonusObject = GameObject.Find("IconBonus");
            var bonusImage = bonusObject.GetComponent<Image>();

            var bonusFilterObj = AssetUtility.CreateEmptyObject(parent, "BonusFilter", new Vector2(1700, 800));
            AssetUtility.CreateImageChild(bonusFilterObj, "BonusFilterImage", Vector2.zero, bonusImage.sprite);

            bonusFilterObj.transform.localScale = new Vector3(2, 2, 2);

            var button = AssetUtility.AddButtonComponent(bonusFilterObj);
            button.onClick.AddListener(() => ClickBonusFilterButton(button));

            BonusButton = button;
        }

        public void InitializeFavoriteFilterButton()
        {
            // I don't think this works properly tbh
            GameObject favoriteObject = GameObject.Find("IconFavorite1P");
            var favoriteImage = favoriteObject.GetComponent<UnityEngine.UI.Image>();

            var favoriteFilterObj = AssetUtility.CreateEmptyObject(parent, "FavoriteFilter", new Vector2(1700, 750));
            AssetUtility.CreateImageChild(favoriteFilterObj, "FavoriteFilterImage", Vector2.zero, favoriteImage.sprite);

            favoriteFilterObj.transform.localScale = new Vector3(2, 2, 2);

            var button = AssetUtility.AddButtonComponent(favoriteFilterObj);
            button.onClick.AddListener(() => ClickFavoriteFilterButton(button));

            FavoriteButton = button;
        }

        #region UI Events

        EnsoData.SongGenre previousGenreRightClick = EnsoData.SongGenre.Num;
        bool rightClickedGenreOnce = false;
        public void ClickGenreButton(EnsoData.SongGenre genre)
        {
            rightClickedGenreOnce = false;
            previousGenreRightClick = EnsoData.SongGenre.Num;
            Plugin.LogInfo("Button Click: " + genre);
            SaveSettingsManager.filterSettings.SetGenre(genre, !SaveSettingsManager.filterSettings.GetGenre(genre));
            AssetUtility.ChangeButtonTransparency(GenreFilterButtons[genre], SaveSettingsManager.filterSettings.GetGenre(genre));
            FilterSongList();
            ActivateInputField();
        }

        public void RightClickGenreButton(EnsoData.SongGenre genre)
        {
            bool isEnable = true;
            if (previousGenreRightClick == genre)
            {
                isEnable = !rightClickedGenreOnce;
            }
            rightClickedGenreOnce = isEnable;
            previousGenreRightClick = genre;
            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var iGenre = (EnsoData.SongGenre)i;
                if (iGenre != EnsoData.SongGenre.Children)
                {
                    if (iGenre == genre)
                    {
                        SaveSettingsManager.filterSettings.SetGenre(iGenre, isEnable);
                    }
                    else
                    {
                        SaveSettingsManager.filterSettings.SetGenre(iGenre, !isEnable);
                    }
                    AssetUtility.ChangeButtonTransparency(GenreFilterButtons[iGenre], SaveSettingsManager.filterSettings.GetGenre(iGenre));
                }
            }
            FilterSongList();
            ActivateInputField();
        }

        DataConst.CrownType previousCrownClick = DataConst.CrownType.Num;
        bool rightClickedCrownOnce = false;
        public void ClickCrownButton(DataConst.CrownType crown)
        {
            rightClickedCrownOnce = false;
            previousCrownClick = DataConst.CrownType.Num;
            Plugin.LogInfo("Button Click: " + crown);
            SaveSettingsManager.filterSettings.SetCrown(crown, !SaveSettingsManager.filterSettings.GetCrown(crown));
            AssetUtility.ChangeButtonTransparency(CrownFilterButtons[crown], SaveSettingsManager.filterSettings.GetCrown(crown));
            FilterSongList();
            ActivateInputField();
        }
        public void RightClickCrownButton(DataConst.CrownType crown)
        {
            bool isEnable = true;
            if (previousCrownClick == crown)
            {
                isEnable = !rightClickedCrownOnce;
            }
            rightClickedCrownOnce = isEnable;
            previousCrownClick = crown;
            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                var iCrown = (DataConst.CrownType)i;
                if (iCrown != DataConst.CrownType.Off &&
                    iCrown != DataConst.CrownType.Bronze)
                {
                    if (iCrown == crown)
                    {
                        SaveSettingsManager.filterSettings.SetCrown(iCrown, isEnable);
                    }
                    else
                    {
                        SaveSettingsManager.filterSettings.SetCrown(iCrown, !isEnable);
                    }
                    AssetUtility.ChangeButtonTransparency(CrownFilterButtons[iCrown], SaveSettingsManager.filterSettings.GetCrown(iCrown));
                }

            }
            FilterSongList();
            ActivateInputField();
        }

        EnsoData.EnsoLevelType previousDifficultyClick = EnsoData.EnsoLevelType.Num;
        bool rightClickedDifficultyOnce = false;
        public void ClickDifficultyButton(EnsoData.EnsoLevelType diff)
        {
            rightClickedDifficultyOnce = false;
            previousDifficultyClick = EnsoData.EnsoLevelType.Num;
            Plugin.LogInfo("Button Click: " + diff);
            SaveSettingsManager.filterSettings.SetDifficulty(diff, !SaveSettingsManager.filterSettings.GetDifficulty(diff));
            AssetUtility.ChangeButtonTransparency(DifficultyFilterButtons[diff], SaveSettingsManager.filterSettings.GetDifficulty(diff));
            FilterSongList();
            ActivateInputField();
        }
        public void RightClickDifficultyButton(EnsoData.EnsoLevelType diff)
        {
            bool isEnable = true;
            if (previousDifficultyClick == diff)
            {
                isEnable = !rightClickedDifficultyOnce;
            }
            rightClickedDifficultyOnce = isEnable;
            previousDifficultyClick = diff;
            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var iDiff = (EnsoData.EnsoLevelType)i;
                if (iDiff == diff)
                {
                    SaveSettingsManager.filterSettings.SetDifficulty(iDiff, isEnable);
                }
                else
                {
                    SaveSettingsManager.filterSettings.SetDifficulty(iDiff, !isEnable);
                }
                AssetUtility.ChangeButtonTransparency(DifficultyFilterButtons[iDiff], SaveSettingsManager.filterSettings.GetDifficulty(iDiff));
            }
            FilterSongList();
            ActivateInputField();
        }


        public void SearchInputChanged(string newValue)
        {
            Plugin.LogInfo("SearchInput Changed: " + newValue);
            SaveSettingsManager.filterSettings.TextFilter = newValue;
            FilterSongList();
        }

        public void DifficultySliderChanged(bool minChanged)
        {
            // I really hope this doesn't cause an infinite loop
            if (minChanged && MinDifficulty.value > MaxDifficulty.value)
            {
                MaxDifficulty.value = MinDifficulty.value;
            }
            else if (!minChanged && MaxDifficulty.value < MinDifficulty.value)
            {
                MinDifficulty.value = MaxDifficulty.value;
            }
            SaveSettingsManager.filterSettings.MinDifficulty = (int)MinDifficulty.value;
            SaveSettingsManager.filterSettings.MaxDifficulty = (int)MaxDifficulty.value;
            FilterSongList();
        }

        public void ActivateInputField()
        {
            inputField.ActivateInputField();
            inputField.Select();
        }

        public void PlaylistDropdownChanged(int newIndex)
        {
            SaveSettingsManager.filterSettings.PlaylistData = playlistDataObjects[newIndex - 1];
            FilterSongList();
        }

        public void SortDropdownChanged(int newIndex)
        {
            SaveSettingsManager.sortSettings.Sorts.Clear();
            SaveSettingsManager.sortSettings.Sorts.Add((SortType)newIndex);
            FilterSongList();
        }

        public void ClickBonusFilterButton(UIButton button)
        {
            SaveSettingsManager.filterSettings.Bonus = !SaveSettingsManager.filterSettings.Bonus;
            AssetUtility.ChangeButtonTransparency(button.gameObject, SaveSettingsManager.filterSettings.Bonus);
            FilterSongList();
        }

        public void ClickFavoriteFilterButton(UIButton button)
        {
            SaveSettingsManager.filterSettings.Favorite = !SaveSettingsManager.filterSettings.Favorite;
            AssetUtility.ChangeButtonTransparency(button.gameObject, SaveSettingsManager.filterSettings.Favorite);
            FilterSongList();
        }

        #endregion

        public void AssignSongSelectManager(SongSelectManager instance)
        {
            songSelectManager = instance;
        }

        public void InitializeFullSongList(List<SongSelectManager.Song> songList)
        {
            if (FullSongList.Count < songList.Count)
            {
                FullSongList = songList;
            }
            Plugin.Log.LogInfo("AdditionalFilterMenu InitializedSongList");
            FilterSongList();
        }

        private void FilterSongList()
        {
            List<SongSelectManager.Song> filteredSongList = LoadPlaylist(SaveSettingsManager.filterSettings.PlaylistData);

            filteredSongList = Filter.FilterText(filteredSongList, SaveSettingsManager.filterSettings.TextFilter);
            filteredSongList = Filter.FilterGenres(filteredSongList, SaveSettingsManager.filterSettings.EnabledGenres);

            var songFilterDataList = new List<SongFilterData>();
            for (int j = 0; j < (int)EnsoData.EnsoLevelType.Num; j++)
            {
                var jDiff = (EnsoData.EnsoLevelType)j;
                if (!SaveSettingsManager.filterSettings.GetDifficulty(jDiff))
                {
                    continue;
                }
                for (int i = 0; i < filteredSongList.Count; i++)
                {
                    songFilterDataList.Add(new SongFilterData(filteredSongList[i], jDiff));
                }
            }


            songFilterDataList = Filter.FilterCrowns(songFilterDataList, SaveSettingsManager.filterSettings.EnabledCrowns);
            songFilterDataList = Filter.FilterDifficulty(songFilterDataList, SaveSettingsManager.filterSettings.MinDifficulty, SaveSettingsManager.filterSettings.MaxDifficulty);

            songFilterDataList = Filter.FilterBonus(songFilterDataList, SaveSettingsManager.filterSettings.Bonus);
            songFilterDataList = Filter.FilterFavorite(songFilterDataList, SaveSettingsManager.filterSettings.Favorite);

            //filteredSongList = Sort.SortSongList(filteredSongList, sortSettings.Sorts, filterSettings.EnabledDifficulties);
            songFilterDataList = Sort.SortSongList(songFilterDataList, SaveSettingsManager.sortSettings, false);


            UpdateSongList(songFilterDataList, filteredSongList);
        }

        List<SongSelectManager.Song> LoadPlaylist(PlaylistData songData)
        {
            if (songData == null || songData.JsonFilePath == string.Empty)
            {
                return new List<SongSelectManager.Song>(FullSongList);
            }
            List<SongSelectManager.Song> result = new List<SongSelectManager.Song>();

            for (int i = 0; i < songData.Songs.Count; i++)
            {
                for (int j = 0; j < FullSongList.Count; j++)
                {
                    if (FullSongList[j].Id == songData.Songs[i].SongId)
                    {
                        SongSelectManager.Song song = new SongSelectManager.Song(FullSongList[j]);
                        song.TitleKey = "song_" + songData.Songs[i].SongId;
                        song.SubKey = "song_sub_" + songData.Songs[i].SongId;
                        song.RubyKey = "song_detail_" + songData.Songs[i].SongId;
                        // Changing Genres is a little buggy, but mostly works
                        // Once you move the song list once, it works just fine
                        song.SongGenre = songData.Songs[i].GenreNo;

                        song.Order = i;

                        // ListGenre is not used anywhere it seems
                        //FullSongList[j].ListGenre = songData.Songs[i].GenreNo;
                        song.DLC = songData.Songs[i].IsDlc;
                        result.Add(song);
                        break;
                    }
                }
            }

            return result;
        }

        //void UpdateSongList(List<SongSelectManager.Song> newList)
        //{
        //    if (FullSongList.Count <= songSelectManager.SelectedSongIndex)
        //    {
        //        Plugin.LogError("FullSongList.Count " + FullSongList.Count + " <= songSelectManager.SelectedSongIndex " + songSelectManager.SelectedSongIndex);
        //    }
        //    var prevSongId = songSelectManager.SongList[songSelectManager.SelectedSongIndex].Id;

        //    if (newList.Count == 0)
        //    {
        //        songSelectManager.SongList = new List<SongSelectManager.Song>(FullSongList);
        //    }
        //    else
        //    {
        //        songSelectManager.SongList = new List<SongSelectManager.Song>(newList);
        //    }

        //    int newSongIndex = 0;
        //    for (int i = 0; i < newList.Count; i++)
        //    {
        //        if (newList[i].Id == prevSongId)
        //        {
        //            newSongIndex = i;
        //            break;
        //        }
        //    }

        //    var currentCueSheetName = songSelectManager.songPlayer.CueSheetName;
        //    if (songSelectManager.SongList.Count <= newSongIndex)
        //    {
        //        Plugin.LogError("songSelectManager.SongList.Count <= newSongIndex");
        //    }
        //    if (songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex)
        //    {
        //        Plugin.LogError("songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex");
        //    }
        //    var currentSongSheetName = songSelectManager.bgmCueSheets[songSelectManager.SongList[newSongIndex].PreviewIndex];

        //    if (currentCueSheetName != currentSongSheetName)
        //    {
        //        songSelectManager.playingSongIndex = -1;
        //        songSelectManager.isSongLoadRequested = true;
        //        songSelectManager.songPlayer.Stop(true);
        //        songSelectManager.isSongPlaying = false;
        //    }

        //    songSelectManager.SelectedSongIndex = newSongIndex;
        //    songSelectManager.PlayKanbanMoveAnim(SongSelectManager.KanbanMoveType.Initialize, SongSelectManager.KanbanMoveSpeed.Normal);
        //    songSelectManager.UpdateKanbanSurface(false);
        //    songSelectManager.UpdateSortBarSurface(true);

        //    songSelectManager.oniUraChangeTimeCount = 0f;
        //    songSelectManager.kanbans[0].DiffCourseChangeAnim.Play("ChangeMania", 0, 1f);
        //    songSelectManager.UpdateScoreDisplay();
        //}

        void UpdateSongList(List<SongFilterData> newList, List<SongSelectManager.Song> filteredList)
        {
            if (FullSongList.Count <= songSelectManager.SelectedSongIndex)
            {
                //Plugin.LogError("FullSongList.Count " + FullSongList.Count + " <= songSelectManager.SelectedSongIndex " + songSelectManager.SelectedSongIndex);
            }
            //if (songSelectManager.SelectedSongIndex >= songSelectManager.SongList.Count)
            //{
            //    songSelectManager.SelectedSongIndex = 0;
            //}

            // TODO: Make sure when a song is removed from the previous play, that the song list goes to that position after the song is removed
            var prevSongId = songSelectManager.SongList[songSelectManager.SelectedSongIndex].Id;
            //Plugin.LogInfo("prevSongId: " + prevSongId);
            //Plugin.LogInfo("songSelectManager.SelectedSongIndex: " + songSelectManager.SelectedSongIndex);

            if (newList.Count == 0)
            {
                songSelectManager.SongList = new List<SongSelectManager.Song>(FullSongList);
            }
            else
            {
                songSelectManager.SongList = new List<SongSelectManager.Song>();
                for (int i = 0; i < newList.Count; i++)
                {
                    songSelectManager.SongList.Add(filteredList.Find((x) => x.Id == newList[i].SongId));
                }
            }

            int newSongIndex = Mathf.Max(previousIndex, 0);
            for (int i = 0; i < songSelectManager.SongList.Count; i++)
            {
                if (songSelectManager.SongList[i].Id == prevSongId)
                {
                    newSongIndex = i;
                    break;
                }
            }

            var currentCueSheetName = songSelectManager.songPlayer.CueSheetName;
            if (songSelectManager.SongList.Count <= newSongIndex)
            {
                Plugin.LogError("songSelectManager.SongList.Count <= newSongIndex");
                newSongIndex = 0;
            }
            if (songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex)
            {
                Plugin.LogError("songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex");
            }
            var currentSongSheetName = songSelectManager.bgmCueSheets[songSelectManager.SongList[newSongIndex].PreviewIndex];

            if (currentCueSheetName != currentSongSheetName)
            {
                songSelectManager.playingSongIndex = -1;
                songSelectManager.isSongLoadRequested = true;
                songSelectManager.songPlayer.Stop(true);
                songSelectManager.isSongPlaying = false;
            }

            songSelectManager.SelectedSongIndex = newSongIndex;
            songSelectManager.PlayKanbanMoveAnim(SongSelectManager.KanbanMoveType.Initialize, SongSelectManager.KanbanMoveSpeed.Normal);
            songSelectManager.UpdateKanbanSurface(false);
            songSelectManager.UpdateSortBarSurface(true);

            songSelectManager.oniUraChangeTimeCount = 0f;
            songSelectManager.kanbans[0].DiffCourseChangeAnim.Play("ChangeMania", 0, 1f);
            songSelectManager.UpdateScoreDisplay();

            UpdateQuickJumpValues(newList, SaveSettingsManager.sortSettings.PrimarySort);

            //if (numSongsDisplay == null)
            //{
            //    Plugin.LogInfo("numSongsDisplay == null");
            //}
            //if (songSelectManager == null)
            //{
            //    Plugin.LogInfo("songSelectManager == null");
            //}
            //else if (songSelectManager.SongList == null)
            //{
            //    Plugin.LogInfo("songSelectManager.SongList == null");
            //}
            //if (FullSongList == null)
            //{
            //    Plugin.LogInfo("FullSongList == null");
            //}


            if (numSongsDisplay != null && songSelectManager != null && songSelectManager.SongList != null && FullSongList != null)
            {
                numSongsDisplay.text = songSelectManager.SongList.Count + "/" + FullSongList.Count;
            }
            Plugin.LogInfo("UpdateSongList Finished");
        }

        void UpdateQuickJumpValues(List<SongFilterData> songs, SortType primarySort)
        {
            Plugin.LogInfo("UpdateQuickJumpValues Start");
            switch (primarySort)
            {
                case SortType.Difficulty:
                    // Difficulty 1-10
                    var numCategories = 11;
                    songSelectManager.CategoryTopSongIndex = new int[numCategories];
                    songSelectManager.CategorySongsNum = new int[numCategories];
                    int num = 0;
                    for (int i = 0; i < numCategories; i++)
                    {
                        songSelectManager.CategoryTopSongIndex[i] = num;
                        songSelectManager.CategorySongsNum[i] = songs.Count((x) => x.Star == (i + 1));
                        num += songSelectManager.CategorySongsNum[i];
                    }
                    break;
                case SortType.AlphabeticalTitle:
                case SortType.AlphabeticalSubtitle:
                case SortType.AlphabeticalSongId:
                    // Split into 10 parts
                    numCategories = 11;
                    songSelectManager.CategoryTopSongIndex = new int[numCategories];
                    songSelectManager.CategorySongsNum = new int[numCategories];
                    num = 0;
                    for (int i = 0; i < numCategories; i++)
                    {
                        songSelectManager.CategoryTopSongIndex[i] = num;
                        songSelectManager.CategorySongsNum[i] = songs.Count() / numCategories;
                        num += songSelectManager.CategorySongsNum[i];
                    }
                    break;
                case SortType.Accuracy:
                    numCategories = 11;
                    songSelectManager.CategoryTopSongIndex = new int[numCategories];
                    songSelectManager.CategorySongsNum = new int[numCategories];
                    num = 0;
                    for (int i = 0; i < numCategories; i++)
                    {
                        songSelectManager.CategoryTopSongIndex[i] = num;
                        songSelectManager.CategorySongsNum[i] = songs.Count((x) => GetAccCategory(x.Accuracy) == i);
                        num += songSelectManager.CategorySongsNum[i];
                    }
                    break;
                default:
                    // Genre
                    numCategories = 8;
                    songSelectManager.CategoryTopSongIndex = new int[numCategories];
                    songSelectManager.CategorySongsNum = new int[numCategories];
                    num = 0;
                    for (int i = 0; i < numCategories; i++)
                    {
                        songSelectManager.CategoryTopSongIndex[i] = num;
                        songSelectManager.CategorySongsNum[i] = songs.Count((x) => x.GenreNo == i);
                        num += songSelectManager.CategorySongsNum[i];
                    }
                    break;
            }
            Plugin.LogInfo("UpdateQuickJumpValues Finish");
        }

        public static int GetAccCategory(float acc)
        {
            float minAcc = 0.0f;
            float maxAcc = 77.5f;
            for (int i = 0; i < 11; i++)
            {
                switch (i)
                {
                    case 0:
                        minAcc = 0.0f;
                        maxAcc = 77.5f;
                        break;
                    case 1:
                        minAcc = 77.5f;
                        maxAcc = 80.0f;
                        break;
                    case 2:
                        minAcc = 80.0f;
                        maxAcc = 82.5f;
                        break;
                    case 3:
                        minAcc = 82.5f;
                        maxAcc = 85.0f;
                        break;
                    case 4:
                        minAcc = 85.0f;
                        maxAcc = 87.5f;
                        break;
                    case 5:
                        minAcc = 87.5f;
                        maxAcc = 90.0f;
                        break;
                    case 6:
                        minAcc = 90.0f;
                        maxAcc = 92.5f;
                        break;
                    case 7:
                        minAcc = 92.5f;
                        maxAcc = 95.0f;
                        break;
                    case 8:
                        minAcc = 95.0f;
                        maxAcc = 97.5f;
                        break;
                    case 9:
                        minAcc = 97.5f;
                        maxAcc = 100.00f;
                        break;
                    case 10:
                        minAcc = 100.0f;
                        maxAcc = 101.0f;
                        break;
                }
                if (acc >= minAcc && acc < maxAcc)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
