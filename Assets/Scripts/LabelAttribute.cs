using UnityEngine;

public class LabelAttribute : PropertyAttribute
{
    public string Name { get; }

    public LabelAttribute(string name)
    {
        Name = name;
    }
}