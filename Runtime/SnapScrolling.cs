using System;
using UnityEngine;
using UnityEngine.UI;

namespace PimpochkaGames.NeutralAgeScreen
{
    public class SnapScrolling : MonoBehaviour
    {
        public event Action<AgeOption> OnAgeOptionChanged;

        [Header("General")]
        [SerializeField]
        private int _count = 99;
        [SerializeField]
        private int _spaceSize = 5;
        [SerializeField]
        private float _snapSpeed = 3f;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private float _scrollVelocityThreshold = 400;
        [Header("Prefabs")]
        [SerializeField]
        private AgeOption _firstElementPrefab;
        [SerializeField]
        private AgeOption _defaultElementPrefab;
        [SerializeField]
        private AgeOption _lastElementPrefab;

        private int _selectedIndex;
        private RectTransform _contentRect;
        private AgeOption[] _ageOptions;
        private Vector2[] _panelPositions;
        private bool _isScrolling;
        private Vector2 _contentVector;

        public AgeOption GetCurrentElement()
        {
            return _ageOptions[_selectedIndex];
        }

        private void Awake()
        {
            //Adding the first element where the neutral age screen starts.
            int countElements = _count + 1;

            _contentRect = GetComponent<RectTransform>();
            _ageOptions = new AgeOption[countElements];
            _panelPositions = new Vector2[countElements];

            //Create the first element where the neutral age screen starts
            _ageOptions[0] = Instantiate(_firstElementPrefab, transform, false);

            int currentAge = _count;
            for (int i = 1; i < countElements; i++)
            {
                _ageOptions[i] = Instantiate(_defaultElementPrefab, transform, false);
                _ageOptions[i].SetAge(currentAge);
                currentAge--;

                _ageOptions[i].transform.localPosition = new Vector2(
                    _ageOptions[i].transform.localPosition.x,
                    _ageOptions[i - 1].transform.localPosition.y + _defaultElementPrefab.GetComponent<RectTransform>().sizeDelta.y + _spaceSize);

                _panelPositions[i] = -_ageOptions[i].transform.localPosition;
            }

            AgeOption lastElement = Instantiate(_lastElementPrefab, transform, false);
            lastElement.transform.localPosition = new Vector2(
                    lastElement.transform.localPosition.x,
                    _ageOptions[_ageOptions.Length - 1].transform.localPosition.y + _defaultElementPrefab.GetComponent<RectTransform>().sizeDelta.y + _spaceSize);
        }

        private void FixedUpdate()
        {
            if (_contentRect.anchoredPosition.y >= _panelPositions[0].y && !_isScrolling
                || _contentRect.anchoredPosition.y <= _panelPositions[_panelPositions.Length - 1].y && !_isScrolling)
                _scrollRect.inertia = false;

            float nearestPosition = float.MaxValue;
            int oldSelectedIndex = _selectedIndex;
            for (int i = 0; i < _ageOptions.Length; i++)
            {
                float distance = Mathf.Abs(_contentRect.anchoredPosition.y - _panelPositions[i].y);
                if (distance < nearestPosition)
                {
                    nearestPosition = distance;
                    _selectedIndex = i;
                }
            }

            if (oldSelectedIndex != _selectedIndex)
                OnAgeOptionChanged?.Invoke(_ageOptions[_selectedIndex]);

            float scrollVelocity = Mathf.Abs(_scrollRect.velocity.y);
            if (scrollVelocity < _scrollVelocityThreshold && !_isScrolling)
                _scrollRect.inertia = false;

            if (_isScrolling || scrollVelocity > _scrollVelocityThreshold)
                return;

            _contentVector.y = Mathf.SmoothStep(
                _contentRect.anchoredPosition.y,
                _panelPositions[_selectedIndex].y,
                _snapSpeed * Time.fixedDeltaTime);

            _contentRect.anchoredPosition = _contentVector;
        }

        public void Scrolling(bool value)
        {
            _isScrolling = value;
            if (value)
                _scrollRect.inertia = true;
        }
    }
}
