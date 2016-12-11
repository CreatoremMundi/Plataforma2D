using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebugCanvas : MonoBehaviour {
    public Color textColor = Color.white;
    private Text[] textList;

	void Start () {
        textList = new Text[transform.childCount];

        for (int i = 0; i < transform.childCount; ++i)
        {
            textList[i] = transform.FindChild(string.Format("Text{0}", i)).GetComponent<Text>();
        }
	}

    public void Show(int position, string text)
    {
        if(position < 0 || position >= textList.Length)
        {
            throw new System.Exception("Não é possível exibir um texto na posição " + position);
        }

        textList[position].text = text;
    }
}
