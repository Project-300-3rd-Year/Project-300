using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgNote;
    [SerializeField] private TextMeshProUGUI tmProDate;
    [SerializeField] private TextMeshProUGUI tmProNote;

    //Start.
    private void Awake()
    {

    }
    private void Start()
    {

    }

    public void Show(Note noteToShow)
    {
        imgBackground.gameObject.SetActive(true);
        imgNote.gameObject.SetActive(true);
        tmProDate.text = noteToShow.date;
        tmProNote.text = noteToShow.text;
    }
    public void Hide()
    {
        imgBackground.gameObject.SetActive(false);
        imgNote.gameObject.SetActive(false);
    }
}
