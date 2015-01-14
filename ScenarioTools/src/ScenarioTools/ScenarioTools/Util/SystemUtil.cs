using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Threading;

namespace ScenarioTools.Util
{
    public static class SystemUtil
    {
        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static void ExecuteCommandSync(object command)
        // from: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C            
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                //// The following commands are needed to redirect the standard output.
                //// This means that it will be redirected to the Process.StandardOutput StreamReader.
                //procStartInfo.RedirectStandardOutput = true;
                //procStartInfo.UseShellExecute = false;
                //// Do not create the black window.
                //procStartInfo.CreateNoWindow = true;
                //// Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                //// Get the output into a string
                //string result = proc.StandardOutput.ReadToEnd();
                //// Display the command output.
                //Console.WriteLine(result);
                ////return result;
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static void ExecuteCommandSyncAndWait(object command)
        // from: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C            
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                //// The following commands are needed to redirect the standard output.
                //// This means that it will be redirected to the Process.StandardOutput StreamReader.
                //procStartInfo.RedirectStandardOutput = true;
                //procStartInfo.UseShellExecute = false;
                //// Do not create the black window.
                //procStartInfo.CreateNoWindow = true;
                //// Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                //// Get the output into a string
                //string result = proc.StandardOutput.ReadToEnd();
                //// Display the command output.
                //Console.WriteLine(result);
                ////return result;
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        /// <summary>
        /// Execute the command Asynchronously.
        /// </summary>
        /// <param name="command">string command.</param>
        public static void ExecuteCommandAsync(string command)
        // from: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C            
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                // Log the exception
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        /// <summary>
        /// Executes a shell command synchronously and redirects output to console.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static void ExecuteCommandSyncRedirect(object command)
        // from: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C            
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
                //return result;
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        /// <summary>
        /// Execute the command Asynchronously and redirects output to console.
        /// </summary>
        /// <param name="command">string command.</param>
        public static void ExecuteCommandAsyncRedirect(string command)
        // from: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C            
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSyncRedirect));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                // Log the exception
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        public static int NumberProcessorCores()
        {
            // ProcessorCount is number of logical processors, but if processor supports
            // hypterthreading, this number is 2 * number of cores.  Still, it's better
            // than nothing.
            int numLogicalProcessors = Environment.ProcessorCount;
            try
            {
                int coreCount = 0;
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
                if (coreCount == 0)
                {
                    coreCount = 1;
                }
                return coreCount;
            }
            catch
            {
                return numLogicalProcessors;
            }
        }
    
    }
}
