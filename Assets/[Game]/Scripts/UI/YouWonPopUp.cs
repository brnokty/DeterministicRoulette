using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YouWonPopUp : Panel
{
    [SerializeField] private TMP_Text youWonText;
    [SerializeField] private Button okButton;

    protected override void Start()
    {
        base.Start();
        okButton.onClick.AddListener(OnOkButtonClicked);
        Disappear();
    }

    public void Show(int profit)
    {
        youWonText.text = $"You won ${profit}";
        Appear();
    }

    private void OnOkButtonClicked()
    {
        Disappear();
    }
}