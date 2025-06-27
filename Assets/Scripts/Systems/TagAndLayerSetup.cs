using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EstiamGameJam2025
{
    /// <summary>
    /// Configure automatiquement les tags et layers nécessaires pour le jeu
    /// </summary>
    public class TagAndLayerSetup : MonoBehaviour
    {
        [Header("Configuration automatique")]
        [SerializeField] private bool setupOnAwake = true;
        
        // Tags requis
        private static readonly string[] requiredTags = {
            "Player",
            "Interactable",
            "Recordable",
            "Bomb"
        };
        
        // Layers requis
        private static readonly string[] requiredLayers = {
            "Interactable", // Layer 6
            "Player",       // Layer 7
            "Obstacles"     // Layer 8
        };
        
        void Awake()
        {
            if (setupOnAwake)
            {
                SetupTagsAndLayers();
            }
        }
        
        [ContextMenu("Setup Tags and Layers")]
        public static void SetupTagsAndLayers()
        {
#if UNITY_EDITOR
            Debug.Log("[TagAndLayerSetup] Configuration des tags et layers...");
            
            // Ajouter les tags
            foreach (string tag in requiredTags)
            {
                AddTag(tag);
            }
            
            // Ajouter les layers
            AddLayer("Interactable", 6);
            AddLayer("Player", 7);
            AddLayer("Obstacles", 8);
            
            Debug.Log("[TagAndLayerSetup] Configuration terminée !");
#else
            Debug.LogWarning("[TagAndLayerSetup] Cette fonction ne marche que dans l'éditeur Unity");
#endif
        }
        
#if UNITY_EDITOR
        private static void AddTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            
            // Vérifier si le tag existe déjà
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName))
                {
                    Debug.Log($"[TagAndLayerSetup] Tag '{tagName}' existe déjà");
                    return;
                }
            }
            
            // Ajouter le tag
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
            newTag.stringValue = tagName;
            tagManager.ApplyModifiedProperties();
            
            Debug.Log($"[TagAndLayerSetup] Tag '{tagName}' ajouté avec succès");
        }
        
        private static void AddLayer(string layerName, int layerIndex)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            
            if (layerIndex < 0 || layerIndex > 31)
            {
                Debug.LogError($"[TagAndLayerSetup] Index de layer invalide : {layerIndex}");
                return;
            }
            
            SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(layerIndex);
            
            if (string.IsNullOrEmpty(layerProp.stringValue))
            {
                layerProp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"[TagAndLayerSetup] Layer '{layerName}' ajouté à l'index {layerIndex}");
            }
            else if (layerProp.stringValue == layerName)
            {
                Debug.Log($"[TagAndLayerSetup] Layer '{layerName}' existe déjà à l'index {layerIndex}");
            }
            else
            {
                Debug.LogWarning($"[TagAndLayerSetup] Layer index {layerIndex} déjà utilisé par '{layerProp.stringValue}'");
            }
        }
#endif
    }
    
    /// <summary>
    /// Composant pour configurer automatiquement une bombe interactive
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BombSetup : MonoBehaviour
    {
        [Header("Configuration automatique")]
        [SerializeField] private bool autoSetup = true;
        
        void Awake()
        {
            if (autoSetup)
            {
                SetupBomb();
            }
        }
        
        [ContextMenu("Setup Bomb")]
        public void SetupBomb()
        {
            // Configurer le tag et layer
            gameObject.tag = "Interactable";
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            
            // Configurer le collider
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
            
            // Ajouter Rigidbody2D si nécessaire (kinematic pour éviter la physique)
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            // Ajouter InteractableObject si pas présent
            InteractableObject interactable = GetComponent<InteractableObject>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<InteractableObject>();
                Debug.Log("[BombSetup] InteractableObject ajouté");
            }
            
            Debug.Log($"[BombSetup] Bombe configurée : Tag={tag}, Layer={LayerMask.LayerToName(gameObject.layer)}");
        }
    }
}