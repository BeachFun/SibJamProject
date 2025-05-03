using UnityEngine;
using TMPro;

public class Activator : MonoBehaviour
{
    [Tooltip("Дистанция взаимодействия")]
    [SerializeField] private float m_interactionDistance = 5f;
    [Tooltip("Клавиша активации")]
    [SerializeField] private KeyCode m_activationKey = KeyCode.E;
    [SerializeField] private LayerMask m_layerMask_E;
    [Tooltip("UI-элемент с текстом подсказки")]
    [SerializeField] private GameObject _hintGUI;

    private Camera _mainCamera;
    private ActivatableObject _currentActivatable;

    public GameObject HintUI
    {
        get => _hintGUI;
        set
        {
            _hintGUI = value;
        }
    }


    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        CheckForActivatable();
    }

    private void Update()
    {
        if (_currentActivatable is null) return;

        if (Input.GetKeyDown(m_activationKey))
        {
            _currentActivatable.Activate();
        }
    }

    private void CheckForActivatable()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, m_interactionDistance, m_layerMask_E))
        {
            var activatable = hit.collider.GetComponent<ActivatableObject>();

            if (activatable != null)
            {
                if (_currentActivatable != activatable)

                {
                    ClearCurrentActivatable();
                    _currentActivatable = activatable;

                    PositionHintUI(hit.collider.transform);
                }
            }
            else
            {
                ClearCurrentActivatable();
            }
        }
        else
        {
            ClearCurrentActivatable();
        }

    }

    /// <summary>
    /// Ставит подсказку над объектом
    /// </summary>
    /// <param name="target"></param>
    private void PositionHintUI(Transform target)
    {
        if (_hintGUI is null) return;

        _hintGUI.SetActive(true);
        Vector3 worldPosition = target.position + Vector3.up * .4f;
        _hintGUI.transform.position = worldPosition;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 directionToCamera = _hintGUI.transform.position - mainCamera.transform.position;
            directionToCamera.y = 0;
            _hintGUI.transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }

    private void ClearCurrentActivatable()
    {
        if (_currentActivatable != null)
        {
            _currentActivatable.Highlight(false);
            _currentActivatable = null;
            _hintGUI.SetActive(false);
        }
    }
}
