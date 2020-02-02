using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using BlazorGrpc;
using System.Threading;

namespace Server.Services
{
    public class WeatherForecastsService : WeatherForecasts.WeatherForecastsBase
    {

        IServerStreamWriter<WeatherReply> _responseStream;
        IAsyncStreamReader<Empty> _requests;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override async Task GetWeather(IAsyncStreamReader<Empty> requests, IServerStreamWriter<WeatherReply> responseStream, ServerCallContext context)
        {

            Console.WriteLine("start");
            if (_responseStream == null)
            {
                _responseStream = responseStream;
                _requests = requests;
            }

            
            while (await _requests.MoveNext())
            {

                Trace("Receive data request");
                var reply = new WeatherReply();

                Trace("Creating data ");
                var rng = new Random();
                reply.Forecasts.Add(Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    DateTimeStamp = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(index)),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                }));
                await _responseStream.WriteAsync(reply);
                Trace("Data sent");
            }

        }
        private async void Trace(string message)
        {

            Console.WriteLine($"{DateTime.Now:0:HH:mm:ss,fff}\t{message}");
        }

    }
}
