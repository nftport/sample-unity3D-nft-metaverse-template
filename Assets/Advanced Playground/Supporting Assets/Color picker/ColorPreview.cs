
using UnityEngine;
using UnityEngine.UI;

public class ColorPreview : MonoBehaviour
{
    public Graphic previewGraphic;
    [SerializeField] private Light potionLght;

    public ColorPicker colorPicker;

    private void Start()
    {
        previewGraphic.color = colorPicker.color;
        colorPicker.onColorChanged += OnColorChanged;
    }

    public void OnColorChanged(Color c)
    {
        previewGraphic.color = c;
        potionLght.color = c;
    }

    private void OnDestroy()
    {
        if (colorPicker != null)
            colorPicker.onColorChanged -= OnColorChanged;
    }
}