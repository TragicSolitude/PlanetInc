﻿using System;
using System.Data;
using System.Data.SqlClient;

public class Reservation
{
    private int clientId;
    private int regionId;
    private DateTime startDate;
    private DateTime endDate;

    private SqlConnection connection =
        new SqlConnection("Server=IISProject.mtchs.org;Database=PlanetInc;Trusted_Connection=Yes");

    public string message;
    public Boolean error = true;

    public Reservation(int clientId, int regionId, DateTime startDate, DateTime endDate)
    {
        this.clientId = clientId;
        this.regionId = regionId;
        this.startDate = startDate;
        this.endDate = endDate;

        connection.Open();

        if (!IsValid())
        {
            message = "Start date must occur before end date";
        }
        else if (!IsReserved())
        {
            message = "Sorry, that region is already reserved. Please choose a different time.";
        }
        else
        {
            Reserve();
            message = "Your reservation has been made!";
            error = false;
        }

        connection.Close();
    }

    private Boolean IsValid()
    {
        return startDate <= endDate;
    }

    private void Reserve()
    {
        var cmd = new SqlCommand(null, connection);

        cmd.CommandText = "INSERT INTO Reservations (ClientId, RegionId, StartDate, EndDate) " +
                          "VALUES (@client, @region, @start, @end);";

        var clientParameter = new SqlParameter("@client", SqlDbType.Int);
        var regionParameter = new SqlParameter("@region", SqlDbType.Int);
        var startParam = new SqlParameter("@start", SqlDbType.Date);
        var endParam = new SqlParameter("@end", SqlDbType.Date);
        clientParameter.Value = clientId;
        regionParameter.Value = regionId;
        startParam.Value = startDate;
        endParam.Value = endDate;

        cmd.Parameters.Add(clientParameter);
        cmd.Parameters.Add(regionParameter);
        cmd.Parameters.Add(startParam);
        cmd.Parameters.Add(endParam);

        cmd.ExecuteNonQuery();
    }

    private Boolean IsReserved()
    {
        var cmd = new SqlCommand(null, connection);

        cmd.CommandText = "SELECT COUNT(*) " +
                          "FROM Reservations " +
                          "WHERE RegionId = @region AND (" +
                          "@start <= EndDate OR " +
                          "@end >= StartDate OR " +
                          "@start = StartDate AND @end = EndDate);";

        var regionParameter = new SqlParameter("@region", SqlDbType.Int);
        var startParam = new SqlParameter("@start", SqlDbType.Date);
        var endParam = new SqlParameter("@end", SqlDbType.Date);
        regionParameter.Value = regionId;
        startParam.Value = startDate;
        endParam.Value = endDate;

        cmd.Parameters.Add(regionParameter);
        cmd.Parameters.Add(startParam);
        cmd.Parameters.Add(endParam);

        return (int) cmd.ExecuteScalar() != 0;
    }
}