using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{

  public class DAL
  {

    public static void SaveProjection(ProjectionInfo pi)
    {

      XmlSerializer xs = new XmlSerializer(typeof(ProjectionInfo));
      FileStream fs = new FileStream(@"F:\Work\ProjectionInfo.xml", FileMode.Create);
      xs.Serialize(fs, pi);
      fs.Close();

    }

    public static ProjectionInfo GetProjection(short projectionid)
    {
      XmlSerializer xs = new XmlSerializer(typeof(ProjectionInfo));
      xs.UnknownElement += new XmlElementEventHandler(xs_UnknownElement);
      FileStream fs = new FileStream(@"F:\Work\ProjectionInfo.xml", FileMode.Open);
      ProjectionInfo pi = (ProjectionInfo)xs.Deserialize(fs);
      fs.Close();
      return pi;
    }

    static void xs_UnknownElement(object sender, XmlElementEventArgs e)
    {
      Console.WriteLine("UnknownElement fired : ");
      Console.WriteLine(e.Element.Name);
      Console.WriteLine(e.Element.InnerText);
    }



    /*
    public static List<ProjectionInfo> GetProjections()
    {

      List<ProjectionInfo> list = new List<ProjectionInfo>();
      SqlCommand cmd = new SqlCommand("SELECT ProjectionID, Title, Revision, Parameters FROM Demographics.Projection");

      SqlDataReader r = cmd.ExecuteReader();
      while (r.Read())
      {
        
        int titlepos = r.GetOrdinal("Title");
        ProjectionInfo pi = new ProjectionInfo(r.GetString(titlepos));
        

      }

    }
    */

    public static ForecastInfo GetForecastInfo(short forecastid)
    {
      throw new NotImplementedException();
    }

    public static void SaveForecast(ForecastInfo fi)
    {
      throw new NotImplementedException();
    }

  }
}
