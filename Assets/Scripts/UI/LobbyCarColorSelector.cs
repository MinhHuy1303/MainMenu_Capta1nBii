using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [System.Serializable]
    public class CarColorOption
    {
        public GameObject prefab;
        public string displayName;
    }

    public class LobbyCarColorSelector : MonoBehaviour
    {
        [SerializeField] private CarColorOption[] colorOptions;
        [SerializeField] private Transform spawnParent;
        [SerializeField] private Text colorNameText;
        [SerializeField] private Vector3 spawnLocalRotationEuler = new Vector3(0f, 200f, 0f);
        [SerializeField] private string carStageLayerName = "LobbyCarStage";

        private const string PrefKey = "Lobby_SelectedCarColorIndex";

        private GameObject currentInstance;
        private int currentIndex;

        private void Awake()
        {
            currentIndex = PlayerPrefs.GetInt(PrefKey, 0);
            if (colorOptions == null || colorOptions.Length == 0) return;
            if (currentIndex < 0 || currentIndex >= colorOptions.Length) currentIndex = 0;

            // Clean up any editor-time placeholder car(s) left under the stage
            for (int i = spawnParent.childCount - 1; i >= 0; i--)
            {
                Transform child = spawnParent.GetChild(i);
                if (child.name == "LobbyCarModel") DestroyImmediate(child.gameObject);
            }

            SpawnCurrent();
        }

        public void NextColor()
        {
            if (colorOptions == null || colorOptions.Length == 0) return;
            currentIndex = (currentIndex + 1) % colorOptions.Length;
            SpawnCurrent();
            Save();
        }

        public void PreviousColor()
        {
            if (colorOptions == null || colorOptions.Length == 0) return;
            currentIndex = (currentIndex - 1 + colorOptions.Length) % colorOptions.Length;
            SpawnCurrent();
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetInt(PrefKey, currentIndex);
            PlayerPrefs.Save();
        }

        private void SpawnCurrent()
        {
            if (currentInstance != null) DestroyImmediate(currentInstance);

            CarColorOption option = colorOptions[currentIndex];
            if (option.prefab == null) return;

            currentInstance = Instantiate(option.prefab, spawnParent);
            currentInstance.name = "LobbyCarModel";
            currentInstance.transform.localPosition = Vector3.zero;
            currentInstance.transform.localRotation = Quaternion.Euler(spawnLocalRotationEuler);
            int layer = LayerMask.NameToLayer(carStageLayerName);
            if (layer < 0) layer = spawnParent.gameObject.layer;
            SetLayerRecursively(currentInstance, layer);

            if (colorNameText != null) colorNameText.text = option.displayName;
        }

        private void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform) SetLayerRecursively(child.gameObject, layer);
        }
    }
}
