﻿namespace WinRMSharp
{
    /// <summary>
    /// Encapsulates WinRM network protocol for sending/receiving SOAP requests/responses necessary
    /// for executing remote commands.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Network transport used for sending/receiving SOAP requests/responses.
        /// </summary>
        ITransport Transport { get; }

        /// <summary>
        /// Maximum allowed time in seconds for any single wsman HTTP operation
        /// </summary>
        TimeSpan OperationTimeout { get; }

        /// <summary>
        /// Maximum response size in bytes. 
        /// </summary>
        int MaxEnvelopeSize { get; }

        /// <summary>
        /// Open a shell instance on the destination host.
        /// </summary>
        /// <param name="inputStream">Input streams to open.</param>
        /// <param name="outputStream">Output streams to open.</param>
        /// <param name="workingDirectory">Working directory of shell.</param>
        /// <param name="environment">Environment variables of shell.</param>
        /// <param name="idleTimeout">Time length before shell is closed, if unused.</param>
        /// <param name="codePage">Encoding of the output std buffers. Correlates to the codepage of the host. en-US traditionally maps to 437.</param>
        /// <param name="noProfile">Whether to create the shell with the user profile active or not.</param>
        /// <returns>Identifier for opened shell.</returns>
        Task<string> OpenShell(string inputStream = "stdin", string outputStream = "stdout stderr", string? workingDirectory = null, Dictionary<string, string>? environment = null, TimeSpan? idleTimeout = null, int? codePage = null, bool? noProfile = null);

        /// <summary>
        /// Run a command in an opened shell on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="command">The command to run on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="args">Array of arguments for the command.</param>
        /// <param name="timeout">The maximum allowed time for the command.</param>
        /// <returns>Identifier for the executing command.</returns>
        Task<string> RunCommand(
            string shellId,
            string command,
            string[]? args = null,
            TimeSpan? timeout = null);

        /// <summary>
        /// Send input to a command executing in a shell.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="commandId">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="input">Input text to send.</param>
        /// <param name="end">Boolean flag to indicate to close the stdin stream.</param>
        Task SendCommandInput(string shellId, string commandId, string input, bool end = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="commandId">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="timeout">The maximum allowed time to poll for the command state.</param>
        Task<CommandState> PollCommandState(
            string shellId,
            string commandId,
            TimeSpan? timeout = null);

        /// <summary>
        /// Gets the latest state of a command executing in a shell.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="commandId">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        /// <param name="timeout">The maximum allowed time to get the command state.</param>
        /// <returns>State of a executing command.</returns>
        Task<CommandState> GetCommandState(
            string shellId,
            string commandId,
            TimeSpan? timeout = null);

        /// <summary>
        /// Cleans up an executed command on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        /// <param name="commandId">The command id on the remote machine. See <see cref="RunCommand"/>.</param>
        Task CloseCommand(string shellId, string commandId);

        /// <summary>
        /// Close a shell on the destination host.
        /// </summary>
        /// <param name="shellId">The shell id on the remote machine. See <see cref="OpenShell"/>.</param>
        Task CloseShell(string shellId);

        /// <summary>
        /// Retrieves identification information from the remote machine.
        /// </summary>
        /// <returns>
        /// The identification information.
        /// </returns>
        Task<IdentifyResponse> Identify();
    }
}
