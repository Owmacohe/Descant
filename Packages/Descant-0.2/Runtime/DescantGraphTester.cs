using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Runtime
{
    public class DescantGraphTester : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] TMP_Text response;
        [SerializeField] Transform choices;
        [SerializeField] GameObject choice;
        
        [Header("Scripts")]
        [SerializeField] DescantGraphController controller;

        void Start()
        {
            DisplayNode();
        }

        void DisplayNode(int choiceIndex = 0)
        {
            List<string> temp = controller.Next(choiceIndex);
            if (temp == null) return;

            for (int i = 0; i < choices.childCount; i++)
                Destroy(choices.GetChild(i).gameObject);

            if (temp.Count == 1)
            {
                response.text = temp[0];
                DisplayNode();
            }
            else
            {
                for (int j = 0; j < temp.Count; j++)
                {
                    GameObject tempChoice = Instantiate(choice, choices);
                    tempChoice.GetComponentInChildren<TMP_Text>().text = temp[j];
                    
                    var copy = j;
                    tempChoice.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisplayNode(copy);
                    });
                }
            }
        }
    }
}