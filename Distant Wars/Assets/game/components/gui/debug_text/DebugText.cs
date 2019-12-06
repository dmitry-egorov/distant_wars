using System;
using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Text))]
public class DebugText : RequiredSingleton<DebugText>
{
    public static void set_text(string category, string text) => Instance.set_text2(category, text);
    
    private void set_text2(string category, string text)
    {
        if (messages == null) messages = new Dictionary<string, string>();
        messages[category] = text;

        Text.text = generate_text();
        Text.enabled = true;
    }

    private string generate_text() => string.Join("\n", messages.OrderBy(x => x.Key).Select(x => $"{x.Key}: {x.Value}"));

    public void disable() 
    {
        messages?.Clear();
        Text.enabled = false;
    }

    Text Text => text != null ? text : text = GetComponent<Text>();
    Text text;

    Dictionary<string, string> messages;
}
