using TMPro;
using UnityEngine;

namespace Hertzole.GoldPlayer.Example
{
    public class InteractionUpdater : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        private string showText = "You have %i% interactions left";
        [SerializeField]
        private TextMeshPro targetLabel = null;
        [SerializeField]
        private GoldPlayerInteractable interactable = null;

        private void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            targetLabel.text = showText.Replace("%i%", (interactable.MaxInteractions - interactable.Interactions).ToString());
        }
    }
}
