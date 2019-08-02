namespace dotnetCampus.Cli.StateMachine
{
    /// <summary>
    /// 表示谓词匹配结果。
    /// </summary>
    internal enum VerbMatchingResult
    {
        /// <summary>
        /// 谓词没有匹配上。
        /// </summary>
        NotMatched,

        /// <summary>
        /// 谓词匹配成功。
        /// </summary>
        Matched,

        /// <summary>
        /// 谓词没有匹配上，但这是这个选项包含默认谓词。
        /// </summary>
        Default,
    }
}