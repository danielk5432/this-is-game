using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class PropsAltar : MonoBehaviour
    {
        public List<SpriteRenderer> runes;
        public Color inactiveColor = new Color(1f, 1f, 1f, 0f);   // 투명
        public Color activeColor = new Color(1f, 1f, 1f, 1f);     // 완전 보이게

        private void Awake()
        {
            // 초기화: 모든 룬 비활성화
            foreach (var r in runes)
            {
                r.color = inactiveColor;
            }

            // 튜토리얼 룬 활성화
            ActivateRune(2);

            // 클리어 여부에 따라 룬 활성화
            if (GameDataModel.Instance.IsLevelCleared("4")) ActivateRune(0);
            if (GameDataModel.Instance.IsLevelCleared("5")) ActivateRune(1);
            if (GameDataModel.Instance.IsLevelCleared("6")) ActivateRune(3);
        }

        public void ActivateRune(int index)
        {
            if (index >= 0 && index < runes.Count)
            {
                runes[index].color = activeColor;
            }
            else
            {
                Debug.LogWarning($"[PropsAltar] Invalid rune index: {index}");
            }
        }
    }
}