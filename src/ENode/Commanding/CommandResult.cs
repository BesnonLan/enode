﻿using System;

namespace ENode.Commanding
{
    /// <summary>Represents a command result.
    /// </summary>
    [Serializable]
    public class CommandResult
    {
        /// <summary>The status of the command.
        /// </summary>
        public CommandStatus Status { get; private set; }
        /// <summary>The unique identifier of the command.
        /// </summary>
        public string CommandId { get; private set; }
        /// <summary>The aggregate root created or modified by the command.
        /// </summary>
        public string AggregateRootId { get; private set; }
        /// <summary>The code of exception if the command execution has any exception.
        /// </summary>
        public int ExceptionCode { get; private set; }
        /// <summary>The error message of the command result.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>Parameterized constructor.
        /// </summary>
        public CommandResult(CommandStatus commandStatus, string commandId, string aggregateRootId, int exceptionCode, string errorMessage)
        {
            Status = commandStatus;
            CommandId = commandId;
            AggregateRootId = aggregateRootId;
            ExceptionCode = exceptionCode;
            ErrorMessage = errorMessage;
        }
    }
    /// <summary>Represents the command processing status.
    /// </summary>
    public enum CommandStatus
    {
        Success = 1,
        NothingChanged = 2,
        Failed,
    }
}
