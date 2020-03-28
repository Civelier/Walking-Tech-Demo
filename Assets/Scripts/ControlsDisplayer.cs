using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Assets.FieldGridElements;
using Assets.DataBinding;
using Assets.GameMenu;
using UnityEditor;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class ControlsDisplayer : MonoBehaviour
{
    public RectTransform RectTransform;
    public InputSettings Input;
    public FieldGrid Grid;
    public InputActionAsset Map;
    public InputDisplayer[] Displayers = new InputDisplayer[0];
    public string[] Maps;
    List<IDataBinder<InputData>> _binders = new List<IDataBinder<InputData>>();
    List<ValueTypeData<InputData>> _data = new List<ValueTypeData<InputData>>();
    List<ReadonlyControlField> _uiData = new List<ReadonlyControlField>();

    // Start is called before the first frame update
    void Start()
    {
        if (RectTransform == null) RectTransform = GetComponent<RectTransform>();
        if (Grid == null) Grid = GetComponentInChildren<FieldGrid>();
        Refresh();
        InputSystem.onDeviceChange += OnDeviceChange;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        Application.quitting += Application_quitting;
    }

    private void Application_quitting()
    {
        Clear();
        InputSystem.onDeviceChange -= OnDeviceChange;
        Application.quitting -= Application_quitting;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode) Application_quitting();
    }
#endif

    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        Clear();
        Refresh();
    }

    void Clear()
    {
        Grid.Clear();
        foreach (var b in _binders)
        {
            b.Unbind();
        }
        _binders.Clear();
        _data.Clear();
        _uiData.Clear();
    }

    void MatchInputDisplayer(ReadonlyControlField field)
    {
        foreach (var c in Displayers)
        {
            if (c.Reference.action == field.Data.Action)
            {
                field.SetDisplayer(c);
            }
        }
    }

    void Refresh()
    {
        foreach (var action in Map)
        {
            if (Maps.Contains(action.actionMap.name))
            {
                var data = new ValueTypeData<InputData>();
                var uiData = FieldFactory.Instance.InstantiateReadonlyControlField(Grid, action);
                _binders.Add(DataBinderUtillities.Bind(data, uiData));
                MatchInputDisplayer(uiData);
                uiData.DataChanged.Binder.OnUIDataChanged();
                _data.Add(data);
                _uiData.Add(uiData);
                Grid.Add(uiData);
            }
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Application_quitting();
        }
    }
#endif

    private void OnDestroy()
    {
        Clear();
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
