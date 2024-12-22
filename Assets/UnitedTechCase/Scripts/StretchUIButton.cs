using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnitedTechCase.Scripts
{
    public class StretchUIButton : Button
    {
        private Graphic[] mGraphics;

        private Color _color;

        private IEnumerable<Graphic> Graphics =>
            mGraphics ??= targetGraphic.transform.parent.GetComponentsInChildren<Graphic>();

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            _color = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => colors.normalColor
            };

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (transition == Transition.ColorTint)
            {
                ColorTween(_color * colors.colorMultiplier, instant);
            }
            else if (transition != Transition.None)
            {
                throw new NotSupportedException();
            }
        }

        private void ColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
            {
                return;
            }

            foreach (Graphic g in Graphics)
            {
                g.CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
            }
        }
    }
}