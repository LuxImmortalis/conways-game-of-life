using System;
using Controls;
using Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Plotter
{
    [Serializable]
    public class QuadPlotter : AbstractPlotter
    {
        public override GameObject GetCell(int x, int y)
        {
            GameObject newCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newCell.name = $"Cell_{x}–{y}";
            newCell.tag = "Cell";
            newCell.layer = LayerMask.NameToLayer("Cell");
            newCell.transform.Rotate(new Vector3(90, 0, 0));
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

        public QuadPlotter(int width, int height, GameObject parent) : base(width, height, parent)
        {
            GameObject.Destroy(GameOfLifeManager.Instance.gameObject.GetComponent<TextureCellPicker>());
            _picker = GameOfLifeManager.Instance.gameObject.GetOrAddComponent<ObjectCellPicker>();
        }
    }
}