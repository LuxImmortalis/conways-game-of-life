using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controls
{
    [Serializable]
    public class CameraControls : UnitySingleton<CameraControls>, ICameraControls
    {
        public new Camera camera;

        public bool enableZoom;
        public float zoomFactor = 80f;
        public float dragFactor = 10f;

        public float cameraMinDistance = 4f;
        public float cameraMaxDistance = 200f;

        public Vector3 cameraOffset = new Vector3(0, 100f, 0);
        public Texture2D cursorPointer;

        private Bounds _focusBounds;
        private List<GameObject> _focusObjects = new List<GameObject>();
        private Vector3 _centerPoint;
        private GameOfLifeManager _manager;

        private Color _cursorNormalColor = Color.white;
        private Color _cursorHighlightedColor = Color.red;

        private Image _pauseButtonImage;
        public bool draggingGamePlot = false;

        private GraphicRaycaster _mRaycaster;
        private PointerEventData _mPointerEventData;
        private EventSystem _mEventSystem;

        private void Start()
        {
            _focusBounds = GetFocusBounds();
            _centerPoint = new Vector3(
                GameOfLifeManager.Instance.width / 2,
                0,
                GameOfLifeManager.Instance.height / 2
            );

            CenterCamera();

            if (null == _manager)
            {
                _manager = gameObject.GetComponent<GameOfLifeManager>();
            }

            var PauseButton = gameObject.transform.Find("UI/Canvas/PauseButton");
            _pauseButtonImage = PauseButton.GetComponent<Image>();

           
            _mRaycaster = gameObject.transform.Find("UI/Canvas").gameObject.GetComponent<GraphicRaycaster>();
            _mEventSystem = gameObject.transform.Find("UI/Canvas").gameObject.GetComponent<EventSystem>();
        }

       public void CenterCamera()
        {
            camera.transform.position = _centerPoint + cameraOffset;
        }

        private void Update()
        {
            if (_manager.isPaused)
            {
                _pauseButtonImage.color = _cursorHighlightedColor;
            }
            else
            {
                _pauseButtonImage.color = _cursorNormalColor;
            }

            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftAlt))
            {
        
                _mPointerEventData = new PointerEventData(_mEventSystem);
                _mPointerEventData.position = Input.mousePosition;
                
                List<RaycastResult> results = new List<RaycastResult>();
                
                _mRaycaster.Raycast(_mPointerEventData, results);

                if (results.Count <= 0)
                {
                    if (
                        Input.GetAxis("Mouse X") != 0f
                        ||
                        Input.GetAxis("Mouse Y") != 0f
                    )
                    {
                        var xOffset = Input.GetAxis("Mouse X") * dragFactor * Time.deltaTime;
                        var yOffset = Input.GetAxis("Mouse Y") * dragFactor * Time.deltaTime;

                        cameraOffset += new Vector3(-xOffset, 0, -yOffset);

                        if (null != cursorPointer)
                        {
                            Cursor.SetCursor(cursorPointer, Vector2.zero, CursorMode.Auto);
                        }

                        draggingGamePlot = true;
                    }
                }
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                draggingGamePlot = false;
            }


            if (camera)
                camera.transform.position = _centerPoint + cameraOffset;

            if (enableZoom)
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0f)
                {
                    cameraOffset.y = Mathf.Clamp(
                        cameraOffset.y + scroll * zoomFactor * Time.deltaTime,
                        cameraMinDistance, cameraMaxDistance);
                }
            }
        }


        Bounds GetFocusBounds()
        {
            var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            if (_focusObjects.Count == 0)
            {
                bounds.Encapsulate(gameObject.transform.position);
            }
            else
            {
                foreach (GameObject focusObject in _focusObjects)
                {
                    bounds.Encapsulate(focusObject.transform.position);
                }
            }

            return bounds;
        }

        public void AddFocusObject(GameObject gameObject)
        {
            _focusObjects.Add(gameObject);
        }

        public List<GameObject> GetFocusObjects()
        {
            return _focusObjects;
        }
    }
}