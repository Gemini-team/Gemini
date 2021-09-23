using UnityEngine;
using UnityEngine.UI;

public class AIOverlay : MonoBehaviour {
    private const string OBSTACLE_TAG = "Obstacle";

    public Sprite bbSprite;
    public Color bbColor;

    private void Start() {
        if (PlayerPrefs.HasKey("ObjectDetection") && PlayerPrefs.GetInt("ObjectDetection") == 0) {
            gameObject.SetActive(false);
            return;
        }

        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag(OBSTACLE_TAG)) {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            
            BoundingBoxWidget widget = go.AddComponent<BoundingBoxWidget>();
            widget.target = obstacle;
            
            Image im = go.AddComponent<Image>();
            im.sprite = bbSprite;
            im.color = bbColor;
            im.rectTransform.anchorMin = Vector2.zero;
            im.rectTransform.anchorMax = Vector2.zero;
            im.type = Image.Type.Sliced;
        }
    }
}
