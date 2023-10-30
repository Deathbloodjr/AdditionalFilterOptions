﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class AssetUtility
    {
        static Dictionary<string, Sprite> LoadedSprites;

        public static Button AddButtonComponent(GameObject newObject)
        {
            newObject.AddComponent<GraphicRaycaster>();
            return newObject.AddComponent<Button>();
        }

        private static GameObject CreateButton(GameObject parent, string name, Rect rect, string text)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);
            var button = newObject.AddComponent<Button>();

            var raycaster = newObject.AddComponent<GraphicRaycaster>();

            var rectTransform = SetRect(newObject, rect);

            var buttonText = CreateTextChild(newObject, "Text", new Rect(0, 0, rect.width, rect.height), "Button").GetOrAddComponent<TextMeshProUGUI>();
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.verticalAlignment = VerticalAlignmentOptions.Middle;
            buttonText.text = text;
            buttonText.color = new Color32(0, 0, 0, 255);

            return newObject;
        }

        public static GameObject CreateButton(GameObject parent, string name, Rect rect, string text, Color32 color)
        {
            var newObject = CreateButton(parent, name, rect, text);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = color;

            var coverImage = CreateImageChild(newObject, "Cover", new Rect(0, 0, rect.width, rect.height), new Color32(0, 0, 0, 0));

            return newObject;
        }

        public static GameObject CreateButton(GameObject parent, string name, Rect rect, string text, Sprite sprite)
        {
            var newObject = CreateButton(parent, name, rect, text);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);
            image.sprite = sprite;

            var coverImage = CreateImageChild(newObject, "Cover", new Rect(0, 0, rect.width, rect.height), new Color32(0, 0, 0, 0));

            return newObject;
        }

        public static void ChangeButtonTransparency(GameObject button, bool isOn)
        {
            var image = button.GetOrAddComponent<Image>();
            var imageColor = image.color;
            if (isOn)
            {
                imageColor.a = 1;
            }
            else
            {
                imageColor.a = 0.5f;
            }
            image.color = imageColor;
        }

        public static void ChangeButtonAndTextTransparency(GameObject button, bool isOn)
        {
            var image = button.GetOrAddComponent<Image>();
            var text = GetChildByName(button, "Text").GetComponent<TextMeshProUGUI>();
            var imageColor = image.color;
            var textColor = text.color;
            if (isOn)
            {
                imageColor.a = 1;
                textColor.a = 1;
            }
            else
            {
                imageColor.a = 0.5f;
                textColor.a = 0.5f;
            }
            image.color = imageColor;
            text.color = textColor;
        }

        public static void ChangeDifficultyButtonAndTextTransparency(GameObject button, bool isOn)
        {
            var image = button.GetOrAddComponent<Image>();
            var text = GetChildByName(button, "Text").GetComponent<TextMeshProUGUI>();
            var imageColor = image.color;
            var textColor = text.color;
            if (isOn)
            {
                imageColor.a = 1;
                textColor.a = 1;
            }
            else
            {
                imageColor.a = 0.5f;
                textColor.a = 0.5f;
            }
            image.color = imageColor;
            text.color = textColor;
        }

        static public GameObject CreateInputField(GameObject parent, string name, Rect rect)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);

            var rectTransform = SetRect(newObject, rect);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);
            var inputField = newObject.AddComponent<TMP_InputField>();
            var raycaster = newObject.AddComponent<GraphicRaycaster>();


            var textArea = CreateEmptyObject(newObject, "Text Area", new Rect(0, 0, rect.width, rect.height));
            var rectMask = textArea.AddComponent<RectMask2D>();

            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
            var titleFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
            var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.KanbanSelect);

            var text = CreateTextChild(textArea, "Text", new Rect(0, 0, rect.width, rect.height), "");
            var textComponent = text.GetComponent<TextMeshProUGUI>();
            textComponent.font = titleFont;
            textComponent.fontMaterial = titleFontMaterial;
            textComponent.alignment = TextAlignmentOptions.Left;

            var placeholder = CreateTextChild(textArea, "Placeholder", new Rect(0, 0, rect.width, rect.height), "Enter text...");
            var placeHolderText = placeholder.GetComponent<TextMeshProUGUI>();

            placeHolderText.font = titleFont;
            placeHolderText.fontMaterial = titleFontMaterial;

            inputField.textViewport = textArea.GetComponent<RectTransform>();
            inputField.textComponent = text.GetComponent<TextMeshProUGUI>();
            inputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();

            return newObject;
        }

        static public Sprite LoadSprite(string spriteFilePath)
        {
            if (LoadedSprites == null)
            {
                LoadedSprites = new Dictionary<string, Sprite>();
            }
            if (LoadedSprites.ContainsKey(spriteFilePath))
            {
                return LoadedSprites[spriteFilePath];
            }
            else if (File.Exists(spriteFilePath))
            {
                LoadedSprites.Add(spriteFilePath, LoadSpriteFromFile(spriteFilePath));
                return LoadedSprites[spriteFilePath];
            }
            // otherwise, the file doesn't exist, log an error, and return null (or hopefully a small transparent sprite
            else
            {
                Plugin.LogError("Could not find file: " + spriteFilePath);
                // Instead of null, could I have this return just a 1x1 transparent sprite or something?

                // Creates a transparent 2x2 texture, and returns that as the sprite
                Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[tex.width * tex.height];
                for (int i = 0; i < fillPixels.Length; i++)
                {
                    fillPixels[i] = fillColor;
                }
                tex.SetPixels(fillPixels);
                tex.Apply();

                Rect rect = new Rect(0, 0, tex.width, tex.height);
                LoadedSprites.Add(spriteFilePath, Sprite.Create(tex, rect, new Vector2(0, 0)));
                return LoadedSprites[spriteFilePath];
            }
        }
        static public GameObject GetOrCreateEmptyObject(GameObject parent, string name, Vector2 position)
        {
            var newObject = GameObject.Find(name);
            if (newObject == null)
            {
                newObject = CreateEmptyObject(parent, name, position);
            }
            return newObject;
        }

        static public GameObject GetOrCreateEmptyChild(GameObject parent, string name, Vector2 position)
        {
            var child = GetChildByName(parent, name);
            if (child == null)
            {
                child = CreateEmptyObject(parent, name, position);
            }
            return child;
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Vector2 position)
        {
            Rect rect = new Rect(position, Vector2.zero);
            return CreateEmptyObject(parent, name, rect);
        }

        public static GameObject GetChildByName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Rect rect)
        {
            GameObject newObject = new GameObject(name);
            if (parent != null)
            {
                newObject.transform.SetParent(parent.transform);
            }
            SetRect(newObject, rect);
            return newObject;
        }


        static public Canvas AddCanvasComponent(GameObject gameObject)
        {
            var canvasObject = gameObject.GetOrAddComponent<Canvas>();
            canvasObject.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.worldCamera = null;
            canvasObject.overrideSorting = true;

            var canvasScalerObject = gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScalerObject.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScalerObject.referenceResolution = new Vector2(1920, 1080);
            canvasScalerObject.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScalerObject.matchWidthOrHeight = 0;

            return canvasObject;
        }



        #region Text

        static public GameObject CreateTextChild(GameObject parent, string name, Rect rect, string text)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var textComponent = newObject.GetOrAddComponent<TextMeshProUGUI>();
            ChangeText(newObject, text);
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMax = 1000;
            textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

            SetRect(newObject, rect);

            return newObject;
        }

        static public void ChangeText(TextMeshProUGUI textComponent, string text)
        {
            if (textComponent == null)
            {
                return;
            }
            textComponent.text = text;
        }

        static public void ChangeText(GameObject gameObject, string text)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();

            ChangeText(textComponent, text);

            return;
        }
        static public void SetTextFontAndMaterial(GameObject gameObject, TMP_FontAsset font, Material material)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            SetTextFontAndMaterial(textComponent, font, material);
        }

        static public void SetTextFontAndMaterial(TextMeshProUGUI text, TMP_FontAsset font, Material material)
        {
            if (text != null)
            {
                text.font = font;
                text.fontSharedMaterial = material;
            }
        }

        static public void SetTextAlignment(GameObject gameObject, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                     VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            SetTextAlignment(textComponent, horizAlignment, vertAlignment);
        }

        static public void SetTextAlignment(TextMeshProUGUI text, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                             VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            if (text != null)
            {
                text.horizontalAlignment = horizAlignment;
                text.verticalAlignment = vertAlignment;
            }
        }

        static public void SetTextColor(GameObject gameObject, Color color)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            textComponent.color = color;
        }

        #endregion


        #region Image

        static public GameObject GetOrCreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var imageChild = GetChildByName(parent, name);
            if (imageChild == null)
            {
                imageChild = CreateImageChild(parent, name, position, spriteFilePath);
            }
            else
            {
                imageChild.GetOrAddComponent<Image>().sprite = LoadSprite(spriteFilePath);
            }
            return imageChild;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Color32 color)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = newObject.GetOrAddComponent<Image>();
            image.color = color;

            return newObject;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, position, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, Sprite sprite)
        {
            Rect rect = new Rect(position, new Vector2(sprite.rect.width, sprite.rect.height));
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Sprite sprite)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = newObject.GetOrAddComponent<Image>();
            image.sprite = sprite;

            return newObject;
        }

        static public void ChangeImageColor(GameObject gameObject, Color32 color)
        {
            var image = GetOrAddImageComponent(gameObject);
            image.color = color;
        }

        static public Image GetOrAddImageComponent(GameObject gameObject)
        {
            var imageObject = gameObject.GetComponent<Image>();
            if (imageObject == null)
            {
                imageObject = gameObject.AddComponent<Image>();
            }

            return imageObject;
        }

        static private Sprite LoadSpriteFromFile(string spriteFilePath)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            if (!File.Exists(spriteFilePath))
            {
                Plugin.Log.LogError("Could not find file: " + spriteFilePath);
                //return null;
            }
            else
            {
                tex.LoadImage(File.ReadAllBytes(spriteFilePath));
            }


            Rect rect = new Rect(0, 0, tex.width, tex.height);
            return Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        static public Image ChangeImageSprite(GameObject gameObject, string spriteFilePath)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, spriteFilePath);
        }

        static public Image ChangeImageSprite(GameObject gameObject, Sprite sprite)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            if (sprite == null)
            {
                return image;
            }
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            return image;
        }



        #endregion


        #region RectTransform

        // This feels kinda repetitive, but I think it's fine
        static public RectTransform SetRect(GameObject gameObject, Vector2 position)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Rect rect)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            return rectTransform;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.pivot = pivot;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect, anchorMin, anchorMax);
            rectTransform.pivot = pivot;
        }

        #endregion
    }

    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T t))
            {
                return t;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
    }
}