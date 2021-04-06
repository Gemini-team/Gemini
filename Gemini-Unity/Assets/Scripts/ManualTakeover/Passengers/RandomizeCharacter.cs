using UnityEngine;
using AdvancedCustomizableSystem;

public class RandomizeCharacter : MonoBehaviour {
    private void Start() {
        GetComponent<CharacterCustomization>().Randomize();
    }
}
