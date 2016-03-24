using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Text;
using Dream.Data.Demographics;

namespace Dream.Data
{
  /// <summary>
  /// Provides static methods for retrieving and saving data in database storage.
  /// </summary>
  public partial class DBStorage
  {


    #region Save methods
    /// <summary>
    /// Saves projection results to storage.
    /// </summary>
    /// <param name="info">A ProjectionInfo object characterizing the projection.</param>
    /// <param name="population">An array containing the projected population levels.</param>
    /// <param name="deaths">An array containing the projected number of deaths.</param>
    /// <param name="births">An array containing the projected number of births.</param>
    /// <param name="mothers">An array containing the projected number of mothers.</param>
    /// <param name="children">An array containing the projected number of children by age of mothers.</param>
    /// <param name="heirs">An array containing the projected number of potential heirs by age of mothers.</param>
    /// <param name="immigrants">An array containing the projected number of immigrants.</param>
    /// <param name="emigrants">An array containing the projected number of emigrants.</param>
    /// <param name="residenceduration">An array containing the projected stock of immigrants by duration of residence.</param>
    /// <param name="replace">Boolean indicating if an existing projection with same title should be replaced.</param>
    /// <param name="worker">A BackgroundWorker object for reporting on progress (may be null).</param>
    public static void SaveProjection(
      ProjectionInfo info, 
      Array population,
      Array deaths,
      Array births,
      Array mothers,
      Array children,
      Array heirs,
      Array immigrants,
      Array emigrants,
      Array residenceduration,
      bool replace,
      BackgroundWorker worker
    )
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving projection information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save projection information to catalog, see innerException.", sqle);
      }
      
      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        DataTable table;        
        SqlBulkCopy bcp = new SqlBulkCopy(conn);
        bcp.BulkCopyTimeout = 60 * 5;

        if(worker != null) worker.ReportProgress(1, "Saving population...");
        table = FillDataTable(info, ProjectionResultType.Population, population);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving deaths...");
        table = FillDataTable(info, ProjectionResultType.Deaths, deaths);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving births...");
        table = FillDataTable(info, ProjectionResultType.Births, births);
        bcp.DestinationTableName = table.TableName;
        bcp.ColumnMappings.Add("OriginID", "ChildOriginID");
        bcp.ColumnMappings.Add("GenderID", "ChildGenderID");
        bcp.WriteToServer(table);
        
        if(worker != null) worker.ReportProgress(1, "Saving mothers...");
        table = FillDataTable(info, ProjectionResultType.Mothers, mothers);
        bcp.DestinationTableName = table.TableName;
        bcp.ColumnMappings.Add("GenderID", "ChildGenderID");
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving children...");
        table = FillDataTable(info, ProjectionResultType.Children, children);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving heirs...");
        table = FillDataTable(info, ProjectionResultType.Children, heirs);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving immigrants...");
        table = FillDataTable(info, ProjectionResultType.Immigrants, immigrants);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving emigrants...");
        table = FillDataTable(info, ProjectionResultType.Emigrants, emigrants);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);

        if(worker != null) worker.ReportProgress(1, "Saving residence duration...");
        table = FillDataTable(info, ProjectionResultType.ResidenceDuration, residenceduration);
        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);
      }

      if (worker != null) worker.ReportProgress(1, "Saved projection results to database.");
    }

    #region Save forecast methods
    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">An ImmigrationInfo object characterizing the forecast.</param>
    /// <param name="forecast">An array of forecasted values.</param>
    /// <param name="worker">A BackgroundWorker object for reporting on progress (may be null).</param>
    public static void SaveForecast(ImmigrationInfo info, double[, , ,] data, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving immigration forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save immigration forecast information to catalog, see innerException.", sqle);
      }

      short id = info.CatalogEntry.DatabaseID;
      short startYear = (short)info.Series.StartYear;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        DataTable table = new DataTable();
        SqlBulkCopy bcp = new SqlBulkCopy(conn);
        bcp.BulkCopyTimeout = 60 * 5;

        if (worker != null) worker.ReportProgress(1, "Saving immigration forecast...");

        table.TableName = "Demographics.ForecastedImmigration" ;
        table.Columns.Add("EstimateID", typeof(Int64));
        table.Columns.Add("ForecastID", typeof(short));
        table.Columns.Add("OriginID", typeof(byte));
        table.Columns.Add("GenderID", typeof(byte));
        table.Columns.Add("Age", typeof(byte));
        table.Columns.Add("Year", typeof(short));
        table.Columns.Add("Estimate", typeof(double));

        for (int t = 0; t < data.GetLength(3); t++)
          for (int g = 0; g < data.GetLength(1); g++)
            for (int a = 0; a < data.GetLength(0); a++)
              for (int o = 0; o < data.GetLength(2); o++)
              {
                Double value = data[a,g,o,t];
                if (value != 0)
                {
                  DataRow row = table.NewRow();
                  row["ForecastID"] = (short)id;
                  row["OriginID"] = (byte)(o + 1);
                  row["GenderID"] = (byte)(g + 1);
                  row["Age"] = (byte)a;
                  row["Year"] = (short)(startYear + t);
                  row["Estimate"] = value;
                  table.Rows.Add(row);
                }
              }

        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);
      }

      if (worker != null) worker.ReportProgress(1, "Saved immigration forecast result to database.");
    }

    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">An EmigrationInfo object characterizing the forecast.</param>
    /// <param name="forecast">An array of forecasted values.</param>
    /// <param name="worker">A BackgroundWorker object for reporting on progress (may be null).</param>
    public static void SaveForecast(EmigrationInfo info, double[, , ,] data, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving emigration forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save emigration forecast information to catalog, see innerException.", sqle);
      }

      short id = info.CatalogEntry.DatabaseID;
      short startYear = (short)info.Series.StartYear;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        DataTable table = new DataTable();
        SqlBulkCopy bcp = new SqlBulkCopy(conn);
        bcp.BulkCopyTimeout = 60 * 5;

        if (worker != null) worker.ReportProgress(1, "Saving emigration forecast...");

        table.TableName = "Demographics.ForecastedEmigration";
        table.Columns.Add("EstimateID", typeof(Int64));
        table.Columns.Add("ForecastID", typeof(short));
        table.Columns.Add("OriginID", typeof(byte));
        table.Columns.Add("GenderID", typeof(byte));
        table.Columns.Add("Age", typeof(byte));
        table.Columns.Add("Year", typeof(short));
        table.Columns.Add("Estimate", typeof(double));

        for (int t = 0; t < data.GetLength(3); t++)
          for (int g = 0; g < data.GetLength(1); g++)
            for (int a = 0; a < data.GetLength(0); a++)
              for (int o = 0; o < data.GetLength(2); o++)
              {
                Double value = data[a, g, o, t];
                if (value != 0)
                {
                  DataRow row = table.NewRow();
                  row["ForecastID"] = (short)id;
                  row["OriginID"] = (byte)(o + 1);
                  row["GenderID"] = (byte)(g + 1);
                  row["Age"] = (byte)a;
                  row["Year"] = (short)(startYear + t);
                  row["Estimate"] = value;
                  table.Rows.Add(row);
                }
              }

        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);
      }

      if (worker != null) worker.ReportProgress(1, "Saved emigration forecast result to database.");
    }

    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">A MortalityInfo object characterizing the forecast.</param>
    /// <param name="forecast">An array of forecasted values.</param>
    public static void SaveForecast(MortalityInfo info, double[, , ,] data, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving mortality forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save mortality forecast information to catalog, see innerException.", sqle);
      }

      short id = info.CatalogEntry.DatabaseID;
      short startYear = (short)info.Series.StartYear;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        DataTable table = new DataTable();
        SqlBulkCopy bcp = new SqlBulkCopy(conn);
        bcp.BulkCopyTimeout = 60 * 5;

        if (worker != null) worker.ReportProgress(1, "Saving mortality forecast...");

        table.TableName = "Demographics.ForecastedMortality";
        table.Columns.Add("EstimateID", typeof(Int64));
        table.Columns.Add("ForecastID", typeof(short));
        table.Columns.Add("OriginID", typeof(byte));
        table.Columns.Add("GenderID", typeof(byte));
        table.Columns.Add("Age", typeof(byte));
        table.Columns.Add("Year", typeof(short));
        table.Columns.Add("Estimate", typeof(double));

        for (int t = 0; t < data.GetLength(3); t++)
          for (int g = 0; g < data.GetLength(1); g++)
            for (int a = 0; a < data.GetLength(0); a++)
              for (int o = 0; o < data.GetLength(2); o++)
              {
                Double value = data[a, g, o, t];
                if (value != 0)
                {
                  DataRow row = table.NewRow();
                  row["ForecastID"] = (short)id;
                  row["OriginID"] = (byte)(o + 1);
                  row["GenderID"] = (byte)(g + 1);
                  row["Age"] = (byte)a;
                  row["Year"] = (short)(startYear + t);
                  row["Estimate"] = value;
                  table.Rows.Add(row);
                }
              }

        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);
      }

      if (worker != null) worker.ReportProgress(1, "Saved mortality forecast result to database.");
    }

    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">A FertilityInfo object characterizing the forecast.</param>
    /// <param name="forecast">An array of forecasted values.</param>
    public static void SaveForecast(FertilityInfo info, double[, , ,] data, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving fertility forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save fertility forecast information to catalog, see innerException.", sqle);
      }

      short id = info.CatalogEntry.DatabaseID;
      short startYear = (short)info.Series.StartYear;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        DataTable table = new DataTable();
        SqlBulkCopy bcp = new SqlBulkCopy(conn);
        bcp.BulkCopyTimeout = 60 * 5;

        if (worker != null) worker.ReportProgress(1, "Saving fertility forecast...");

        table.TableName = "Demographics.ForecastedFertility";
        table.Columns.Add("EstimateID", typeof(Int64));
        table.Columns.Add("ForecastID", typeof(short));
        table.Columns.Add("OriginID", typeof(byte));
        table.Columns.Add("GenderID", typeof(byte));
        table.Columns.Add("Age", typeof(byte));
        table.Columns.Add("Year", typeof(short));
        table.Columns.Add("Estimate", typeof(double));

        for (int t = 0; t < data.GetLength(3); t++)
          for (int g = 0; g < data.GetLength(1); g++)
            for (int a = 0; a < data.GetLength(0); a++)
              for (int o = 0; o < data.GetLength(2); o++)
              {
                Double value = data[a, g, o, t];
                if (value != 0)
                {
                  DataRow row = table.NewRow();
                  row["ForecastID"] = (short)id;
                  row["OriginID"] = (byte)(o + 1);
                  row["GenderID"] = (byte)(g + 1);
                  row["Age"] = (byte)a;
                  row["Year"] = (short)(startYear + t);
                  row["Estimate"] = value;
                  table.Rows.Add(row);
                }
              }

        bcp.DestinationTableName = table.TableName;
        bcp.WriteToServer(table);
      }

      if (worker != null) worker.ReportProgress(1, "Saved fertility forecast result to database.");
    }

    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">A NaturalizationInfo object characterizing the forecast.</param>
    /// <param name="forecast">An array of forecasted values.</param>
    public static void SaveForecast(NaturalizationInfo info, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving naturalization forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save naturalization forecast information to catalog, see innerException.", sqle);
      }

      if (worker != null) worker.ReportProgress(1, "Saved naturalization forecast result to database.");
    }

    /// <summary>
    /// Saves a forecast result to storage.
    /// </summary>
    /// <param name="info">A BirthInfo object characterizing a birth forecast.</param>
    public static void SaveForecast(BirthInfo info, bool replace, BackgroundWorker worker)
    {
      try
      {
        if (worker != null) worker.ReportProgress(1, "Saving birth forecast information...");
        DBCatalog.Save(info, replace);
      }
      catch (SqlException sqle)
      {
        throw new ApplicationException("Failed to save birth forecast information to catalog, see innerException.", sqle);
      }

      if (worker != null) worker.ReportProgress(1, "Saved birth forecast result to database.");
    }
    #endregion

    #region Save estimation methods
    /// <summary>
    /// Saves an estimation result to storage.
    /// </summary>
    /// <param name="info">An ImmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="estimates">An array of estimates.</param>
    public static void SaveEstimation(ImmigrationEstimationInfo info, Array estimates)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Saves an estimation result to storage.
    /// </summary>
    /// <param name="info">An EmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="estimates">An array of estimates.</param>
    public static void SaveEstimation(EmigrationEstimationInfo info, Array estimates)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Saves an estimation result to storage.
    /// </summary>
    /// <param name="info">A MortalityEstimationInfo object characterizing the estimation.</param>
    /// <param name="estimates">An array of estimates.</param>
    public static void SaveEstimation(MortalityEstimationInfo info, Array estimates)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Saves an estimation result to storage.
    /// </summary>
    /// <param name="info">A FertilityEstimationInfo object charachterizing the estimation.</param>
    /// <param name="estimates">An array of estimates.</param>
    public static void SaveEstimation(FertilityEstimationInfo info, Array estimates)
    {
      throw new Exception("The method is not implemented yet.");
    }
    #endregion
    #endregion

    #region Projection load methods
    /// <summary>
    /// Loads initial population from storage.
    /// </summary>
    /// <param name="info">A ProjectionInfo object characterizing the projection.</param>
    /// <param name="population">An array containing the initial population.</param>
    public static void LoadInitialPopulation(ProjectionInfo info, out Array population)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">An ImmigrationInfo object characterizing the forecast.</param>
    /// <param name="immigration">An array of forecasted immigration.</param>
    /// <param name="durations">An array of observed average residence durations for immigrants.</param>
    public static void LoadForecast(ImmigrationInfo info, out Array immigration, out Array durations)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">An EmigrationInfo object characterizing the forecast.</param>
    /// <param name="emigration">An array of forecasted emigration.</param>
    public static void LoadForecast(EmigrationInfo info, out Array emigration)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">A NaturalizationInfo object characterizing the forecast.</param>
    /// <param name="naturalization">An array of observed average naturalization rates.</param>
    public static void LoadForecast(NaturalizationInfo info, out Array naturalization)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">A MortalityInfo object characterizing the forecast.</param>
    /// <param name="mortality">An array of forecasted mortality rates.</param>
    public static void LoadForecast(MortalityInfo info, out Array mortality)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">A FertilityInfo object characterizing the forecast.</param>
    /// <param name="fertility">An array of forecasted fertility.</param>
    public static void LoadForecast(FertilityInfo info, out Array fertility)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads forecasted values from storage.
    /// </summary>
    /// <param name="info">A BirthInfo object characterizing the forecast.</param>
    /// <param name="origin">An array containing the origin rates of children given parent origin.</param>
    /// <param name="naturalization">An array containing the naturalization rates of newborn children.</param>
    /// <param name="motherage">An array containing the age distribution of mothers.</param>
    public static void LoadForecast(BirthInfo info, out Array origin, out Array naturalization, out Array motherage)
    {
      throw new Exception("The method is not implemented yet.");
    }
    #endregion

    #region Forecast load methods
    /// <summary>
    /// Loads estimates from storage.
    /// </summary>
    /// <param name="info">A MortalityEstimationInfo characterizing the estimation.</param>
    /// <param name="mortality">An array of estimated mortality rates.</param>
    public static void LoadEstimation(MortalityEstimationInfo info, out Array mortality)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads estimates from storage.
    /// </summary>
    /// <param name="info">A FertilityEstimationInfo characterizing the estimation.</param>
    /// <param name="fertility">An array of estimated fertility.</param>
    public static void LoadEstimation(FertilityEstimationInfo info, out Array fertility)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads estimates from storage.
    /// </summary>
    /// <param name="info">An ImmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="immigration">An array of estimated immigration.</param>
    public static void LoadEstimation(ImmigrationEstimationInfo info, out Array immigration)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads estimates from storage.
    /// </summary>
    /// <param name="info">An EmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="emigration">An array of estimated emigration.</param>
    public static void LoadEstimation(EmigrationEstimationInfo info, out Array emigration)
    {
      throw new Exception("The method is not implemented yet.");
    }
    #endregion

    #region Estimation input load methods
    /// <summary>
    /// Loads immigration levels by type of permit.
    /// </summary>
    /// <param name="info">An ImmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="immigration">An array of observed immigration levels.</param>
    public static void LoadImmigrationPermits(ImmigrationEstimationInfo info, out Array immigration)
    {
      throw new Exception("The method is not implemented yet.");
    }

    /// <summary>
    /// Loads emigration levels by type of permit.
    /// </summary>
    /// <param name="info">An EmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="emigration">An array of observed emigration levels.</param>
    public static void LoadEmigrationPermits(EmigrationEstimationInfo info, out Array emigration)
    {
      throw new Exception("The method is not implemented yet.");
    }

    #endregion

    #region Methods for loading data tables for bulk copy operations
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="resultType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static DataTable FillDataTable(ProjectionInfo info, ProjectionResultType resultType, Array data) 
    {
      int[] indx = new int[data.Rank];
      
      DataTable table = new DataTable();
      table.TableName = "Demographics." + resultType.ToString();
      string idcol = resultType.ToString() + "ID";
      table.Columns.Add(idcol, typeof(Int64));
      table.Columns.Add("ProjectionID", typeof(short));

      short startYear = (short)info.Series.StartYear;
      short id = info.CatalogEntry.DatabaseID;

      switch (resultType)
      {
        case ProjectionResultType.Children:
          table.Columns.Add("OriginID", typeof(byte));
          table.Columns.Add("GenderID", typeof(byte));
          table.Columns.Add("Age", typeof(byte));
          table.Columns.Add("MotherAge", typeof(byte));
          table.Columns.Add("Year", typeof(short));
          table.Columns.Add("Persons", typeof(double));

          for (int t = 0; t < data.GetLength(4); t++)
            for (int g = 0; g < data.GetLength(1); g++)
              for (int a = 0; a < data.GetLength(0); a++)
                for (int ma = 0; ma < data.GetLength(3); ma++)
                  for (int o = 0; o < data.GetLength(2); o++)
                  {
                    indx[0] = a; indx[1] = g; indx[2] = o; indx[3] = ma; ; indx[4] = t;
                    Double value = (double)data.GetValue(indx);

                    if (value != 0)
                    {
                      DataRow row = table.NewRow();
                      row["ProjectionID"] = (short)id;
                      row["OriginID"] = (byte)(o + 1);
                      row["GenderID"] = (byte)(g + 1);
                      row["Age"] = (byte)a;
                      row["MotherAge"] = (byte)ma;
                      row["Year"] = (short)(startYear + t);
                      row["Persons"] = value;
                      table.Rows.Add(row);
                    }
                  }
          break;

        case ProjectionResultType.Heirs:
          table.Columns.Add("GenderID", typeof(byte));
          table.Columns.Add("Age", typeof(byte));
          table.Columns.Add("MotherAge", typeof(byte));
          table.Columns.Add("Year", typeof(short));
          table.Columns.Add("Persons", typeof(double));

          byte heirsMinAge = info.BequestMinimumAge;

          for (int t = 0; t < data.GetLength(3); t++)
            for (int g = 0; g < data.GetLength(1); g++)
              for (int a = 0; a < data.GetLength(0); a++)
                for (int ma = 0; ma < data.GetLength(2); ma++)
                {
                  indx[0] = a; indx[1] = g; indx[2] = ma; indx[3] = t;
                  Double value = (double)data.GetValue(indx);

                  if (value != 0)
                  {
                    DataRow row = table.NewRow();
                    row["ProjectionID"] = (short)id;
                    row["GenderID"] = (byte)(g + 1);
                    row["Age"] = (byte)a;
                    row["MotherAge"] = (byte)(ma + heirsMinAge);
                    row["Year"] = (short)(startYear + t);
                    row["Persons"] = value;
                    table.Rows.Add(row);
                  }
                }
          break;

        case ProjectionResultType.ResidenceDuration:
          table.Columns.Add("OriginID", typeof(byte));
          table.Columns.Add("DurationID", typeof(short));
          table.Columns.Add("GenderID", typeof(byte));
          table.Columns.Add("Age", typeof(byte));
          table.Columns.Add("Year", typeof(short));
          table.Columns.Add("Persons", typeof(double));

          for (int t = 0; t < data.GetLength(4); t++)
            for (int g = 0; g < data.GetLength(1); g++)
              for (int a = 0; a < data.GetLength(0); a++)
                for (int d = 0; d < data.GetLength(3); d++)
                  for (int o = 0; o < data.GetLength(2); o++)
                  {
                    indx[0] = a; indx[1] = g; indx[2] = o; indx[3] = d; indx[4] = t;
                    Double value = (double)data.GetValue(indx);

                    if (value != 0)
                    {
                      DataRow row = table.NewRow();
                      row["ProjectionID"] = (short)id;
                      row["OriginID"] = (byte)(o + 5);
                      row["DurationID"] = (short)((d == data.GetLength(3) - 1) ? Int16.MaxValue : d);
                      row["GenderID"] = (byte)(g + 1);
                      row["Age"] = (byte)a;
                      row["Year"] = (short)(startYear + t);
                      row["Persons"] = value;
                      table.Rows.Add(row);
                    }
                  }
          break;

        default:
          table.Columns.Add("OriginID", typeof(byte));
          table.Columns.Add("GenderID", typeof(byte));
          table.Columns.Add("Age", typeof(byte));
          table.Columns.Add("Year", typeof(short));
          table.Columns.Add("Persons", typeof(double));
          
          for (int t = 0; t < data.GetLength(3); t++)
            for (int g = 0; g < data.GetLength(1); g++)
              for (int a = 0; a < data.GetLength(0); a++)
                for (int o = 0; o < data.GetLength(2); o++)
                {
                  indx[0] = a; indx[1] = g; indx[2] = o; indx[3] = t;
                  Double value = (double)data.GetValue(indx);
                  
                  if (value != 0)
                  {
                    DataRow row = table.NewRow();
                    row["ProjectionID"] = (short)id;
                    row["OriginID"] = (byte)(o + 1);
                    row["GenderID"] = (byte)(g + 1);
                    row["Age"] = (byte)a;
                    row["Year"] = (short)(startYear + t);
                    row["Persons"] = value;
                    table.Rows.Add(row);
                  }
                }
          break;
      }
      return table;
    }


    #region Projection Result Table Content
    private enum ProjectionResultType
    {
      Population,
      Deaths,
      Births,
      Mothers,
      Children,
      Heirs,
      Immigrants,
      Emigrants,
      ResidenceDuration
    }
    #endregion



    #endregion
  }
}
