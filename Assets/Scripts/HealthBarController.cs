using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour {
    
    public SlideCharacter character;
    public Text text;
    public RectTransform canvas;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (character == null) return;

        text.text = character.Health.ToString();

        text.GetComponent<RectTransform>().anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, character.transform.position) - canvas.sizeDelta/2f ;
    }
}
