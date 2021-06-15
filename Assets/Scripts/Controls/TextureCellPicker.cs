using UnityEngine;

namespace Controls
{
    public class TextureCellPicker : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !CameraControls.Instance.draggingGamePlot)
            {
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo,
                    float.MaxValue,
                    LayerMask.GetMask("Cell")
                );

                if (hit)
                {
                    Renderer meshRenderer = hitInfo.collider.GetComponent<MeshRenderer>();
                    Texture2D texture2D = meshRenderer.material.mainTexture as Texture2D;
                    Vector2 pCoord = hitInfo.textureCoord;

                    pCoord.x *= texture2D.width;
                    pCoord.y *= texture2D.height;

                    Vector2 tiling = meshRenderer.material.mainTextureScale;

                    var x = GameOfLifeManager.Instance.width + Mathf.FloorToInt(pCoord.x * tiling.x);
                    var y = GameOfLifeManager.Instance.height + Mathf.FloorToInt(pCoord.y * tiling.y);

                    GameOfLifeManager.Instance.ToggleAliveValueAt(
                        x,
                        y
                    );

                    GameOfLifeManager.Instance.UpdatePlot();
                }
            }
        }
    }
}