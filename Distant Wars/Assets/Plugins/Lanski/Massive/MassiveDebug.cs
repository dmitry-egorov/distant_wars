using System;

public static class MassiveDebug
{
    public static Action<string> Action;

    public static void Display(string text) => Action?.Invoke(text);
}