using TMPro;
using UnityEngine;

namespace PimpochkaGames.NeutralAgeScreen
{
    public class AgeOption : MonoBehaviour
    {
        public int Age { get; private set; }

        [SerializeField]
        private TMP_Text _ageText;

        public void SetAge(int age)
        {
            Age = age;
            _ageText.text = age.ToString();
        }
    }
}
