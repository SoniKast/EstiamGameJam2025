using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EstiamGameJam2025
{
    public class LayerSetup : MonoBehaviour
    {
        [Header("Layer Configuration")]
        [SerializeField] private bool checkLayersOnStart = true;
        [SerializeField] private bool showWarnings = true;
        
        // Les layers requis pour le jeu
        private static readonly string[] requiredLayers = { "Player", "Wall", "Interactable" };
        
        void Awake()
        {
            if (checkLayersOnStart)
            {
                CheckAndReportMissingLayers();
            }
        }
        
        public static void CheckAndReportMissingLayers()
        {
            bool allLayersExist = true;
            string missingLayers = "";
            
            foreach (string layerName in requiredLayers)
            {
                if (LayerMask.NameToLayer(layerName) == -1)
                {
                    allLayersExist = false;
                    missingLayers += $"\n- {layerName}";
                }
            }
            
            if (!allLayersExist)
            {
                string message = $"Les layers suivants sont manquants :{missingLayers}\n\n" +
                    "Pour les créer :\n" +
                    "1. Edit > Project Settings > Tags and Layers\n" +
                    "2. Ajoutez les layers manquants dans User Layer\n\n" +
                    "Sans ces layers, les collisions ne fonctionneront pas correctement!";
                
                Debug.LogError(message);
                
                #if UNITY_EDITOR
                if (EditorUtility.DisplayDialog("Layers Manquants", message, "Ouvrir Project Settings", "Plus tard"))
                {
                    SettingsService.OpenProjectSettings("Project/TagManager");
                }
                #endif
            }
            else
            {
                Debug.Log("Tous les layers requis sont présents ✓");
            }
        }
        
        // Méthode pour vérifier si un layer existe
        public static bool LayerExists(string layerName)
        {
            return LayerMask.NameToLayer(layerName) != -1;
        }
        
        // Méthode pour obtenir un layer avec fallback
        public static int GetLayerWithFallback(string preferredLayer, string fallbackLayer = "Default")
        {
            int layer = LayerMask.NameToLayer(preferredLayer);
            if (layer == -1)
            {
                layer = LayerMask.NameToLayer(fallbackLayer);
                if (layer == -1)
                {
                    layer = 0; // Default layer
                }
                Debug.LogWarning($"Layer '{preferredLayer}' non trouvé, utilisation de '{fallbackLayer}'");
            }
            return layer;
        }
    }
    
    #if UNITY_EDITOR
    // Menu pour vérifier les layers depuis l'éditeur
    public static class LayerSetupMenu
    {
        [MenuItem("Tools/Check Required Layers")]
        public static void CheckLayers()
        {
            LayerSetup.CheckAndReportMissingLayers();
        }
        
        [MenuItem("Tools/Open Layer Settings")]
        public static void OpenLayerSettings()
        {
            SettingsService.OpenProjectSettings("Project/TagManager");
        }
    }
    #endif
}