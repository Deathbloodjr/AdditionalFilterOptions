using AdditionalFilterOptions.Settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterOptionsPatch
    {
        const string FilterObjectName = "AdditionalFilterMenu";

        public static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();

        static AdditionalFilterMenu filterMenu;

        public static bool isFirstStartup = true;

        private static bool pressedF3 = false;

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_Start_Postfix(SongSelectManager __instance)
        {
            GameObject additionalFilterMenu = GameObject.Find(FilterObjectName);
            if (additionalFilterMenu == null)
            {
                additionalFilterMenu = new GameObject(FilterObjectName);
            }

            filterMenu = additionalFilterMenu.GetComponent<AdditionalFilterMenu>();
            if (filterMenu == null)
            {
                filterMenu = additionalFilterMenu.AddComponent<AdditionalFilterMenu>();
                filterMenu.AssignSongSelectManager(__instance);
            }
        }

        [HarmonyPatch(typeof(SongSelectManager), "SortSongList", new Type[] { typeof(DataConst.SongSortCourse), typeof(DataConst.SongSortType), typeof(DataConst.SongFilterType), typeof(DataConst.SongFilterTypeFavorite) })]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_SortSongList_Postfix(SongSelectManager __instance, DataConst.SongSortCourse sortDifficulty, DataConst.SongSortType sortType, DataConst.SongFilterType filterType, DataConst.SongFilterTypeFavorite filterTypeFavorite)
        {
            if (filterMenu != null)
            {
                filterMenu.InitializeFullSongList(new List<SongSelectManager.Song>(__instance.SongList));
            }
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.UpdateSongSelect))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_UpdateSongSelect_Prefix(SongSelectManager __instance)
        {
            bool currentlyPressingF3 = Input.GetKey(KeyCode.F3);
            if (currentlyPressingF3 && !pressedF3)
            {
                pressedF3 = true;
                ToggleFilterMenuActive();
            }
            else if (!currentlyPressingF3)
            {
                pressedF3 = false;
            }

            return !isActive;
        }

        static bool isActive = false;
        private static void SetFilterMenuActive(bool changeActive)
        {
            isActive = changeActive;
            filterMenu.SetFilterMenuActive(changeActive);
            List<string> ObjectsToDisableRaycasting = new List<string>()
            {
                "mid_canvas",
                "fg_canvas(ModeSelect)",
                "input_guide_canvas",
            };
            for (int i = 0; i < ObjectsToDisableRaycasting.Count; i++)
            {
                var gameObject = GameObject.Find(ObjectsToDisableRaycasting[i]);
                if (gameObject != null)
                {
                    var rayCaster = gameObject.GetComponent<GraphicRaycaster>();
                    if (rayCaster != null)
                    {
                        rayCaster.enabled = !changeActive;
                    }
                }
            }
        }

        private static void ToggleFilterMenuActive()
        {
            var isActive = filterMenu.gameObject.activeInHierarchy;
            if (isActive)
            {
                SaveSettingsManager.SaveLatestSettings();
            }
            SetFilterMenuActive(!isActive);
        }

        private static bool IsFilterActive()
        {
            return filterMenu.gameObject.activeInHierarchy;
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.UpdateSortCategoryInfo))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_UpdateSortCategoryInfo_Prefix(SongSelectManager __instance)
        {
            return false;
        }

        [HarmonyPatch(typeof(CourseSelect))]
        [HarmonyPatch(nameof(CourseSelect.EnsoConfigSubmit))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool CourseSelect_EnsoConfigSubmit_Prefix(CourseSelect __instance)
        {
            filterMenu.SongStarted();
            return true;
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.SkipSongNext))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_SkipSongNext_Prefix(SongSelectManager __instance)
        {
            if (__instance.CurrentState != SongSelectManager.State.SongSelect)
            {
                return false;
            }
            if (__instance.isKanbanMoving)
            {
                return false;
            }
            if (__instance.SongList.Count <= 0)
            {
                return false;
            }

            var num = 0;
            for (int i = __instance.SortCategoryNum - 1; i >= 0; i--)
            {
                if (__instance.SelectedSongIndex >= __instance.CategoryTopSongIndex[i])
                {
                    num = (i + 1) % __instance.SortCategoryNum;
                    break;
                }
            }

            Plugin.Log.LogInfo("__instance.SelectedSongIndex Before: " + __instance.SelectedSongIndex);
            __instance.SelectedSongIndex = __instance.CategoryTopSongIndex[num] % __instance.SongList.Count;
            Plugin.Log.LogInfo("__instance.SelectedSongIndex After: " + __instance.SelectedSongIndex);
            __instance.PlayKanbanMoveAnim(SongSelectManager.KanbanMoveType.Initialize, SongSelectManager.KanbanMoveSpeed.Normal);
            __instance.UpdateKanbanSurface(false);
            __instance.UpdateSortBarSurface(true);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("return", false, false);
            __instance.isSongLoadRequested = true;
            __instance.songPlayer.Stop(true);
            __instance.isSongPlaying = false;
            __instance.oniUraChangeTimeCount = 0f;
            __instance.kanbans[0].DiffCourseChangeAnim.Play("ChangeMania", 0, 1f);
            __instance.UpdateScoreDisplay();

            return false;
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.SkipSongPrev))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_SkipSongPrev_Prefix(SongSelectManager __instance)
        {
            if (__instance.CurrentState != SongSelectManager.State.SongSelect)
            {
                return false;
            }
            if (__instance.isKanbanMoving)
            {
                return false;
            }
            if (__instance.SongList.Count <= 0)
            {
                return false;
            }

            var num = 0;
            for (int i = __instance.SortCategoryNum - 1; i >= 0; i--)
            {
                if (__instance.SelectedSongIndex >= __instance.CategoryTopSongIndex[i])
                {
                    num = (i - 1 + __instance.SortCategoryNum) % __instance.SortCategoryNum;
                    break;
                }
            }

            Plugin.Log.LogInfo("__instance.SelectedSongIndex Before: " + __instance.SelectedSongIndex);
            __instance.SelectedSongIndex = (__instance.CategoryTopSongIndex[num] + __instance.CategorySongsNum[num] - 1 + __instance.SongList.Count) % __instance.SongList.Count;
            Plugin.Log.LogInfo("__instance.SelectedSongIndex After: " + __instance.SelectedSongIndex);
            __instance.PlayKanbanMoveAnim(SongSelectManager.KanbanMoveType.Initialize, SongSelectManager.KanbanMoveSpeed.Normal);
            __instance.UpdateKanbanSurface(false);
            __instance.UpdateSortBarSurface(true);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("return", false, false);
            __instance.isSongLoadRequested = true;
            __instance.songPlayer.Stop(true);
            __instance.isSongPlaying = false;
            __instance.oniUraChangeTimeCount = 0f;
            __instance.kanbans[0].DiffCourseChangeAnim.Play("ChangeMania", 0, 1f);
            __instance.UpdateScoreDisplay();

            return false;
        }
    }
}
