namespace EasyMaskTool
{
    using UnityEngine;

    public static class StylePresets
    {
        public static GUIStyle SubtitleStyle
        {
            get
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleRight,
                    normal = { textColor = Color.gray },
                    hover = { textColor = Color.gray },
                    margin = new RectOffset(2, 2, 2, 2)
                };
                return style;
            }
        }

        public static GUIStyle ButtonStyle
        {
            get
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white},
                    hover = { textColor = Color.black},
                };
                return style;
            }
        }
    }

}