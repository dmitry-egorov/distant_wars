using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MeshRenderer))]
public class SelectionBox : RequiredSingleton<SelectionBox>
{
    [Header("Auto")]
    public MeshRenderer Renderer;
}