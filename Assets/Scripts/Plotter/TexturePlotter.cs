using Controls;
using Core;
using Simulation;
using UnityEngine;

namespace Plotter
{
    public class TexturePlotter : AbstractPlotter
    {
        private Material _texMaterial;
        private Texture2D _mainTex;
        private readonly int _scaleFactor = 10;

        public TexturePlotter(int width, int height, GameObject parent) : base(width, height, parent)
        {
            GameObject.Destroy(GameOfLifeManager.Instance.gameObject.GetComponent<ObjectCellPicker>());
            
            GameOfLifeManager.Instance.gameObject.GetOrAddComponent<TextureCellPicker>();
        }

        public override void Prepare()
        {
            cellRepresentation = new GameObject[1, 1];

            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            cellRepresentation[0, 0] = plane;
            plane.tag = "Cell";
            plane.layer = LayerMask.NameToLayer("Cell");
            plane.name = "GameOfLifePlot";
            plane.transform.parent = _cellContainer.transform;
            plane.transform.localScale =
                new Vector3(_scaleFactor, 1, _scaleFactor);
            plane.transform.position = new Vector3(
                GameOfLifeManager.Instance.width / 2,
                0,
                GameOfLifeManager.Instance.height / 2
            );

            _texMaterial = new Material(Shader.Find("Standard"));
            // Flip texture to match other coordinates with other plotters...
            _texMaterial.mainTextureScale = new Vector2(-1, -1);

            Renderer r = plane.GetComponent<Renderer>();
            r.material = _texMaterial;

            _mainTex = new Texture2D(width, height);
            _mainTex.filterMode = FilterMode.Point;

            _isReady = true;
        }

        public override GameObject GetCell(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public override void Plot(Generation currentGeneration)
        {
            if (_isReady)
            {
                Color[] colorArray = new Color[width * height];

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        colorArray[y * height + x] =
                            currentGeneration.Cells[x, y].IsAlive ? _aliveMaterial.color : _deadMaterial.color;
                    }
                }

                _mainTex.SetPixels(colorArray);
                _mainTex.Apply();

                _texMaterial.mainTexture = _mainTex;
            }
        }
    }
}