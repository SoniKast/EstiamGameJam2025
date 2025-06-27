using UnityEngine;
using UnityEngine.EventSystems;

public class MazeWall : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MazeGame.Instance != null && MazeGame.Instance.isPlaying)
        {
            MazeGame.Instance.Lose();
        }
    }
}
