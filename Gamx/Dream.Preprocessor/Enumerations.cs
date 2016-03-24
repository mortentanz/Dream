using System;


namespace Dream.Preprocessor
{

  /// <summary>
  /// Represents various types of invocations of the GAMS executable.
  /// </summary>
  public enum ExecutionAction
  {
    CompileAndExecute,
    CompileOnly,
    ExecuteOnly,
    GenerateGlueCode,
    RestartAfterSolve,
    Trace
  }

  /// <summary>
  /// Determines how the GAMS compiler resolves undefined control variable symbols.
  /// </summary>
  public enum StringCheckOption
  {
    NoSubstitution,
    Error,
    Remove
  }

}