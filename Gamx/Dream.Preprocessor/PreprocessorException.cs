using System;
using System.IO;
using System.Runtime.Serialization;

namespace Dream.Preprocessor
{
  /// <summary>
  /// Reprensents an application exception specific to the preprocessor.
  /// </summary>
  [Serializable]
  public class PreprocessorException : ApplicationException
  {
    public PreprocessorException() : base() { }
    public PreprocessorException(string message) : base(message) { }
    public PreprocessorException(string message, Exception innerException) : base(message, innerException) { }
    protected PreprocessorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
