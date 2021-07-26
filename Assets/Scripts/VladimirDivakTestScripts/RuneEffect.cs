using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RuneColor
{
    Fire,
    Air,
    Water,
    Earth
}

public class RuneEffect : MonoBehaviour
{
    [SerializeField]
    private Texture2D[] Runes;
    private Transform _transform;
    private Material _material;

    private string _emissiveColor = "Color_0df9faf3d67a48d0a88f2cd033a18af3";
    private string _texture = "Texture2D_c847c8050ab6461bbc7bc9dd20ac82d1";

    public void StartRuneAnimation(RuneColor color, int textureIndex)
    {
        _transform = transform;
        _material = GetComponent<MeshRenderer>().material;

        Color emisseveColor = new Color();

        switch(color)
        {
            case RuneColor.Fire:
                emisseveColor = Color.red;
                break;
            case RuneColor.Air:
                emisseveColor = Color.white;
                break;
            case RuneColor.Water:
                emisseveColor = Color.blue;
                break;
            case RuneColor.Earth:
                emisseveColor = Color.green;
                break;
        }

        _material.SetColor(_emissiveColor, emisseveColor);
        _material.SetTexture(_texture, Runes[textureIndex]);

        StartCoroutine(AnimationRoutine(emisseveColor));
    }

    IEnumerator AnimationRoutine(Color matColor)
    {
        Vector3 startPos = _transform.position;
        Vector3 endPos = Camera.main.transform.forward * 0.2f;

        Color color = new Color();

        Color startColor = matColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        float lerpProc = 0;
        while (lerpProc < 1)
        {
            _transform.position = Vector3.Lerp(startPos, endPos, lerpProc);
            color = Color.Lerp(startColor, endColor, lerpProc);
            _material.SetColor(_emissiveColor,color);

            lerpProc += 0.25f * Time.deltaTime;
            yield return null;
        }
    }
}