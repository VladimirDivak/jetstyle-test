using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Hand : MonoBehaviour
{
    [SerializeField]
    private GameObject angleVisualization;
    private LineRenderer _angleLine;

    private Animator _animator;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Transform _transform;
    private Camera _mainCamera;
    private LineRenderer _line;

    private Coroutine C_spellMode;
    private Coroutine C_resetPos;

    private List<Vector2> _runeArray = new List<Vector2>();

    public bool isSpellMode { get; private set; }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _angleLine = angleVisualization.GetComponent<LineRenderer>();

        _line = GetComponent<LineRenderer>();
        _line.enabled = false;

        _transform = transform;
        _startPosition = _transform.localPosition;
        _startRotation = _transform.localRotation;

        _mainCamera = Camera.main;
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

            StringBuilder runeData = new StringBuilder(string.Empty);

            _runeArray = null;
            _runeArray = new List<Vector2>();
        }
    }

    private Vector3 MouseWorldPosition()
    {
        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.75f);
        return _mainCamera.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator SpellModeRoutine()
    {
        int xLinesCount = 0;
        int yLinesCount = 0;

        float lineLenght = 0.2f;
        
        int offsetCoef = 90;
        float maxOffset = ((float)Screen.width / (float)Screen.height) * offsetCoef;
        
        var lastMousePos = Input.mousePosition;
        var startLinePos = MouseWorldPosition();
        var lastLinePos = MouseWorldPosition();

        _runeArray.Add(Vector2.zero);

        _line.enabled = true;
        _line.positionCount = 1;
        _line.SetPosition(_line.positionCount - 1, lastLinePos);

        _angleLine.positionCount = 1;
        _angleLine.SetPosition(0, startLinePos);

        int iterations = 0;
        
        Vector3 newLinePos = new Vector3();

        while(true)
        {
            var mouseWorldPos = MouseWorldPosition();
            _transform.position = mouseWorldPos;

            var newMousePos = Input.mousePosition - lastMousePos;

            var newMousePosAps = newMousePos;
            newMousePosAps.x = Mathf.Abs(newMousePos.x);
            newMousePosAps.y = Mathf.Abs(newMousePos.y);

            var line1 = new Vector2(lastLinePos.x, lastLinePos.y);
            var line2 = new Vector2(newMousePos.x, newMousePos.y);
            var angle = Vector2.Angle(line2, line1);

            if(iterations == 10)
            {
                _angleLine.positionCount++;
                _angleLine.SetPosition(_angleLine.positionCount - 1, mouseWorldPos);
                iterations = 0;
            }

            if(newMousePosAps.x >= maxOffset || newMousePosAps.y >= maxOffset)
            {
                if((angle >= 15 && angle <= 75) || (angle >= 115 && angle <= 165))
                {
                    var lineLocalPosIndex = _line.positionCount;

                    xLinesCount = 0;
                    yLinesCount = 0;

                    int minusX = 1;
                    int minusY = 1;

                    if(newMousePos.x < 0) minusX *= -1;
                    if(newMousePos.y < 0) minusY *= -1;

                    newLinePos = new Vector3(lastLinePos.x + lineLenght * minusX / 2,
                        lastLinePos.y + lineLenght * minusY / 2,
                        lastLinePos.z);
                    
                    if(lineLocalPosIndex == _line.positionCount)
                        _line.positionCount++;
                    _line.SetPosition(lineLocalPosIndex, newLinePos);

                    _runeArray.Add(new Vector2(newLinePos.x - startLinePos.x, newLinePos.y - startLinePos.y));

                    lastMousePos = Input.mousePosition;
                    lastLinePos = newLinePos;

                    continue;
                }

                if(newMousePosAps.x >= maxOffset && xLinesCount < 2)
                {
                    if(xLinesCount == 1)
                    {
                        if(newMousePos.x < 0) lineLenght *= -1;
                        newLinePos = new Vector3(lastLinePos.x + lineLenght, lastLinePos.y, lastLinePos.z);
                        _line.SetPosition(_line.positionCount - 1, newLinePos);

                        _runeArray.Remove(_runeArray.Last());
                        _runeArray.Add(new Vector2(newLinePos.x - startLinePos.x, newLinePos.y - startLinePos.y));

                        lastMousePos = Input.mousePosition;
                        lastLinePos = newLinePos;

                        xLinesCount++;
                        yLinesCount = 0;

                        lineLenght = Mathf.Abs(lineLenght);

                        continue;                  
                    }
                    else
                    {
                        if(newMousePos.x < 0) lineLenght *= -1;
                        newLinePos = new Vector3(lastLinePos.x + lineLenght, lastLinePos.y, lastLinePos.z);
                    }

                    xLinesCount++;
                    yLinesCount = 0;
                }

                else if(newMousePosAps.y >= maxOffset && yLinesCount < 2)
                {
                    if(yLinesCount == 1)
                    {
                        if(newMousePos.y < 0) lineLenght *= -1;
                        newLinePos = new Vector3(lastLinePos.x, lastLinePos.y + lineLenght, lastLinePos.z);
                        _line.SetPosition(_line.positionCount - 1, newLinePos);

                        _runeArray.Remove(_runeArray.Last());
                        _runeArray.Add(new Vector2(newLinePos.x - startLinePos.x, newLinePos.y - startLinePos.y));

                        lastMousePos = Input.mousePosition;
                        lastLinePos = newLinePos;

                        yLinesCount++;
                        xLinesCount = 0;

                        lineLenght = Mathf.Abs(lineLenght);

                        continue;                  
                    }
                    else
                    {
                        if(newMousePos.y < 0) lineLenght *= -1;
                        newLinePos = new Vector3(lastLinePos.x, lastLinePos.y + lineLenght, lastLinePos.z);
                    }

                    yLinesCount++;
                    xLinesCount = 0;
                }

                if(xLinesCount < 2 && yLinesCount < 2)
                {
                    _line.positionCount++;
                    _line.SetPosition(_line.positionCount - 1, newLinePos);

                    _runeArray.Add(new Vector2(newLinePos.x - startLinePos.x, newLinePos.y - startLinePos.y));

                    lastMousePos = Input.mousePosition;
                    lastLinePos = newLinePos;
                }

                lineLenght = Mathf.Abs(lineLenght);
            }
            
            iterations++;
            yield return null;
        }
    }

    private IEnumerator ResetPosRoutine()
    {
        Vector3 currentPosition = _transform.localPosition;
        Quaternion currentRotation = _transform.localRotation;

        float lerpProc = 0;

        while(lerpProc < 1)
        {
            _transform.localPosition = Vector3.Lerp(currentPosition, _startPosition, lerpProc);
            _transform.localRotation = Quaternion.Lerp(currentRotation, _startRotation, lerpProc);

            lerpProc += 10 * Time.deltaTime;

            yield return null;
        }

        _animator.enabled = true;

        _line.positionCount = 0;
        _line.enabled = false;
        yield break;
    }
}