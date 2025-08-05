using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YouWonPopUp : Panel
{
    #region INSPECTOR PROPERTIES

    [SerializeField] private TMP_Text youWonText;
    [SerializeField] private Button okButton;

    #endregion
    
    #region UNITY METHODS

    protected override void Start()
    {
        base.Start();
        okButton.onClick.AddListener(OnOkButtonClicked);
        Disappear();
    }

    #endregion

    #region PUBLIC METHODS

    public void Show(int profit)
    {
        youWonText.text = $"You won ${profit}";
        Appear();
    }

    #endregion
    
    #region PRIVATE METHODS

    private void OnOkButtonClicked()
    {
        Disappear();
    }

    #endregion
}