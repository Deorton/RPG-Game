using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        Health healthComponent;
        [SerializeField] RectTransform foreground;
        [SerializeField] Canvas rootCanvas;

        void Awake()
        {
            healthComponent = GetComponentInParent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Mathf.Approximately(healthComponent.GetHealthFraction(), 0) || Mathf.Approximately(healthComponent.GetHealthFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetHealthFraction(), 1, 1);
        }
    }
}
