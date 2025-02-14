using Vauth.Persistence;
using Vauth.SmartContract;
using Vauth.SmartContract.Native;
using Vauth.VM;
using System;

namespace Vauth.Wallets
{
    /// <summary>
    /// Represents the descriptor of an asset.
    /// </summary>
    public class AssetDescriptor
    {
        /// <summary>
        /// The id of the asset.
        /// </summary>
        public UInt160 AssetId { get; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        /// The symbol of the asset.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// The number of decimal places of the token.
        /// </summary>
        public byte Decimals { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetDescriptor"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <param name="settings">The <see cref="ProtocolSettings"/> used by the <see cref="ApplicationEngine"/>.</param>
        /// <param name="asset_id">The id of the asset.</param>
        public AssetDescriptor(DataCache snapshot, ProtocolSettings settings, UInt160 asset_id)
        {
            var contract = NativeContract.ContractManagement.GetContract(snapshot, asset_id);
            if (contract is null) throw new ArgumentException(null, nameof(asset_id));

            byte[] script;
            using (ScriptBuilder sb = new())
            {
                sb.EmitDynamicCall(asset_id, "decimals", CallFlags.ReadOnly);
                sb.EmitDynamicCall(asset_id, "symbol", CallFlags.ReadOnly);
                script = sb.ToArray();
            }
            using ApplicationEngine engine = ApplicationEngine.Run(script, snapshot, settings: settings, valt: 0_10000000);
            if (engine.State != VMState.HALT) throw new ArgumentException(null, nameof(asset_id));
            this.AssetId = asset_id;
            this.AssetName = contract.Manifest.Name;
            this.Symbol = engine.ResultStack.Pop().GetString();
            this.Decimals = (byte)engine.ResultStack.Pop().GetInteger();
        }

        public override string ToString()
        {
            return AssetName;
        }
    }
}
