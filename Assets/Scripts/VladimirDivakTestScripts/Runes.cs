using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Runes : MonoBehaviour
{
    [SerializeField]
    private Texture2D[] runesMask;

    private Vector2[] _pointsArray;
    private string[] _runesNames = new string[]
    {
        "fire1",    "fire2",    "fire3",
        "air1",     "air2",     "air3",
        "water1",   "water2",   "water3",
        "earth1",   "earth2",   "earth3"
    };
    private float[] _blackPixelsValue;
    public event Action<string> OnGetRuneEvent;
    private Coroutine C_GetRune;

    void Awake()
    {
        _blackPixelsValue = new float[runesMask.Length];
        int count = 0;

        foreach(var texture in runesMask)
        {
            var black = 0;

            for(int y = 0; y < texture.height; y++)
            {
                for(int x = 0; x < texture.width; x++)
                {
                    var color = texture.GetPixel(x, y);
                    if(color != Color.white) black++;
                }
            }
            
            _blackPixelsValue[count] = black;
            count++;
        }
    }

    public void GetRune(Vector2[] pointsArray)
    {
        if(C_GetRune != null)
        {
            StopCoroutine(C_GetRune);
            C_GetRune = null;
        }

        C_GetRune = StartCoroutine(GetRuneRoutine(pointsArray));
    }

    private IEnumerator GetRuneRoutine(Vector2[] pointsArray)
    {
        _pointsArray = PointsArrayFormater(pointsArray);

        float currentBlackMaxValue = 0.0f;
        float currentWhiteMinValue = 100.0f;

        int runeIndex = -1;
        string rune = string.Empty;

        for (int i = 0; i < runesMask.Length; i++)
        {
            int blackCount = 0;
            int whiteCount = 0;

            foreach (var point in _pointsArray)
            {
                var color = runesMask[i].GetPixel((int)point.x, (int)point.y);
                if (color != Color.white)blackCount++;
                else whiteCount++;
            }

            if(whiteCount > blackCount)
                continue;
            
            var blackValue = (float)blackCount * 100 / (float)_blackPixelsValue[i];
            var whiteValue = (float)whiteCount * 100 / (float)_pointsArray.Length;

            if(whiteValue > 20 || blackValue < 4)
                continue;

            Debug.Log($"{_runesNames[i]}: процент охвата чёрного: {blackValue}%, процент белого: {whiteValue}%");

            if(blackValue > currentBlackMaxValue && whiteValue <= currentWhiteMinValue)
            {
                currentBlackMaxValue = blackValue;
                currentWhiteMinValue = whiteValue;
                runeIndex = i;
            }
        }

        if(runeIndex != -1) rune = _runesNames[runeIndex];
        OnGetRuneEvent?.Invoke(rune);

        yield break;
    }

    private Vector2[] PointsArrayFormater(Vector2[] source)
    {
        Vector2[] newPointsFormat = new Vector2[source.Length];

        int[] valueX = new int[source.Length];
        int[] valueY = new int[source.Length];

        int cropCoef = 1;

        for(int i = 0; i < source.Length; i++)
        {
            valueX[i] = (int)source[i].x;
            valueY[i] = (int)source[i].y;
        }

        int maximum = 0;
        int maximumX = valueX.Max(x => Math.Abs(x));
        int maximumY = valueY.Max(y => Math.Abs(y));

        if(maximumX > maximumY) maximum = maximumX;
        else maximum = maximumY;

        if(maximum > 31)
            cropCoef = maximum / 31;

        for (int i = 0; i < source.Length; i++)
        {
            valueX[i] /= cropCoef;
            valueY[i] /= cropCoef;
        }

        for(int i = 0; i < newPointsFormat.Length; i++)
        {
            newPointsFormat[i].x = valueX[i];
            newPointsFormat[i].y = valueY[i];
        }

        var valueOffsetX = Mathf.Abs(valueX.Min());
        var valueOffsetY = Mathf.Abs(valueY.Min());

        for(int i = 0; i < source.Length; i++)
        {
            newPointsFormat[i].x += valueOffsetX;
            newPointsFormat[i].y += valueOffsetY;
        }
        
        newPointsFormat.Distinct();
        return newPointsFormat;
    }
}
