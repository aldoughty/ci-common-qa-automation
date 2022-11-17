using System.Diagnostics;
using System.Text;
using ci_common_qa_automation.BaseObjects;
using FluentAssertions;

namespace ci_common_qa_automation.Utilities
{
    public static class CommandExecuter
    {
        public static string ExecuteCommand(ProcessStartInfo processStartInfo, int waitTime, string callingMethod)
        {
            int processReturnCode;
            StringBuilder outputBuilder = new();
            processStartInfo.RedirectStandardOutput = true;
            try
            {
                using Process process = new();
                process.StartInfo = processStartInfo;
                process.OutputDataReceived +=
                (
                    delegate (object sender, DataReceivedEventArgs e)
                    {
                        // append the new data to the data already read-in
                        outputBuilder.AppendLine(e.Data);
                    }
                );
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit(waitTime).Should().BeTrue("The command '" + processStartInfo.FileName + " " + processStartInfo.Arguments + "' did not complete in " + waitTime / 1000 + " seconds.");
                processReturnCode = process.ExitCode;
                process.CancelOutputRead();
                process.Close();
            }
            catch (Exception e)
            {
                throw new AutomationFrameworkException(e.Message, e.InnerException, callingMethod);
            }
            processReturnCode.Should().Be(0, "The command '" + processStartInfo.FileName + " " + processStartInfo.Arguments + "' failed with unexpected error code.");
            return outputBuilder.ToString();
        }
        public static async void ExecuteCommand(ProcessStartInfo processStartInfo, string callingMethod)
        {
            StringBuilder outputBuilder = new();
            processStartInfo.RedirectStandardOutput = true;
            try
            {
                using Process process = new();
                process.StartInfo = processStartInfo;
                process.OutputDataReceived +=
                (
                    delegate (object sender, DataReceivedEventArgs e)
                    {
                            // append the new data to the data already read-in
                        outputBuilder.AppendLine(e.Data);
                    }
                );
                process.Start();
                process.BeginOutputReadLine();
                await process.WaitForExitAsync();
            }
            catch (Exception e)
            {
                throw new AutomationFrameworkException(e.Message, e.InnerException, callingMethod);
            }
        }

        private static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (process.HasExited) return Task.CompletedTask;

            TaskCompletionSource<object> tcs = new();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(() => tcs.SetCanceled());

            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }
    }
}
