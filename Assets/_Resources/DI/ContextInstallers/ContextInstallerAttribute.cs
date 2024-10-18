using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ContextInstallerAttribute : Attribute
{
    public ContextInstallerAttribute() { }
}
