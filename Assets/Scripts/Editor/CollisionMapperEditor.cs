using UnityEngine;
using UnityEditor;

namespace EstiamGameJam2025
{
    [CustomEditor(typeof(CollisionMapper))]
    public class CollisionMapperEditor : Editor
    {
        private CollisionMapper mapper;
        private bool isPlacingCollider = false;
        private Vector2 startPosition;
        
        void OnEnable()
        {
            mapper = (CollisionMapper)target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Outils de Collision", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Ajouter un Collider Simple", GUILayout.Height(30)))
            {
                Undo.RecordObject(mapper, "Add Collision Area");
                var collisionAreas = serializedObject.FindProperty("collisionAreas");
                collisionAreas.InsertArrayElementAtIndex(collisionAreas.arraySize);
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.Space(5);
            
            GUI.backgroundColor = isPlacingCollider ? Color.red : Color.green;
            if (GUILayout.Button(isPlacingCollider ? "Annuler Placement" : "Placer avec la Souris", GUILayout.Height(30)))
            {
                isPlacingCollider = !isPlacingCollider;
                if (isPlacingCollider)
                {
                    SceneView.duringSceneGui += OnSceneGUI;
                }
                else
                {
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Générer Colliders Automatiques", GUILayout.Height(30)))
            {
                GenerateAutomaticColliders();
            }
            
            if (GUILayout.Button("Nettoyer Tous les Colliders", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirmer", "Supprimer tous les colliders?", "Oui", "Non"))
                {
                    Undo.RecordObject(mapper, "Clear Colliders");
                    var collisionAreas = serializedObject.FindProperty("collisionAreas");
                    collisionAreas.ClearArray();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        
        void OnSceneGUI(SceneView sceneView)
        {
            if (!isPlacingCollider) return;
            
            Event e = Event.current;
            
            // Convertir la position de la souris en coordonnées du monde
            Vector3 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            mousePos.z = 0;
            
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        startPosition = mousePos;
                        e.Use();
                    }
                    break;
                    
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        Vector2 endPosition = mousePos;
                        CreateColliderFromPoints(startPosition, endPosition);
                        e.Use();
                    }
                    break;
                    
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        // Dessiner un rectangle de prévisualisation
                        Handles.color = new Color(1, 0, 0, 0.3f);
                        Vector3[] verts = new Vector3[] {
                            new Vector3(startPosition.x, startPosition.y, 0),
                            new Vector3(mousePos.x, startPosition.y, 0),
                            new Vector3(mousePos.x, mousePos.y, 0),
                            new Vector3(startPosition.x, mousePos.y, 0)
                        };
                        Handles.DrawSolidRectangleWithOutline(verts, new Color(1, 0, 0, 0.2f), Color.red);
                        SceneView.RepaintAll();
                        e.Use();
                    }
                    break;
            }
        }
        
        void CreateColliderFromPoints(Vector2 start, Vector2 end)
        {
            Vector2 center = (start + end) / 2f;
            Vector2 size = new Vector2(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
            
            if (size.magnitude > 0.1f)
            {
                Undo.RecordObject(mapper, "Add Collision Area");
                mapper.AddCollisionArea(center - (Vector2)mapper.transform.position, size, "Mur");
                EditorUtility.SetDirty(mapper);
            }
        }
        
        void GenerateAutomaticColliders()
        {
            // Cette méthode pourrait analyser l'image pour détecter les murs
            // Pour l'instant, on génère juste les bordures
            Undo.RecordObject(mapper, "Generate Automatic Colliders");
            
            var collisionAreas = serializedObject.FindProperty("collisionAreas");
            collisionAreas.ClearArray();
            
            // Bordures de l'écran (à ajuster selon votre caméra)
            float screenWidth = 20f;
            float screenHeight = 12f;
            float wallThickness = 1f;
            
            // Haut
            AddCollisionAreaToList(new Vector2(0, screenHeight/2), new Vector2(screenWidth, wallThickness), "Bordure Haut");
            // Bas
            AddCollisionAreaToList(new Vector2(0, -screenHeight/2), new Vector2(screenWidth, wallThickness), "Bordure Bas");
            // Gauche
            AddCollisionAreaToList(new Vector2(-screenWidth/2, 0), new Vector2(wallThickness, screenHeight), "Bordure Gauche");
            // Droite
            AddCollisionAreaToList(new Vector2(screenWidth/2, 0), new Vector2(wallThickness, screenHeight), "Bordure Droite");
            
            serializedObject.ApplyModifiedProperties();
        }
        
        void AddCollisionAreaToList(Vector2 position, Vector2 size, string name)
        {
            var collisionAreas = serializedObject.FindProperty("collisionAreas");
            int index = collisionAreas.arraySize;
            collisionAreas.InsertArrayElementAtIndex(index);
            
            var element = collisionAreas.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = name;
            element.FindPropertyRelative("position").vector2Value = position;
            element.FindPropertyRelative("size").vector2Value = size;
            element.FindPropertyRelative("rotation").floatValue = 0;
            element.FindPropertyRelative("gizmoColor").colorValue = Color.red;
        }
    }
}