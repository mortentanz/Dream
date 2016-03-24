using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Dream.Data
{
  /// <summary>
  /// Provides static general purpose utility and helper methods for the Data Access Layer.
  /// </summary>
  public class Utility
  {

    /// <summary>
    /// Generic method for serialising object instances.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="o">The object to be serialized.</param>
    /// <param name="s">The Stream to serialize to.</param>
    public static void XmlSerializeToStream<T>(object o, Stream s)
    {
      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        xs.Serialize(s, o);
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Generic method for deserializing an object from Xml.
    /// </summary>
    /// <typeparam name="T">The type to deserialize.</typeparam>
    /// <param name="s">A stream containing the Xml representation of the object being deserialized.</param>
    /// <returns>The deserialized object.</returns>
    public static T XmlDeserializeFromStream<T>(Stream s)
    {
    #if DEBUG
      XmlElementEventHandler handler = new XmlElementEventHandler(OnUnknownElementEvent);
      return XmlDeserializeFromStream<T>(s, handler);
    #else
      return XmlDeserializeFromStream<T>(s, null);
    #endif
    }

    /// <summary>
    /// Generic method for deserialized an object from Xml. Allows for handling of unknown element events.
    /// </summary>
    /// <typeparam name="T">The type to deserialize.</typeparam>
    /// <param name="s">A stream containing the Xml representation of the object being deserialized.</param>
    /// <param name="unknownElementHandler">An XmlElementEventHandler delegate (may be null).</param>
    /// <returns>The deserialized object.</returns>
    public static T XmlDeserializeFromStream<T>(Stream s, XmlElementEventHandler unknownElementHandler)
    {
      object o = null;
      try
      {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        if (unknownElementHandler != null)
        {
          ser.UnknownElement += unknownElementHandler;
        }
        o = ser.Deserialize(s);
        return (T)o;
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// Default Event Handler for UnknownElementEvent raised by XmlSerializers.
    /// </summary>
    /// <param name="sender">The XmlSerializer raising the UnknownElement Event.</param>
    /// <param name="e">The XmlEventArgs object wrapping details about the element causing the event.</param>
    static void OnUnknownElementEvent(object sender, XmlElementEventArgs e)
    {
      Debug.WriteLine("Unknown Element event occurred while deserializing from stream.");
      Debug.WriteLine("Details: ");
      Debug.IndentLevel++;
      Debug.WriteLine(string.Format("ObjectBeingDeserialized : {0}", e.ObjectBeingDeserialized.ToString()));
      Debug.WriteLine(string.Format("Element : {0}", e.Element));
      Debug.WriteLine(string.Format("Line, position : {0}, {1}", e.LineNumber.ToString(), e.LinePosition.ToString()));
      Debug.IndentLevel--;
    }

  }
}
