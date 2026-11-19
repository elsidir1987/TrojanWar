using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    // This method can be called from the UI Button's OnClick() event in the Inspector
    public void PlayMySound()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayClickSound();
        }
    }
}