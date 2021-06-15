using System.Diagnostics;
using Controls;
using Core;
using Plotter;
using Simulation;
using UnityEditor;
using UnityEngine;

public class GameOfLifeManager : UnitySingleton<GameOfLifeManager>
{
    [Header("Simulation Parameters")] public int width;
    public int height;

    public bool isPaused = false;
    public bool autoPause = false;

    [Header("Statistics")] public float targetSimulationFrames = 30f;
    public float targetSimulationFramesCap = 60f;
    public float lastSimulationDuration = 0f;
    public int GenerationNumber { get; private set; }

    private float _timeSinceLastSimulation = 0f;

    private Generation _currentGeneration;
    private AbstractPlotter Plotter { get; set; }

    public void Awake()
    {
        RandomizeNew();
    }

    public void RandomizeNew()
    {
        StartNewGeneration(true);
    }

    public void DeadNew()
    {
        StartNewGeneration(false);
    }

    void StartNewGeneration(bool randomize)
    {
        _currentGeneration =
            new Generation(width, height, randomize);
        GenerationNumber = 0;

        if (autoPause && !isPaused)
        {
            isPaused = true;
        }

        UpdatePlot();
    }

    public void SetGeneration(Generation generation)
    {
        _currentGeneration = generation;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) DoQuit();
        
        // Save Stuff is not finished yet!
        // if (Input.GetKey(KeyCode.A)) SerializeGeneration();
        // if (Input.GetKey(KeyCode.S)) LoadGame();

        if (isPaused) return;

        Simulate();
        UpdatePlot();
    }

    void Simulate()
    {
        if (_timeSinceLastSimulation > 1f / targetSimulationFrames)
        {
            var s = new Stopwatch();
            s.Start();
            _currentGeneration = _currentGeneration.GetNextGeneration();
            GenerationNumber++;
            lastSimulationDuration = s.ElapsedMilliseconds;
            _timeSinceLastSimulation = 0f;
        }

        _timeSinceLastSimulation += Time.deltaTime;
    }

    public void ToggleAliveValueAt(int x, int y)
    {
        _currentGeneration.ToggleAliveValueAt(x, y);
    }

    public void UpdatePlot()
    {
        Plotter?.Plot(_currentGeneration);
    }

    public void DoQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangePlotter(int newPlotterIndex)
    {
        if (PlotterTypes.GetOptionIndex(Plotter) == newPlotterIndex) return;

        Plotter?.DestroyPlot();

        switch (PlotterTypes.GetTypeFromIndex(newPlotterIndex))
        {
            case TypePlotter.TypeCubePlotter:
                Plotter = new CubePlotter(width, height, gameObject);
                break;
            case TypePlotter.TypeQuadPlotter:
                Plotter = new QuadPlotter(width, height, gameObject);
                break;
            case TypePlotter.TypeTexturePlotter:
                Plotter = new TexturePlotter(width, height, gameObject);
                break;
        }

        CameraControls.Instance.CenterCamera();
        UpdatePlot();
    }
}