namespace Innoactive.CreatorEditor.ImguiTester
{
    /// <summary>
    /// State of <see cref="EditorImguiTest{T}"/>
    /// </summary>
    internal enum TestState
    {
        Normal,
        Pending,
        Failed,
        Passed,
    }
}