using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EstiamGameJam2025
{
    public enum CatastropheType
    {
        Explosion,
        Fire,
        Collapse,
        GasLeak,
        Flooding,
        ElectricalFailure
    }

    [System.Serializable]
    public class Catastrophe
    {
        public CatastropheType type;
        public string name;
        public string description;
        public GameObject effectPrefab;
        public GameObject triggerPrefab;
        public AudioClip soundEffect;
        public float damageRadius = 5f;
        public Color warningColor = Color.red;
    }

    public class CatastropheManager : MonoBehaviour
    {
        [Header("Catastrophe Settings")]
        [SerializeField] private List<Catastrophe> availableCatastrophes = new List<Catastrophe>();
        [SerializeField] private Transform catastropheSpawnPoint;
        [SerializeField] private float catastropheWarningTime = 2f;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject warningIndicatorPrefab;
        [SerializeField] private GameObject explosionEffectPrefab;
        [SerializeField] private ParticleSystem globalParticleEffect;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip warningSound;
        
        // État actuel
        private Catastrophe currentCatastrophe;
        private GameObject currentCatastropheInstance;
        private GameObject currentTriggerInstance;
        private Vector3 catastropheLocation;
        private bool catastropheTriggered = false;
        private bool catastrophePrevented = false;
        
        // Events
        public delegate void OnCatastropheTriggered(CatastropheType type, Vector3 location);
        public event OnCatastropheTriggered onCatastropheTriggered;
        
        public delegate void OnCatastrophePrevented(CatastropheType type);
        public event OnCatastrophePrevented onCatastrophePrevented;

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            
            // Initialiser les catastrophes par défaut si la liste est vide
            if (availableCatastrophes.Count == 0)
            {
                InitializeDefaultCatastrophes();
            }
        }

        private void InitializeDefaultCatastrophes()
        {
            // Explosion
            availableCatastrophes.Add(new Catastrophe
            {
                type = CatastropheType.Explosion,
                name = "Explosion",
                description = "Une bombe va exploser !",
                damageRadius = 8f,
                warningColor = Color.red
            });
            
            // Incendie
            availableCatastrophes.Add(new Catastrophe
            {
                type = CatastropheType.Fire,
                name = "Incendie",
                description = "Un court-circuit va déclencher un incendie !",
                damageRadius = 6f,
                warningColor = new Color(1f, 0.5f, 0f) // Orange
            });
            
            // Effondrement
            availableCatastrophes.Add(new Catastrophe
            {
                type = CatastropheType.Collapse,
                name = "Effondrement",
                description = "La structure va s'effondrer !",
                damageRadius = 10f,
                warningColor = Color.gray
            });
            
            // Fuite de gaz
            availableCatastrophes.Add(new Catastrophe
            {
                type = CatastropheType.GasLeak,
                name = "Fuite de gaz",
                description = "Une fuite de gaz toxique !",
                damageRadius = 12f,
                warningColor = Color.green
            });
        }

        public void PrepareCatastrophe(CatastropheType? specificType = null)
        {
            catastropheTriggered = false;
            catastrophePrevented = false;
            
            // Sélectionner une catastrophe
            if (specificType.HasValue)
            {
                currentCatastrophe = availableCatastrophes.Find(c => c.type == specificType.Value);
            }
            else
            {
                // Sélection aléatoire
                currentCatastrophe = availableCatastrophes[Random.Range(0, availableCatastrophes.Count)];
            }
            
            if (currentCatastrophe == null)
            {
                Debug.LogError("Aucune catastrophe disponible !");
                return;
            }
            
            // Déterminer la position de la catastrophe
            DetermineCatastropheLocation();
            
            // Placer le déclencheur de la catastrophe dans la scène
            PlaceCatastropheTrigger();
            
            Debug.Log($"Catastrophe préparée : {currentCatastrophe.name} à la position {catastropheLocation}");
        }

        private void DetermineCatastropheLocation()
        {
            if (catastropheSpawnPoint != null)
            {
                catastropheLocation = catastropheSpawnPoint.position;
            }
            else
            {
                // Position aléatoire dans un rayon
                Vector2 randomPos = Random.insideUnitCircle * 10f;
                catastropheLocation = new Vector3(randomPos.x, randomPos.y, 0f);
            }
        }

        private void PlaceCatastropheTrigger()
        {
            // Nettoyer l'ancien trigger s'il existe
            if (currentTriggerInstance != null)
            {
                Destroy(currentTriggerInstance);
            }
            
            // Créer le trigger/objet interactif pour prévenir la catastrophe
            if (currentCatastrophe.triggerPrefab != null)
            {
                currentTriggerInstance = Instantiate(currentCatastrophe.triggerPrefab, catastropheLocation, Quaternion.identity);
            }
            else
            {
                // Créer un trigger par défaut
                currentTriggerInstance = CreateDefaultTrigger();
            }
            
            // Configurer le trigger
            ConfigureTrigger(currentTriggerInstance);
        }

        private GameObject CreateDefaultTrigger()
        {
            GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trigger.name = $"CatastropheTrigger_{currentCatastrophe.type}";
            trigger.transform.localScale = Vector3.one * 0.5f;
            
            // Rendre le trigger semi-transparent
            Renderer renderer = trigger.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = new Color(currentCatastrophe.warningColor.r, 
                                    currentCatastrophe.warningColor.g, 
                                    currentCatastrophe.warningColor.b, 0.5f);
                renderer.material = mat;
            }
            
            // Ajouter un collider trigger
            Collider collider = trigger.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
            
            return trigger;
        }

        private void ConfigureTrigger(GameObject trigger)
        {
            // Ajouter le tag pour que le PlayerController puisse l'identifier
            trigger.tag = "Interactable";
            
            // Ajouter le layer si nécessaire
            int interactableLayer = LayerMask.NameToLayer("Interactable");
            if (interactableLayer != -1)
            {
                trigger.layer = interactableLayer;
            }
            
            // Ajouter le composant MiniGameTrigger
            MiniGameTrigger miniGameTrigger = trigger.GetComponent<MiniGameTrigger>();
            if (miniGameTrigger == null)
            {
                miniGameTrigger = trigger.AddComponent<MiniGameTrigger>();
            }
            miniGameTrigger.MiniGameType = GetMiniGameTypeForCatastrophe(currentCatastrophe.type);
            
            // Masquer le trigger pendant la première phase
            trigger.SetActive(false);
        }

        private MiniGameType GetMiniGameTypeForCatastrophe(CatastropheType catastropheType)
        {
            // Mapper chaque catastrophe à un mini-jeu approprié
            switch (catastropheType)
            {
                case CatastropheType.Explosion:
                    return MiniGameType.NumberSequence; // Entrer le code de désamorçage
                
                case CatastropheType.Fire:
                    return MiniGameType.SwitchActivation; // Activer les extincteurs
                
                case CatastropheType.Collapse:
                    return MiniGameType.CableMatch; // Reconnecter les supports
                
                case CatastropheType.GasLeak:
                    return MiniGameType.SwitchActivation; // Fermer les valves
                
                case CatastropheType.Flooding:
                    return MiniGameType.CableMatch; // Réparer les pompes
                
                case CatastropheType.ElectricalFailure:
                    return MiniGameType.NumberSequence; // Réinitialiser le système
                
                default:
                    // Sélection aléatoire parmi les 3 mini-jeux disponibles
                    MiniGameType[] availableGames = { MiniGameType.SwitchActivation, MiniGameType.CableMatch, MiniGameType.NumberSequence };
                    return availableGames[Random.Range(0, availableGames.Length)];
            }
        }

        public void TriggerCatastrophe()
        {
            if (catastropheTriggered || currentCatastrophe == null) return;
            
            catastropheTriggered = true;
            StartCoroutine(Co_CatastropheSequence());
        }

        private IEnumerator Co_CatastropheSequence()
        {
            // Avertissement
            if (warningIndicatorPrefab != null)
            {
                GameObject warning = Instantiate(warningIndicatorPrefab, catastropheLocation, Quaternion.identity);
                Destroy(warning, catastropheWarningTime);
            }
            
            // Son d'avertissement
            if (warningSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(warningSound);
            }
            
            // Attendre avant la catastrophe
            yield return new WaitForSeconds(catastropheWarningTime);
            
            // Déclencher l'effet de la catastrophe
            ExecuteCatastrophe();
        }

        private void ExecuteCatastrophe()
        {
            Debug.Log($"CATASTROPHE : {currentCatastrophe.name} !");
            
            // Effet visuel
            if (currentCatastrophe.effectPrefab != null)
            {
                currentCatastropheInstance = Instantiate(currentCatastrophe.effectPrefab, catastropheLocation, Quaternion.identity);
            }
            else if (explosionEffectPrefab != null)
            {
                currentCatastropheInstance = Instantiate(explosionEffectPrefab, catastropheLocation, Quaternion.identity);
            }
            
            // Effet sonore
            if (currentCatastrophe.soundEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(currentCatastrophe.soundEffect);
            }
            
            // Particules globales
            if (globalParticleEffect != null)
            {
                globalParticleEffect.Play();
            }
            
            // Notifier les autres systèmes
            onCatastropheTriggered?.Invoke(currentCatastrophe.type, catastropheLocation);
            
            // Appliquer les dégâts dans la zone
            ApplyCatastropheDamage();
        }

        private void ApplyCatastropheDamage()
        {
            // Trouver tous les objets dans le rayon de la catastrophe
            Collider2D[] colliders = Physics2D.OverlapCircleAll(catastropheLocation, currentCatastrophe.damageRadius);
            
            foreach (Collider2D col in colliders)
            {
                // Vérifier si c'est le joueur
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Le joueur a été touché par la catastrophe !");
                }
                
                // Appliquer des effets visuels aux objets
                SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    StartCoroutine(Co_FlashObject(sr, currentCatastrophe.warningColor));
                }
            }
        }

        private IEnumerator Co_FlashObject(SpriteRenderer renderer, Color flashColor)
        {
            Color originalColor = renderer.color;
            
            for (int i = 0; i < 3; i++)
            {
                renderer.color = flashColor;
                yield return new WaitForSeconds(0.1f);
                renderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void ShowCatastropheTrigger()
        {
            // Montrer le trigger pendant la phase d'investigation
            if (currentTriggerInstance != null)
            {
                currentTriggerInstance.SetActive(true);
                
                // Ajouter un effet visuel pour le rendre plus visible
                StartCoroutine(Co_PulseTrigger(currentTriggerInstance));
            }
        }

        private IEnumerator Co_PulseTrigger(GameObject trigger)
        {
            Transform t = trigger.transform;
            Vector3 originalScale = t.localScale;
            
            while (trigger != null && trigger.activeSelf)
            {
                t.localScale = originalScale * 1.2f;
                yield return new WaitForSeconds(0.5f);
                t.localScale = originalScale;
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void PreventCatastrophe()
        {
            if (catastrophePrevented) return;
            
            catastrophePrevented = true;
            Debug.Log($"Catastrophe {currentCatastrophe.name} empêchée !");
            
            // Désactiver le trigger
            if (currentTriggerInstance != null)
            {
                currentTriggerInstance.SetActive(false);
            }
            
            // Nettoyer les effets de catastrophe
            if (currentCatastropheInstance != null)
            {
                Destroy(currentCatastropheInstance);
            }
            
            // Notifier
            onCatastrophePrevented?.Invoke(currentCatastrophe.type);
        }

        public void ResetCatastrophe()
        {
            catastropheTriggered = false;
            catastrophePrevented = false;
            
            // Nettoyer les instances
            if (currentCatastropheInstance != null)
            {
                Destroy(currentCatastropheInstance);
            }
            
            if (currentTriggerInstance != null)
            {
                Destroy(currentTriggerInstance);
            }
        }

        // Méthodes publiques
        public Catastrophe GetCurrentCatastrophe() => currentCatastrophe;
        public Vector3 GetCatastropheLocation() => catastropheLocation;
        public bool IsCatastropheTriggered() => catastropheTriggered;
        public bool IsCatastrophePrevented() => catastrophePrevented;
        
        // Debug
        private void OnDrawGizmosSelected()
        {
            if (currentCatastrophe != null)
            {
                Gizmos.color = currentCatastrophe.warningColor;
                Gizmos.DrawWireSphere(catastropheLocation, currentCatastrophe.damageRadius);
            }
        }
    }
}