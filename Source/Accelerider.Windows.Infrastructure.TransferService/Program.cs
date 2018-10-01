﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class Program
    {
        public static async Task Main()
        {
            var downloader = FileTransferService.GetFileDownloaderBuilder()
                .UseDefaultConfigure()
                .Build()
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODE1NTkxNyIsImV4cCI6IjE1MzgxNTk1MTciLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik4yUXhaVFJtTkdZdE1qZzBaaTAwWW1SaExUbGxNamt0WlRVeFl6RmlNRFkyWlRZdyIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.VFplRnVtVllDdTdyc3VScHdkU0FFWEFiOVhZRkliRXlYa05zenlqdnNpdz0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODM3MzU3NiIsImV4cCI6IjE1MzgzNzcxNzYiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik1ERXdZVEF5Wm1FdE9ERXlaUzAwWldNMkxUZzBNell0WTJSaU56VmlZelF4WlRSayIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.NTg1WWszUUhhTEdRZ0FsdVlPWnpVVEwyTmREVWpHUkhGQzJMTENBMU1CTT0&ApiVersion=2.0")
                .From("https://accelerider-my.sharepoint.com/personal/cs02_onedrive_accelerider_com/_layouts/15/download.aspx?UniqueId=b8a04e28-cbe7-46b6-a7e9-ff1dc364539e&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvYWNjZWxlcmlkZXItbXkuc2hhcmVwb2ludC5jb21AMjZmYTQ2ZDYtNDA3YS00YjMwLWJmMjYtOTEwZmFhMjZiZGQ2IiwiaXNzIjoiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIiwibmJmIjoiMTUzODM3NzkwOCIsImV4cCI6IjE1MzgzODE1MDgiLCJlbmRwb2ludHVybCI6ImZPVjloMFdhOFlLT3hNVVhOM0w4RDhySXBnVWVvYkt0ZTI1TVg2UUgrWkU9IiwiZW5kcG9pbnR1cmxMZW5ndGgiOiIxNjQiLCJpc2xvb3BiYWNrIjoiVHJ1ZSIsImNpZCI6Ik5ESmhNRFV4T0dRdE1qZ3pNUzAwWkRVNUxUaGpPVFV0WVRZMk0yTmlOR0l3T0RneCIsInZlciI6Imhhc2hlZHByb29mdG9rZW4iLCJzaXRlaWQiOiJaVGxpWXpsaVltSXROVFkyTWkwMFlqazNMVGd6TVdNdFl6ZzFNMkk1TkRobU0yTmkiLCJhcHBfZGlzcGxheW5hbWUiOiJBY2NlbGVyaWRlciIsInNpZ25pbl9zdGF0ZSI6IltcImttc2lcIl0iLCJhcHBpZCI6ImIyZjY2NTg0LTBhZGMtNDEzNS1hOTMwLTdiZjQ2YmM3YzdkNCIsInRpZCI6IjI2ZmE0NmQ2LTQwN2EtNGIzMC1iZjI2LTkxMGZhYTI2YmRkNiIsInVwbiI6ImNzMDJAb25lZHJpdmUuYWNjZWxlcmlkZXIuY29tIiwicHVpZCI6IjEwMDMwMDAwQTQyRUM5QjEiLCJzY3AiOiJhbGxmaWxlcy53cml0ZSBhbGxwcm9maWxlcy5yZWFkIiwidHQiOiIyIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbH0.SFRLS3c5cWZtL2NZL3VhbG5Eb1U4SS9XeVduZ2g0bVEzdk5tREZQN2lNND0&ApiVersion=2.0")
                .To(@"C:\Users\Dingp\Desktop\DownloadTest\download-multi-thread.rmvb");

            var disposable1 = downloader.SubscribeReport();

            WriteLine("Enter ant key to Start downloader: ");
            ReadKey(true);
            var cancellationTokenSource = new CancellationTokenSource();
            WriteLine($"The status of this downloader is {downloader.Status}. ");
            WriteLine("Try to ActivateAsync... ");
            await downloader.ActivateAsync(cancellationTokenSource.Token);
            WriteLine($"The status of this downloader is {downloader.Status}. ");

            await TimeSpan.FromSeconds(20);
            WriteLine("Try to Suspend... ");
            downloader.Suspend();
            WriteLine($"The status of this downloader is {downloader.Status}. ");
            var json = downloader.ToJson();
            WriteLine("Try to Dispose... ");
            downloader.Dispose();
            WriteLine($"The status of this downloader is {downloader.Status}. ");

            await TimeSpan.FromSeconds(5);

            var downloader2 = FileTransferService.GetFileDownloaderBuilder()
                .UseDefaultConfigure()
                .Build()
                .FromJson(json);

            downloader2.SubscribeReport();

            WriteLine($"The status of this downloader2 is {downloader2.Status}. ");
            WriteLine("Try to ActivateAsync... ");
            await downloader2.ActivateAsync(cancellationTokenSource.Token);
            WriteLine($"The status of this downloader2 is {downloader2.Status}. ");

            //FileTransferService
            //    .GetFileDownloaderBuilder()
            //    .UseDefaultConfigure()
            //    .Build()
            //    .FromJson(json);

            //await TimeSpan.FromMilliseconds(5000);
            //WriteLine("downloader has been disposed. ");
            //downloader.Dispose();

            ReadKey();
        }

        private static IObservable<(string flag, long number)> GenerateObservable(int flagNumber)
        {
            return Observable.Create<(string flag, long number)>(o =>
            {
                var source = new CancellationTokenSource();
                var token = source.Token;
                token.Register(() => WriteLine("Do something on cancelled. "));


                var flag = $"FLAG - {flagNumber}";
                Task.Run(async () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        if (token.IsCancellationRequested) break;
                        o.OnNext((flag, i));
                    }

                    o.OnCompleted();
                }, token);
                return () => source.Cancel();
            });
        }
    }

    public static class Extensions
    {
        public static IDisposable SubscribeReport(this IDownloader @this)
        {
            const long oneM = 1024 * 1024;
            const long blockSize = 1024 * 1024 * 20;

            var previousDateTimeOffset = DateTimeOffset.Now;
            var previousCompletedSize = 0L;

            return @this
                .Sample(TimeSpan.FromMilliseconds(500))
                .Timestamp()
                .Subscribe(timestampedContext =>
                {
                    var timestamp = timestampedContext.Timestamp;
                    var notification = timestampedContext.Value;
                    var completedSize = @this.GetCompletedSize();

                    WriteLine($"{notification.Id:B}: " +
                              $"{@this.BlockContexts[notification.Id].Offset / blockSize:D3} --> {@this.BlockContexts[notification.Id].TotalSize / blockSize} " +
                              $"{1.0 * (completedSize - previousCompletedSize) / (timestamp - previousDateTimeOffset).TotalSeconds / oneM:00.00} MB/s " +
                              $"{100.0 * completedSize / @this.Context.TotalSize: 00.00}% " +
                              $"{completedSize:D9}/{@this.Context.TotalSize}");

                    previousCompletedSize = completedSize;
                    previousDateTimeOffset = timestamp;
                }, () =>
                {
                    WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                });
        }
    }
}
