using Akka.Actor;
using Akka.Routing;
using Vauth.Network.P2P.Payloads;
using System;

namespace Vauth.Ledger
{
    internal class TransactionRouter : UntypedActor
    {
        public record Preverify(Transaction Transaction, bool Relay);
        public record PreverifyCompleted(Transaction Transaction, bool Relay, VerifyResult Result);

        private readonly VauthSystem system;

        public TransactionRouter(VauthSystem system)
        {
            this.system = system;
        }

        protected override void OnReceive(object message)
        {
            if (message is not Preverify preverify) return;
            system.Blockchain.Tell(new PreverifyCompleted(preverify.Transaction, preverify.Relay, preverify.Transaction.VerifyStateIndependent(system.Settings)), Sender);
        }

        internal static Props Props(VauthSystem system)
        {
            return Akka.Actor.Props.Create(() => new TransactionRouter(system)).WithRouter(new SmallestMailboxPool(Environment.ProcessorCount));
        }
    }
}
