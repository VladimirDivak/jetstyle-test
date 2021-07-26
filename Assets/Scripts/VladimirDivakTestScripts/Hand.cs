using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private GameObject angleVisualization;
    private LineRenderer _uiLine;
    [SerializeField]
    private RuneEffect runeEffect;

    private Animator _animator;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Transform _transform;
    private Camera _mainCamera;

    private Coroutine C_spellMode;
    private Coroutine C_resetPos;

    private List<Vector2> _runeArray = new List<Vector2>();

    public bool isSpellMode { get; private set; }

    private Runes _runes;
    private Vector3 _runeEffectPos;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _uiLine = angleVisualization.GetComponent<LineRenderer>();
        
        _runes = FindObjectOfType<Runes>();
        _runes.OnGetRuneEvent += OnGetRune;

        _transform = transform;
        _startPosition = _transform.localPosition;
        _startRotation = _transform.localRotation;

        _mainCamera = Camera.main;
    }

    private void OnGetRune(string runeName)
    {
        if(runeName == string.Empty)
        {
            Debug.LogWarning("такой руны нет");
        }
        else
        {
            RuneColor runeColor;
            int runeTextureIndex = 0;

            if(runeName.Contains("fire"))
                runeColor = RuneColor.Fire;
            else if(runeName.Contains("air"))
                runeColor = RuneColor.Air;
            else if(runeName.Contains("water"))
                runeColor = RuneColor.Water;
            else
                runeColor = RuneColor.Earth;

            switch(runeColor)
            {
                case RuneColor.Fire:
                    if(runeName.Contains("1"))
                        runeTextureIndex = 0;
                    else if(runeName.Contains("2"))
                        runeTextureIndex = 1;
                    else
                        runeTextureIndex = 2;
                break;
                case RuneColor.Air:
                    if(runeName.Contains("1"))
                        runeTextureIndex = 3;
                    else if(runeName.Contains("2"))
                        runeTextureIndex = 4;
                    else
                        runeTextureIndex = 5;
                break;
                case RuneColor.Water:
                    if(runeName.Contains("1"))
                        runeTextureIndex = 6;
                    else if(runeName.Contains("2"))
                        runeTextureIndex = 7;
                    else
                        runeTextureIndex = 8;
                break;
                case RuneColor.Earth:
                    if(runeName.Contains("1"))
                        runeTextureIndex = 9;
                    else if(runeName.Contains("2"))
                        runeTextureIndex = 10;
                    else
                        runeTextureIndex = 11;
                break;
            }

            var effect = Instantiate(runeEffect, _runeEffectPos, Quaternion.identity);
            effect.StartRuneAnimation(runeColor, runeTextureIndex);
        }
    }

    public void SetMovingAnimation(bool isMove)
    {
        _animator.SetBool("Move", isMove);
    }

    public void SetSpellMode(bool isActive)
    {
        if(isActive)
        {
            _animator.enabled = false;
            _uiLine.enabled = true;

            if(C_spellMode != null)
            {
                StopCoroutine(C_spellMode);
                C_spellMode = null;
            }
            if(C_resetPos != null)
            {
                StopCoroutine(C_resetPos);
                C_resetPos = null;
            }

            C_spellMode = StartCoroutine(SpellModeRoutine());
            isSpellMode = true;
        }
        else
        {
            StopCoroutine(C_spellMode);
            C_spellMode = null;
            isSpellMode = false;

            C_resetPos = StartCoroutine(ResetPosRoutine());

            _runes.GetRune(_runeArray.ToArray());

            _runeArray = null;
            _runeArray = new List<Vector2>();

            _uiLine.enabled = false;
        }
    }

    private Vector3 MouseWorldPosition()
    {
        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.75f);
        return _mainCamera.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator SpellModeRoutine()
    {
        var firstMousePos = Input.mousePosition;
        var lastMousePos = Vector3.zero;
        var maxOffsetValue = 50;

        _runeArray.Add(Vector2.zero);

        _uiLine.positionCount = 1;
        _uiLine.SetPosition(0, MouseWorldPosition());

        int iterations = 0;

        while(true)
        {
            var currentMousePos = Input.mousePosition - firstMousePos;
            var newPoint = new Vector3();

            var mouseWorldPos = MouseWorldPosition();
            _transform.position = mouseWorldPos;

            if(iterations == 10)
            {
                _uiLine.positionCount++;
                _uiLine.SetPosition(_uiLine.positionCount - 1, mouseWorldPos);

                if(Mathf.Abs(lastMousePos.x - currentMousePos.x) <= maxOffsetValue)
                    newPoint.x = lastMousePos.x;
                else
                    newPoint.x = currentMousePos.x;

                if(Mathf.Abs(lastMousePos.y - currentMousePos.y) <= maxOffsetValue)
                    newPoint.y = lastMousePos.y;
                else
                    newPoint.y = currentMousePos.y;

                _runeArray.Add(newPoint);

                lastMousePos = newPoint;
                iterations = 0;
            }

            iterations++;
            yield return null;
        }
    }

    private IEnumerator ResetPosRoutine()
    {
        Vector3 currentPosition = _transform.localPosition;
        Quaternion currentRotation = _transform.localRotation;
        _runeEffectPos = _transform.position;

        float lerpProc = 0;

        while(lerpProc < 1)
        {
            _transform.localPosition = Vector3.Lerp(currentPosition, _startPosition, lerpProc);
            _transform.localRotation = Quaternion.Lerp(currentRotation, _startRotation, lerpProc);

            lerpProc += 10 * Time.deltaTime;

            yield return null;
        }

        _animator.enabled = true;
        yield break;
    }
}