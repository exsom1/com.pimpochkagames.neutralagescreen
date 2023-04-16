using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PimpochkaGames.NeutralAgeScreen
{
    public class NeutralAgeScreenManager : MonoBehaviour
    {
        private const string USER_AGE_PROPERTY_NAME = "user_age";
        public static event Action<int> OnApplyUserAge;

        [SerializeField]
        private bool _autoDestory = true;
        [SerializeField]
        private SnapScrolling _snapScrollingAge;
        [SerializeField]
        private Button _applyButton;
        [SerializeField]
        private TMP_Text _applyText;
        [SerializeField]
        private TMP_Text _infoMessage;

        private void Awake()
        {
            if (PlayerPrefs.HasKey(USER_AGE_PROPERTY_NAME))
            {
                if (_autoDestory)
                    Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            _applyButton.onClick.AddListener(ApplyUserAge);
            _snapScrollingAge.OnAgeOptionChanged += AgeOptionChangeHandler;
        }

        private void OnDestroy()
        {
            _applyButton.onClick.RemoveAllListeners();
            _snapScrollingAge.OnAgeOptionChanged -= AgeOptionChangeHandler;
        }

        public static bool IsAgeReady() => PlayerPrefs.HasKey(USER_AGE_PROPERTY_NAME);

        public static bool TryGetUserAge(out int age)
        {
            if (!PlayerPrefs.HasKey(USER_AGE_PROPERTY_NAME))
            {
                age = 0;
                return false;
            }

            age = PlayerPrefs.GetInt(USER_AGE_PROPERTY_NAME);
            return true;
        }

        public void LocalizeNeutralAgeScreen(string applyText, string infoMessage)
        {
            _applyText.text = applyText;
            _infoMessage.text = infoMessage;
        }

        private void ApplyUserAge()
        {
            AgeOption ageOption = _snapScrollingAge.GetCurrentElement();
            if (ageOption.Age == 0)
                return;

            PlayerPrefs.SetInt(USER_AGE_PROPERTY_NAME, ageOption.Age);
            OnApplyUserAge?.Invoke(ageOption.Age);

            if (_autoDestory)
                Destroy(gameObject);
        }

        private void AgeOptionChangeHandler(AgeOption ageOption)
        {
            if (ageOption.Age == 0)
                _applyButton.interactable = false;
            else
                _applyButton.interactable = true;
        }
    }
}
