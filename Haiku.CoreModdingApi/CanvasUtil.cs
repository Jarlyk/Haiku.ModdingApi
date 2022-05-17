using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Haiku.CoreModdingApi
{
    // Canvas helper Class, mostly from hk-modding API https://github.com/hk-modding/api/blob/master/Assembly-CSharp/CanvasUtil.cs
    public static class CanvasUtil
    {
        /// <summary>
        ///     Creates a base panel for UI elements
        /// </summary>
        /// <param name="parent">Parent Game Object under which this panel will be held</param>
        /// <param name="rd">Rectangle data for this panel</param>
        /// <returns></returns>
        public static GameObject CreateBasePanel(GameObject parent, RectData rd)
        {
            GameObject basePanel = new GameObject();
            if (parent != null)
            {
                basePanel.transform.SetParent(parent.transform);
                basePanel.transform.localScale = new Vector3(1, 1, 1);
            }

            basePanel.AddComponent<CanvasRenderer>();
            AddRectTransform(basePanel, rd);
            return basePanel;
        }

        /// <summary>
        ///     Transforms the RectData into a RectTransform for the GameObject.
        /// </summary>
        /// <param name="go">GameObject to which this rectdata should be put into.</param>
        /// <param name="rd">Rectangle Data</param>
        public static void AddRectTransform(GameObject go, RectData rd)
        {
            // Create a rectTransform
            // Set the total size of the content
            // all you need to know is, 
            // --

            // sizeDelta is size of the difference of the anchors multiplied by screen size so
            // the sizeDelta width is actually = ((anchorMax.x-anchorMin.x)*screenWidth) + sizeDelta.x
            // so assuming a streched horizontally rectTransform on a 1920 screen, this would be
            // ((1-0)*1920)+sizeDelta.x
            // 1920 + sizeDelta.x
            // so if you wanted a 100pixel wide box in the center of the screen you'd do -1820, height as 1920+-1820 = 100
            // and if you wanted a fullscreen wide box, its just 0 because 1920+0 = 1920
            // the same applies for height

            // anchorPosition is basically an offset to the center of the anchors multiplies by screen size so
            // a 0.5,0.5 min and 0.5,0.5 max, would put the anchor in the middle of the screen but anchorPosition just offsets that 
            // i.e on a 1920x1080 screen
            // anchorPosition 100,100 would do (1920*0.5)+100,(1080*0.5)+100, so 1060,640

            // ANCHOR MIN / MAX
            // --
            // 0,0 = bottom left
            // 0,1 = top left
            // 1,0 = bottom right
            // 1,1 = top right
            // --


            // The only other rects I'd use are
            // anchorMin = 0.0, yyy anchorMax = 1.0, yyy (strech horizontally) y = 0.0 is bottom, y = 0.5 is center, y = 1.0 is top
            // anchorMin = xxx, 0.0 anchorMax = xxx, 1.0 (strech vertically) x = 0.0 is left, x = 0.5 is center, x = 1.0 is right
            // anchorMin = 0.0, 0.0 anchorMax = 1.0, 1.0 (strech to fill)
            // --
            // technically you can anchor these anywhere on the screen
            // you can even use negative values to float something offscreen

            // as for the pivot, the pivot determines where the "center" of the rect is which is useful if you want to rotate something by its corner, note that this DOES offset the anchor positions
            // i.e. with a 100x100 square, setting the pivot to be 1,1 will put the top right of the square at the anchor position (-50,-50 from its starting position)

            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMax = rd.AnchorMax;
            rt.anchorMin = rd.AnchorMin;
            rt.pivot = rd.AnchorPivot;
            rt.sizeDelta = rd.RectSizeDelta;
            rt.anchoredPosition = rd.AnchorPosition;
        }

        /// <summary>
        ///     Creates a Canvas Element set up to work with Haiku's Screenspace
        /// </summary>
        /// <param name="sortingOrder">The sorting Order of the Canvas</param>
        /// <returns>GameObject with Raycaster, CanvasScaler and CanvasAspectScaler</returns>
        public static GameObject CreateCanvas(int sortingOrder = 0)
        {
            GameObject c = new GameObject();
            Canvas cv = c.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = sortingOrder;
            CanvasScaler cs = c.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referencePixelsPerUnit = 16f;
            cs.referenceResolution = new Vector2(384f, 216f);
            c.AddComponent<CanvasAspectScaler>();
            c.AddComponent<GraphicRaycaster>();
            return c;
        }

        /// <summary>
        ///     Creates a Text Object
        /// </summary>
        /// <param name="parent">The GameObject that this text will be put into.</param>
        /// <param name="text">The text that will be shown with this object</param>
        /// <param name="fontSize">The text's font size.</param>
        /// <param name="textAnchor">The location within the rectData where the text anchor should be.</param>
        /// <param name="rectData">Rectangle Data to describe the Text Panel.</param>
        /// <param name="font">The Font to use</param>
        /// <returns>The Panel that the Text Component is attached to</returns>
        public static GameObject CreateTextPanel(GameObject parent, string text, int fontSize, TextAnchor textAnchor,
            RectData rectData, Font font)
        {
            GameObject panel = CreateBasePanel(parent, rectData);

            Text textObj = panel.AddComponent<Text>();
            textObj.font = font;

            textObj.text = text;
            textObj.supportRichText = true;
            textObj.fontSize = fontSize;
            textObj.alignment = textAnchor;
            return panel;
        }

        /// <summary>
        ///     Creates an Image Panel
        /// </summary>
        /// <param name="parent">The Parent GameObject for this image.</param>
        /// <param name="sprite">The Image/Sprite to use</param>
        /// <param name="rectData">The rectangle description for this sprite to inhabit</param>
        /// <returns></returns>
        public static GameObject CreateImagePanel(GameObject parent, Sprite sprite, RectData rectData)
        {
            GameObject panel = CreateBasePanel(parent, rectData);

            Image img = panel.AddComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = true;
            return panel;
        }

        /// <summary>
        ///     Creates a Button
        /// </summary>
        /// <param name="parent">The Parent GameObject for this Button</param>
        /// <param name="action">Action to take when Button is clicked</param>
        /// <param name="id">Id passed to the action</param>
        /// <param name="spr">Sprite to use for the button</param>
        /// <param name="text">Text for the button</param>
        /// <param name="font">The font for the Text on the Button</param>
        /// <param name="fontSize">Size of the Text</param>
        /// <param name="textAnchor">Where to Anchor the text within the button</param>
        /// <param name="rectData">The rectangle description for this button</param>
        /// <param name="extraSprites">
        ///     Size 3 array of other sprite states for the button.  0 = Highlighted Sprite, 1 = Pressed
        ///     Sprited, 2 = Disabled Sprite
        /// </param>
        /// <returns></returns>
        public static GameObject CreateButton(GameObject parent, Action<int> action, int id, Sprite spr, string text,
            Font font, int fontSize, TextAnchor textAnchor, RectData rectData,params Sprite[] extraSprites)
        {
            GameObject panel = CreateBasePanel(parent, rectData);

            CreateTextPanel(panel, text, fontSize, textAnchor, rectData, font);

            Image img = panel.AddComponent<Image>();
            img.sprite = spr;

            Button button = panel.AddComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(delegate { action(id); });

            if (extraSprites.Length == 3)
            {
                button.transition = Selectable.Transition.SpriteSwap;
                button.targetGraphic = img;
                SpriteState sprState = new SpriteState
                {
                    highlightedSprite = extraSprites[0],
                    pressedSprite = extraSprites[1],
                    disabledSprite = extraSprites[2]
                };

                button.spriteState = sprState;
            }
            else
            {
                button.transition = Selectable.Transition.None;
            }

            return panel;
        }

        /// <summary>
        ///     Rectangle Helper Class
        /// </summary>
        public class RectData
        {
            /// <summary>
            ///     Describes on of the X,Y Positions of the Element
            /// </summary>
            public Vector2 AnchorMax;

            /// <summary>
            ///     Describes on of the X,Y Positions of the Element
            /// </summary>
            public Vector2 AnchorMin;

            /// <summary>
            /// </summary>
            public Vector2 AnchorPivot;

            /// <summary>
            ///     Relative Offset Postion where Element is anchored as compared to Min / Max
            /// </summary>
            public Vector2 AnchorPosition;

            /// <summary>
            ///     Difference in size of the rectangle as compared to it's parent.
            /// </summary>
            public Vector2 RectSizeDelta;

            /// <inheritdoc />
            /// <summary>
            ///     Describes a Rectangle's relative size, shape, and relative position to it's parent.
            /// </summary>
            /// <param name="sizeDelta">
            ///     sizeDelta is size of the difference of the anchors multiplied by screen size so
            ///     the sizeDelta width is actually = ((anchorMax.x-anchorMin.x)*screenWidth) + sizeDelta.x
            ///     so assuming a streched horizontally rectTransform on a 1920 screen, this would be
            ///     ((1-0)*1920)+sizeDelta.x
            ///     1920 + sizeDelta.x
            ///     so if you wanted a 100pixel wide box in the center of the screen you'd do -1820, height as 1920+-1820 = 100
            ///     and if you wanted a fullscreen wide box, its just 0 because 1920+0 = 1920
            ///     the same applies for height
            /// </param>
            /// <param name="anchorPosition">Relative Offset Postion where Element is anchored as compared to Min / Max</param>
            public RectData(Vector2 sizeDelta, Vector2 anchorPosition)
                : this(sizeDelta, anchorPosition, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f))
            {
            }


            /// <inheritdoc />
            /// <summary>
            ///     Describes a Rectangle's relative size, shape, and relative position to it's parent.
            /// </summary>
            /// <param name="sizeDelta">
            ///     sizeDelta is size of the difference of the anchors multiplied by screen size so
            ///     the sizeDelta width is actually = ((anchorMax.x-anchorMin.x)*screenWidth) + sizeDelta.x
            ///     so assuming a streched horizontally rectTransform on a 1920 screen, this would be
            ///     ((1-0)*1920)+sizeDelta.x
            ///     1920 + sizeDelta.x
            ///     so if you wanted a 100pixel wide box in the center of the screen you'd do -1820, height as 1920+-1820 = 100
            ///     and if you wanted a fullscreen wide box, its just 0 because 1920+0 = 1920
            ///     the same applies for height
            /// </param>
            /// <param name="anchorPosition">Relative Offset Postion where Element is anchored as compared to Min / Max</param>
            /// <param name="min">
            ///     Describes 1 corner of the rectangle
            ///     0,0 = bottom left
            ///     0,1 = top left
            ///     1,0 = bottom right
            ///     1,1 = top right
            /// </param>
            /// <param name="max">
            ///     Describes 1 corner of the rectangle
            ///     0,0 = bottom left
            ///     0,1 = top left
            ///     1,0 = bottom right
            ///     1,1 = top right
            /// </param>
            public RectData(Vector2 sizeDelta, Vector2 anchorPosition, Vector2 min, Vector2 max)
                : this(sizeDelta, anchorPosition, min, max, new Vector2(0.5f, 0.5f))
            {
            }

            /// <summary>
            ///     Describes a Rectangle's relative size, shape, and relative position to it's parent.
            /// </summary>
            /// <param name="sizeDelta">
            ///     sizeDelta is size of the difference of the anchors multiplied by screen size so
            ///     the sizeDelta width is actually = ((anchorMax.x-anchorMin.x)*screenWidth) + sizeDelta.x
            ///     so assuming a streched horizontally rectTransform on a 1920 screen, this would be
            ///     ((1-0)*1920)+sizeDelta.x
            ///     1920 + sizeDelta.x
            ///     so if you wanted a 100pixel wide box in the center of the screen you'd do -1820, height as 1920+-1820 = 100
            ///     and if you wanted a fullscreen wide box, its just 0 because 1920+0 = 1920
            ///     the same applies for height
            /// </param>
            /// <param name="anchorPosition">Relative Offset Postion where Element is anchored as compared to Min / Max</param>
            /// <param name="min">
            ///     Describes 1 corner of the rectangle
            ///     0,0 = bottom left
            ///     0,1 = top left
            ///     1,0 = bottom right
            ///     1,1 = top right
            /// </param>
            /// <param name="max">
            ///     Describes 1 corner of the rectangle
            ///     0,0 = bottom left
            ///     0,1 = top left
            ///     1,0 = bottom right
            ///     1,1 = top right
            /// </param>
            /// <param name="pivot">Controls the location to use to rotate the rectangle if necessary.</param>
            public RectData(Vector2 sizeDelta, Vector2 anchorPosition, Vector2 min, Vector2 max, Vector2 pivot)
            {
                RectSizeDelta = sizeDelta;
                AnchorPosition = anchorPosition;
                AnchorMin = min;
                AnchorMax = max;
                AnchorPivot = pivot;
            }
        }
    }
}
