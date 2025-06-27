using UnityEngine;
using UnityEditor;

namespace EstiamGameJam2025
{
    public class CollisionLayerChecker : EditorWindow
    {
        private Vector2 scrollPos;
        
        [MenuItem("Tools/Collision Layer Checker")]
        public static void ShowWindow()
        {
            GetWindow<CollisionLayerChecker>("Collision Layer Checker");
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Configuration des Layers de Collision", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Afficher les layers existants
            EditorGUILayout.LabelField("Layers définis :", EditorStyles.boldLabel);
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    EditorGUILayout.LabelField($"Layer {i}: {layerName}");
                }
            }
            
            EditorGUILayout.Space();
            
            // Vérifier les layers requis
            EditorGUILayout.LabelField("Vérification des layers requis :", EditorStyles.boldLabel);
            CheckRequiredLayer("Player", Color.green, Color.red);
            CheckRequiredLayer("Wall", Color.green, Color.red);
            CheckRequiredLayer("Interactable", Color.green, Color.red);
            
            EditorGUILayout.Space();
            
            // Bouton pour créer les layers manquants
            if (GUILayout.Button("Créer les layers manquants", GUILayout.Height(30)))
            {
                CreateMissingLayers();
            }
            
            EditorGUILayout.Space();
            
            // Afficher la matrice de collision
            EditorGUILayout.LabelField("Matrice de collision :", EditorStyles.boldLabel);
            DisplayCollisionMatrix();
            
            EditorGUILayout.Space();
            
            // Bouton pour configurer les collisions
            if (GUILayout.Button("Configurer les collisions par défaut", GUILayout.Height(30)))
            {
                SetupDefaultCollisions();
            }
        }
        
        void CheckRequiredLayer(string layerName, Color existsColor, Color missingColor)
        {
            int layer = LayerMask.NameToLayer(layerName);
            GUI.color = layer != -1 ? existsColor : missingColor;
            EditorGUILayout.LabelField($"{layerName}: {(layer != -1 ? "✓ Existe" : "✗ Manquant")}");
            GUI.color = Color.white;
        }
        
        void CreateMissingLayers()
        {
            // Cette méthode ne peut pas créer directement les layers
            // mais elle peut guider l'utilisateur
            EditorUtility.DisplayDialog("Information", 
                "Pour créer les layers manquants :\n\n" +
                "1. Allez dans Edit > Project Settings > Tags and Layers\n" +
                "2. Ajoutez les layers suivants :\n" +
                "   - Player\n" +
                "   - Wall\n" +
                "   - Interactable\n\n" +
                "Puis revenez ici pour configurer les collisions.",
                "OK");
        }
        
        void DisplayCollisionMatrix()
        {
            string[] importantLayers = { "Default", "Player", "Wall", "Interactable" };
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(200));
            
            // En-tête
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(80));
            foreach (string layerName in importantLayers)
            {
                EditorGUILayout.LabelField(layerName, GUILayout.Width(80));
            }
            EditorGUILayout.EndHorizontal();
            
            // Matrice
            foreach (string layer1 in importantLayers)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(layer1, GUILayout.Width(80));
                
                int layer1Idx = LayerMask.NameToLayer(layer1);
                if (layer1Idx == -1)
                {
                    EditorGUILayout.LabelField("Layer manquant", EditorStyles.miniLabel);
                }
                else
                {
                    foreach (string layer2 in importantLayers)
                    {
                        int layer2Idx = LayerMask.NameToLayer(layer2);
                        if (layer2Idx == -1)
                        {
                            EditorGUILayout.LabelField("?", GUILayout.Width(80));
                        }
                        else
                        {
                            bool collides = !Physics2D.GetIgnoreLayerCollision(layer1Idx, layer2Idx);
                            GUI.color = collides ? Color.green : Color.red;
                            EditorGUILayout.LabelField(collides ? "✓" : "✗", GUILayout.Width(80));
                            GUI.color = Color.white;
                        }
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void SetupDefaultCollisions()
        {
            // Player devrait collider avec Wall
            SetLayerCollision("Player", "Wall", true);
            
            // Player devrait collider avec Default
            SetLayerCollision("Player", "Default", true);
            
            // Wall devrait collider avec Default
            SetLayerCollision("Wall", "Default", true);
            
            EditorUtility.DisplayDialog("Configuration appliquée", 
                "Les collisions par défaut ont été configurées.\n\n" +
                "Player ↔ Wall : ✓\n" +
                "Player ↔ Default : ✓\n" +
                "Wall ↔ Default : ✓",
                "OK");
        }
        
        void SetLayerCollision(string layer1, string layer2, bool shouldCollide)
        {
            int layer1Idx = LayerMask.NameToLayer(layer1);
            int layer2Idx = LayerMask.NameToLayer(layer2);
            
            if (layer1Idx != -1 && layer2Idx != -1)
            {
                Physics2D.IgnoreLayerCollision(layer1Idx, layer2Idx, !shouldCollide);
            }
        }
    }
}