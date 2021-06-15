using System.Collections;
using Controls;
using Simulation;
using Ui;
using UnityEngine;

namespace Plotter
{
    public abstract class AbstractPlotter
    {
        protected int width;
        protected int height;
        protected GameObject Parent;
        protected GameObject[,] cellRepresentation;
        protected ObjectCellPicker _picker;
        protected Material _deadMaterial;
        protected Material _aliveMaterial;
        protected bool _isReady = false;
        protected GameObject _cellContainer;
        protected float _scaleFactor = .95f;
        protected float _timeStamp;

        protected AbstractPlotter(int width, int height, GameObject parent)
        {
            this.width = width;
            this.height = height;
            Parent = parent;
            
            _deadMaterial = Resources.Load<Material>("Material/DeadMaterial");
            _aliveMaterial = Resources.Load<Material>("Material/AliveMaterial");

            _cellContainer = new GameObject("CellContainer");
            _cellContainer.transform.parent = Parent.transform;
            
            Prepare();
        }

        public virtual void Prepare()
        {
            cellRepresentation = new GameObject[width, height];
            UiManager.Instance.StartCoroutine(BuildCells());
        }

        public abstract GameObject GetCell(int x, int y);

        public virtual IEnumerator BuildCells()
        {
            _timeStamp = Time.realtimeSinceStartup;
            float totalLoad = width * height;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    cellRepresentation[x, y] = GetCell(x, y);
                    UiManager.Instance.loadingProgress = 1f / totalLoad * (x * width + y);
                    if (Time.realtimeSinceStartup > _timeStamp + UiManager.maximumTimePerFrame)
                    {
                        yield return null;
                        _timeStamp = Time.realtimeSinceStartup;
                    }
                }
            }

            UiManager.Instance.loadingProgress = 1f;
            _isReady = true;
            GameOfLifeManager.Instance.UpdatePlot();
        }


        public virtual void Plot(Generation currentGeneration)
        {
            if (_isReady)
            {
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        cellRepresentation[x, y].SetActive(true);
                        var r = cellRepresentation[x, y].GetComponent<Renderer>();
                        r.sharedMaterial = currentGeneration.GetAliveValueAt(x, y) ? _aliveMaterial : _deadMaterial;
                    }
                }
            }
        }

        public void DestroyPlot()
        {
            foreach (GameObject gameObject in cellRepresentation)
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}