#region Source file version
// $Archive: $
// $Date: $
// $Revision: $ by 
// $Author: $
// 
// $Workfile: $
// $Modtime: $
#endregion
        
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data.Utility
{
  /// <summary>
  /// Provides a library of static utility methods for serialization of objects.
  /// </summary>
  public class Serialization
  {

    public static T XmlDeserialize<T>(Stream xmlStream)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      T o = (T)xs.Deserialize(xmlStream);
      return o;
    }

    public static T XmlDeserialize<T>(Stream xmlStream, XmlElementEventHandler unknownElementHandler)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      xs.UnknownElement += unknownElementHandler;
      T o = (T)xs.Deserialize(xmlStream);
      xmlStream.Close();
      return o;
    }


    /// <summary>
    /// Serializes an object or object graph to a memory stream using the simple object access protocol (Soap).
    /// </summary>
    /// <param name="o">The object or object graph to be serialized (must be Serializable).</param>
    /// <returns>A memory stream containing an object graph serialized using Soap.</returns>
    //public static Stream SoapSerializedStream(object o)
    //{
    //  try
    //  {
    //    Stream s = new MemoryStream();
    //    SoapFormatter sf = new SoapFormatter();
    //    sf.Serialize(s, o);
    //    return s;
    //  }
    //  catch
    //  {
    //    throw;
    //  }
    //}

    ///// <summary>
    ///// Serializes an object or object graph to a string using the simple object access protocol (Soap).
    ///// </summary>
    ///// <param name="o">The object or object graph to be serialized (must be Serializable).</param>
    ///// <param name="encoding">The encoding to use for the string.</param>
    ///// <returns>A string containing an object graph serialized using Soap.</returns>
    //public static string SoapSerializedString(object o, Encoding encoding)
    //{
    //  string x;
    //  using (Stream s = Serialization.SoapSerializedStream(o))
    //  {
    //    s.Position = 0;
    //    byte[] b = new byte[s.Length];
    //    s.Read(b, 0, (int)b.Length);
    //    x = encoding.GetString(b,0,b.Length);
    //  }
    //  return x;
    //}

    ///// <summary>
    ///// Generic method deserializing an object graph from a Soap serialized memory stream.
    ///// </summary>
    ///// <param name="s">The memory stream to deserialize from.</param>
    ///// <returns>The deserialized object or object graph (generic).</returns>
    //public static T SoapDeserialize<T>(Stream s)
    //{
    //  object o = null;
    //  try
    //  {
    //    SoapFormatter sf = new SoapFormatter();
    //    o = (object)sf.Deserialize(s);
    //    return (T)o;
    //  }
    //  catch
    //  {
    //    throw;
    //  }
    //}

    ///// <summary>
    ///// Generic method deserializing an object graph from a Soap serialized string.
    ///// </summary>
    ///// <param name="s">The string to deserialize from.</param>
    ///// <param name="encoding">The encoding to assume for the string.</param>
    ///// <returns>The deserialized object or object graph (generic).</returns>
    //public static T SoapDeserialize<T>(string s, Encoding encoding)
    //{
    //  try
    //  {
    //    byte[] b;
    //    b = encoding.GetBytes(s);
    //    Stream ms = new MemoryStream(b);
    //    return Serialization.SoapDeserialize<T>(ms);
    //  }
    //  catch
    //  {
    //    throw;
    //  }
    //}
  }
}
