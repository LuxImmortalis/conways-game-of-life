using System;
using Controls;
using Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Plotter
{
    [Serializable]
    public class CubePlotter : AbstractPlotter
    {
        public override GameObject GetCell(int x, int y)
        {
            var newCell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCell.name = $"Cell_{x}–{y}";
            newCell.tag = "Cell";
            newCell.layer = LayerMask.NameToLayer("Cell");
            newCell.transform.parent = _cellContainer.transform;
            newCell.transform.position = new Vector3(x, 0, y);
            newCell.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
            newCell.SetActive(false);

            var r = newCell.GetComponent<Renderer>();
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
            r.material = _deadMaterial;

            return newCell;
        }
        public CubePlotter(int width, int height, GameObject parent) : base(width, height, parent)
        {
            GameObject.Destroy(GameOfLifeManager.Instance.gameObject.GetComponent<TextureCellPicker>());
            _picker = GameOfLifeManager.Instance.gameObject.GetOrAddComponent<ObjectCellPicker>();
        }
    }
}