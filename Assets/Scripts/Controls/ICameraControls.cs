using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    public interface ICameraControls
    {
        void AddFocusObject(GameObject gameObject);
        List<GameObject> GetFocusObjects();
    }
}