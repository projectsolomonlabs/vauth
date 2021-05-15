using Vauth.Network.P2P.Payloads;
using Vauth.SmartContract;
using Vauth.VM;
using Vauth.VM.Types;
using System;
using System.Linq;

namespace Vauth.Ledger
{
    partial class Blockchain
    {
        partial class ApplicationExecuted
        {
            /// <summary>
            /// The transaction that contains the executed script. This field could be <see langword="null"/> if the contract is invoked by system.
            /// </summary>
            public Transaction Transaction { get; }

            /// <summary>
            /// The trigger of the execution.
            /// </summary>
            public TriggerType Trigger { get; }

            /// <summary>
            /// The state of the virtual machine after the contract is executed.
            /// </summary>
            public VMState VMState { get; }

            /// <summary>
            /// The exception that caused the execution to terminate abnormally. This field could be <see langword="null"/> if the execution ends normally.
            /// </summary>
            public Exception Exception { get; }

            /// <summary>
            /// VALT spent to execute.
            /// </summary>
            public long ValtConsumed { get; }

            /// <summary>
            /// Items on the stack of the virtual machine after execution.
            /// </summary>
            public StackItem[] Stack { get; }

            /// <summary>
            /// The notifications sent during the execution.
            /// </summary>
            public NotifyEventArgs[] Notifications { get; }

            internal ApplicationExecuted(ApplicationEngine engine)
            {
                Transaction = engine.ScriptContainer as Transaction;
                Trigger = engine.Trigger;
                VMState = engine.State;
                ValtConsumed = engine.ValtConsumed;
                Exception = engine.FaultException;
                Stack = engine.ResultStack.ToArray();
                Notifications = engine.Notifications.ToArray();
            }
        }
    }
}
