using Plugins.Lanski;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugText : RequiredSingleton<DebugText>
{
    public void set_text(string t)
    {
        Text.text = t;
        Text.enabled = true;
    }

    public void disable() => Text.enabled = false;

    Text Text => text != null ? text : text = GetComponent<Text>();
    Text text;
}
