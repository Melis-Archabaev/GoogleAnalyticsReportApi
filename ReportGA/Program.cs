using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
namespace ReportGA
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var credential = GetCredential().Result;
                using(var svc=new AnalyticsReportingService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer=credential,
                        ApplicationName="MyExample"
                    }))
                {
                    var dateRange = new DateRange
                    {
                        StartDate = "2021-08-15",
                        EndDate = "2016-08-31"
                    };
                    var session = new Metric
                    {
                        Expression = "ga:sessions",
                        Alias = "Sessions"
                    };
                    var date = new Dimension { Name = "ga:date" };
                    var reportRequest = new ReportRequest
                    {
                        DateRanges = new List<DateRange> { dateRange },
                        Dimensions = new List<Dimension> { date },
                        Metrics = new List<Metric> { session },
                        ViewId = "<<your view id"
                    };
                    var getReportsRequest = new GetReportsRequest
                    {
                        ReportRequests = new List<ReportRequest> { reportRequest }
                    };
                    var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                    var response = batchRequest.Execute();
                    foreach(var x in response.Reports.First().Data.Rows)
                    {
                        Console.WriteLine(string.Join(", ", x.Dimensions) + " " + string.Join(", ", x.Metrics.First().Values));
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        static async Task<UserCredential> GetCredential()
        {
            using(var stream=new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                const string loginEmailAddress = "melis99mrx@gmail.com";

                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { AnalyticsReportingService.Scope.Analytics },
                    loginEmailAddress, CancellationToken.None,
                    new FileDataStore("GoogleAnalyticsApiConsole"));
            }
        }
    }
}
