using System;
using System.Collections.Generic;
using Core;
using Plotter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class UiManager : UnitySingleton<UiManager>
    {
        public static readonly float targetFrameRate = 90f;
        public static readonly float maximumTimePerFrame = 1 / targetFrameRate;
        
        public Canvas uiCanvas;

        public GameObject targetFramerateSlider;
        public GameObject newRandomizeButton;
        public GameObject pauseButton;
        public GameObject autoPauseToggle;
        public GameObject fpsCounterText;
        public GameObject generationCounterText;
        public GameObject quitButton;
        public GameObject simFramerateText;
        public GameObject newEmptyButton;
        public GameObject plotterDropdown;
        public GameObject loadingPanel;

        public float loadingProgress = 1f;

        private void Awake()
        {
            if (null == uiCanvas)
            {
                throw new ArgumentNullException(nameof(uiCanvas));
            }

            AssignUiElementOrQuit(ref targetFramerateSlider, "TargetFramerateSlider");
            AssignUiElementOrQuit(ref newRandomizeButton, "NewRandomizeButton");
            AssignUiElementOrQuit(ref pauseButton, "PauseButton");
            AssignUiElementOrQuit(ref autoPauseToggle, "AutoPauseToggle");
            AssignUiElementOrQuit(ref fpsCounterText, "FpsCounterText");
            AssignUiElementOrQuit(ref generationCounterText, "GenerationCounterText");
            AssignUiElementOrQuit(ref quitButton, "QuitButton");
            AssignUiElementOrQuit(ref simFramerateText, "SimFramerateText");
            AssignUiElementOrQuit(ref newEmptyButton, "NewEmptyButton");
            AssignUiElementOrQuit(ref plotterDropdown, "PlotterDropdown");
            AssignUiElementOrQuit(ref loadingPanel, "LoadingPanel");

            AssignDropdownOptions(plotterDropdown, PlotterTypes.GetOptionList(), ChangePlotter);
            autoPauseToggle.GetComponentInChildren<Toggle>().isOn = GameOfLifeManager.Instance.autoPause;

            SetupTargetFrameRate();
        }

        void SetupTargetFrameRate()
        {
            var targetFramerateSliderComponent = targetFramerateSlider.GetComponent<Slider>();

            targetFramerateSliderComponent.minValue = 1;
            targetFramerateSliderComponent.maxValue = GameOfLifeManager.Instance.targetSimulationFramesCap;
            targetFramerateSliderComponent.value = Mathf.Clamp(
                GameOfLifeManager.Instance.targetSimulationFrames,
                1,
                GameOfLifeManager.Instance.targetSimulationFramesCap
            );
        }

        private static void ChangePlotter(int newPlotterIndex)
        {
            GameOfLifeManager.Instance.ChangePlotter(newPlotterIndex);
        }

        void AssignDropdownOptions(GameObject uiElement, List<Dropdown.OptionData> optionList,
            UnityAction<int> callback)
        {
            Dropdown dropdownComponent = uiElement.GetComponent<Dropdown>();
            if (null != dropdownComponent)
            {
                dropdownComponent.ClearOptions();
                dropdownComponent.AddOptions(optionList);
            }

            dropdownComponent.onValueChanged.AddListener(callback);
            dropdownComponent.SetValueWithoutNotify(2);
            dropdownComponent.onValueChanged.Invoke(2);
        }

        void AssignUiElementOrQuit(ref GameObject uiElement, string gameObjectName)
        {
            var gObject = uiCanvas.transform.Find(gameObjectName).gameObject;
            if (null == gObject)
            {
                throw new Exception($"Ui-Element: {gameObjectName} was not found!");
            }

            uiElement = gObject;
        }

        public void UpdateTargetSimulationFramerate()
        {
            Slider targetFrameRateSliderComponent = targetFramerateSlider.GetComponent<Slider>();

            GameOfLifeManager.Instance.targetSimulationFrames =
                Mathf.Clamp(
                    targetFrameRateSliderComponent.value,
                    1,
                    GameOfLifeManager.Instance.targetSimulationFramesCap
                );

            Text targetFrameRateLabel = targetFramerateSlider.transform.Find("Label").gameObject.GetComponent<Text>();
            targetFrameRateLabel.text = $"Sim-FPS Limit: {GameOfLifeManager.Instance.targetSimulationFrames}";
        }

        private void Update()
        {
            if (Time.frameCount % 20 == 0)
            {
                SetTextElementText(fpsCounterText, $"FPS: {Mathf.Floor(1f / Time.unscaledDeltaTime)}");
                string simtimeText =
                    (GameOfLifeManager.Instance.lastSimulationDuration > 1)
                        ? GameOfLifeManager.Instance.lastSimulationDuration.ToString()
                        : "< 1";
                SetTextElementText(simFramerateText,
                    $"SIM-Time: {simtimeText} ms");
            }

            SetTextElementText(generationCounterText, $"Generation: {GameOfLifeManager.Instance.GenerationNumber}");
            UpdateTargetSimulationFramerate();
            isLoading = loadingProgress < 0.99f;
            updateLoadingIndicator();
        }

        public bool isLoading;

        void updateLoadingIndicator()
        {
            loadingPanel.SetActive(isLoading);
            var loadingIndicatorText = loadingPanel.transform.Find("LoadingLabel").GetComponent<Text>();
            var loadingIndicatorProgressBar = loadingPanel.transform.Find("Progressbar").GetComponent<Slider>();
            loadingIndicatorText.text = $"{(100 * loadingProgress).ToString("F")} %";
            loadingIndicatorProgressBar.value = loadingProgress;
        }

        void SetTextElementText(GameObject uiElement, string text)
        {
            var textComponent = uiElement.GetComponent<Text>();
            if (null != textComponent)
            {
                textComponent.text = text;
            }
        }

        public void ToggleAutoPause(bool newState)
        {
            GameOfLifeManager.Instance.autoPause = newState;
        }

        public void TogglePause()
        {
            GameOfLifeManager.Instance.isPaused = !GameOfLifeManager.Instance.isPaused;
        }
    }
}