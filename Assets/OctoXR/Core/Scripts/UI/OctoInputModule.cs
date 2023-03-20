using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace OctoXR.UI
{
    public class OctoInputModule : BaseInputModule
    {
        private List<UIPointer> pointers = new List<UIPointer>();
        private PointerEventData[] pointerEventDatas;

        private OctoInputModule instance;
        private bool isDestroyed;

        public OctoInputModule Instance
        {
            get
            {
                if (isDestroyed)
                {
                    return null;
                }

                if (instance == null)
                {
                    if (!(instance = FindObjectOfType<OctoInputModule>()))
                    {
                        instance = new GameObject("OctoInputModule").AddComponent<OctoInputModule>();
                    }
                }

                return instance;
            }
        }

        protected override void OnDestroy()
        {
            isDestroyed = true;
        }

        public int AddPointer(UIPointer pointer)
        {
            if (!pointers.Contains(pointer))
            {
                pointers.Add(pointer);

                SetupPointerEventData();
            }

            return pointers.IndexOf(pointer);
        }

        public void RemovePointer(UIPointer pointer)
        {
            if (pointers.Contains(pointer))
            {
                pointers.Remove(pointer);
            }

            foreach (var point in pointers)
            {
                point.SetIndex(pointers.IndexOf(point));
            }

            SetupPointerEventData();
        }

        private void SetupPointerEventData()
        {
            pointerEventDatas = new PointerEventData[pointers.Count];

            for (int i = 0; i < pointerEventDatas.Length; i++)
            {
                pointerEventDatas[i] = new PointerEventData(eventSystem);
                pointerEventDatas[i].delta = Vector2.zero;
                pointerEventDatas[i].position = new Vector2(Screen.width / 2, Screen.height / 2);
            }
        }

        public override void Process()
        {
            for (int index = 0; index < pointers.Count; index++)
            {
                if (pointers[index] != null && pointers[index].enabled)
                {
                    pointers[index].Preprocess();

                    eventSystem.RaycastAll(pointerEventDatas[index], m_RaycastResultCache);
                    pointerEventDatas[index].pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                    HandlePointerExitAndEnter(pointerEventDatas[index], pointerEventDatas[index].pointerCurrentRaycast.gameObject);

                    ExecuteEvents.Execute(pointerEventDatas[index].pointerDrag, pointerEventDatas[index], ExecuteEvents.dragHandler);
                }
            }
        }

        public void ProcessSelect(int index)
        {
            pointers[index].Preprocess();

            pointerEventDatas[index].pointerPressRaycast = pointerEventDatas[index].pointerCurrentRaycast;

            pointerEventDatas[index].pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventDatas[index].pointerPressRaycast.gameObject);
            pointerEventDatas[index].pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(pointerEventDatas[index].pointerPressRaycast.gameObject);

            ExecuteEvents.Execute(pointerEventDatas[index].pointerPress, pointerEventDatas[index], ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(pointerEventDatas[index].pointerDrag, pointerEventDatas[index], ExecuteEvents.beginDragHandler);
        }

        public void ProcessRelease(int index)
        {
            pointers[index].Preprocess();

            GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventDatas[index].pointerCurrentRaycast.gameObject);

            if (pointerEventDatas[index].pointerPress == pointerRelease)
            {
                ExecuteEvents.Execute(pointerEventDatas[index].pointerPress, pointerEventDatas[index], ExecuteEvents.pointerClickHandler);
            } 
            
            if (pointerEventDatas.Length <= index || isDestroyed) return;

            Cancel(index);
        }

        public void ProcessCancel(int index)
        {
            pointers[index].Preprocess();

            Cancel(index);
        }

        private void Cancel(int index)
        {
            ExecuteEvents.Execute(pointerEventDatas[index].pointerPress, pointerEventDatas[index], ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(pointerEventDatas[index].pointerDrag, pointerEventDatas[index], ExecuteEvents.endDragHandler);

            pointerEventDatas[index].pointerPress = null;
            pointerEventDatas[index].pointerDrag = null;

            pointerEventDatas[index].pointerCurrentRaycast.Clear();
        }
        
        public PointerEventData GetPointerEventData(int index) => pointerEventDatas[index];
    }
}
