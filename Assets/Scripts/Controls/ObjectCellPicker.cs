using UnityEngine;

namespace Controls
{
    public class ObjectCellPicker : MonoBehaviour
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
                    string otherName = hitInfo.transform.gameObject.name;

                    // Extract coordinates from name, for sake of ease :-)
                    string coords = otherName.Split('_')[1];
                    string[] splitCoords = coords.Split('–');

                    if (splitCoords.Length == 2)
                    {
                        // If we have valid coords we can switch the actual bit in Cells of the generation
                        string coordX = splitCoords[0];
                        string coordY = splitCoords[1];

                        GameOfLifeManager.Instance.isPaused = true;

                        GameOfLifeManager.Instance.ToggleAliveValueAt(
                            int.Parse(coordX),
                            int.Parse(coordY)
                        );

                        GameOfLifeManager.Instance.UpdatePlot();
                    }
                }
            }
        }
    }
}