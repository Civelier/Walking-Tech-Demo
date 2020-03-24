using Assets.GameMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class GameMenuHandler : MonoBehaviour, IMenu
{
    public Walking Walking;
    public GameObject MainPanel;
    public GameObject BasePanel;
    public ButtonMenuBind[] Binds;
    public InputActionReference Escape;
    public InputActionReference Back;
    public InputActionReference Select;
    public Button Quit;

    bool _justGotFocus = false;

    IMenu _current;
    public IMenu Current
    {
        get => _current;
        set
        {
            _current?.Hide();
            _current = value;
            _current?.Show();
        }
    }
    public bool IsCurrent 
    { 
        get => Current.Equals(this);
        set
        {
            if (value)
            {
                Current = this;
            }
            else Unfocus();
        }
    }

    public void Unfocus()
    {
        MainPanel.SetActive(false);
        Current = null;
    }

    public void Focus()
    {
        MainPanel.SetActive(true);
        Current = this;
    }

    public void Hide()
    {
        BasePanel.gameObject.SetActive(false);
        _justGotFocus = false;
    }

    public void Show()
    {
        BasePanel.gameObject.SetActive(true);
        _justGotFocus = true;
    }

    void OnEscape(InputAction.CallbackContext obj)
    {
        if (Current == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Walking.enabled = false;
            Focus();
            _justGotFocus = false;
            return;
        }
        if (IsCurrent)
        {
            if (_justGotFocus) _justGotFocus = false;
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Walking.enabled = true;
                Unfocus();
            }
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _current = this;
        foreach (var bind in Binds)
        {
            bind.Initialize();
            bind.Hide();
        }
        var menu = Walking.Input.actions["Menu"];
        menu.Enable();
        menu.performed += OnEscape;
        //Escape.action.performed += OnEscape;
#if UNITY_EDITOR
        Quit.onClick.AddListener(() => UnityEditor.EditorApplication.isPlaying = false);
#else
        Quit.onClick.AddListener(Application.Quit);
#endif
        Unfocus();
    }
}
