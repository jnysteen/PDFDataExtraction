using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PDFDataExtraction.Exceptions;

namespace PDFDataExtraction.Helpers
{
    public static class CmdLineHelper
    {
        public static async Task<(int exitCode, string stdOutput, string errorOutput)> Run(string applicationName, string args, CancellationToken cancellationToken)
        {
            var escapedArgs = args.Replace("\"", "\\\"");

            using var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = applicationName,
                    Arguments = escapedArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            
            process.Start();

            var (stdOutput, errorOutput) = await ReadToEndAsyncWithCancellationToken(process, cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            
            return (process.ExitCode, stdOutput, errorOutput);
        }
        
        private static async Task<(string stdOutput, string errorOutput)> ReadToEndAsyncWithCancellationToken(Process process, CancellationToken cancellationToken)
        {
            // Code inspired by https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/#
            
            // We create a TaskCompletionSource of decimal
            var taskCompletionSource = new TaskCompletionSource<(string stdOutput, string errorOutput)>();

            // Registering a lambda into the cancellationToken
            cancellationToken.Register(() =>
            {
                // We received a cancellation message, cancel the TaskCompletionSource.Task
                taskCompletionSource.TrySetCanceled();
            });

            
            var stdOutputReadTask = process.StandardOutput.ReadToEndAsync();
            var stdErrorReadTask = process.StandardError.ReadToEndAsync();
            var aggregateReadTask = Task.WhenAll(stdOutputReadTask, stdErrorReadTask);

            // Wait for the first task to finish among the two
            var completedTask = await Task.WhenAny(aggregateReadTask, taskCompletionSource.Task);

            // If the completed task is our long running operation we set its result.
            if (completedTask == aggregateReadTask)
            {
                // Extract the result, the task is finished and the await will return immediately
                var stdOutput = await stdOutputReadTask;
                var stdError = await stdErrorReadTask;

                // Set the taskCompletionSource result
                taskCompletionSource.TrySetResult((stdOutput, stdError));
            }

            // Return the result of the TaskCompletionSource.Task
            return await taskCompletionSource.Task;
        }

    }
}