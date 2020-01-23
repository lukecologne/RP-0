using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RP0.Unity.Styles
{
    public class RP1_GUIStyles : MonoBehaviour
    {
        public enum ElementTypes
        {
            None,
            Window,
            Box,
            Button,
            Toggle,
            Scrollbar,
            ScrollbarHandle
        }

        [SerializeField]
        private ElementTypes m_ElementType = ElementTypes.None;

        public ElementTypes ElementType
        {
            get { return m_ElementType; }
        }

        public void setImage(Sprite sprite, Image.Type type)
        {
            Image image = GetComponent<Image>();

            if (image == null)
                return;

            image.sprite = sprite;
            image.type = type;
        }

        public void setButton(Sprite normalBackground, Sprite highlightBackground, Sprite activeBackground, Sprite disabledBackground)
        {
            setSelectable(normalBackground, highlightBackground, activeBackground, disabledBackground);
        }

        public void setToggle(Sprite normalBackground, Sprite highlightBackground, Sprite activeBackground, Sprite disabledBackground)
        {
            setSelectable(normalBackground, highlightBackground, activeBackground, disabledBackground);

            Toggle toggle = GetComponent<Toggle>();

            if (toggle == null)
                return;

            //The "checkmark" sprite is replaced with the "active" sprite; this is only displayed when the toggle is in the true state
            Image toggleImage = toggle.graphic as Image;

            if (toggleImage == null)
                return;

            toggleImage.sprite = activeBackground;
            toggleImage.type = Image.Type.Sliced;
        }

        public void setSelectable(Sprite normalBackground, Sprite highlightBackground, Sprite activeBackground, Sprite disabledBackground)
        {
            Selectable select = GetComponent<Selectable>();

            if (select == null)
                return;

            select.image.sprite = normalBackground;
            select.image.type = Image.Type.Sliced;
            select.transition = Selectable.Transition.SpriteSwap;

            SpriteState spriteState = select.spriteState;
            spriteState.highlightedSprite = highlightBackground;
            spriteState.pressedSprite = activeBackground;
            spriteState.disabledSprite = disabledBackground;
            select.spriteState = spriteState;
        }
    }
}
