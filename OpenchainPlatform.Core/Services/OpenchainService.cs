using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Openchain.SDK;
using NBitcoin;
using OpenchainPlatform.Core.Services.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Openchain.SDK.Models;
using Newtonsoft.Json;
using System.Linq;

namespace OpenchainPlatform.Core.Services
{
    public class OpenchainService
    {
        private readonly OpenchainPlatformOptions _options;
        private readonly ILogger _logger;
        private readonly OpenchainSDK _sdk;
        private readonly Network _network;

        public OpenchainService(IOptions<OpenchainPlatformOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;

            _sdk = new OpenchainSDK(_options.ServerUrl);

            _network = _sdk.Network;

            _logger = loggerFactory.CreateLogger<OpenchainService>();
        }

        private ExtKey GetAdminKey()
        {
            var issuanceAccount = _options.IssuanceAccount;

            var mnemonic = new Mnemonic(issuanceAccount.Seed);

            var privateKey = mnemonic.DeriveExtKey(issuanceAccount.Passphrase);

            var rootKey = privateKey.Derive(0);

            return rootKey;
        }

        private ExtKey GetIssuanceKey(uint index)
        {
            var issuanceAccount = _options.IssuanceAccount;

            var mnemonic = new Mnemonic(issuanceAccount.Seed);

            var privateKey = mnemonic.DeriveExtKey(issuanceAccount.Passphrase);

            var rootKey = privateKey.Derive(index);

            return rootKey;
        }

        private ExtKey GetAccountKey(string seed, string passphrase, uint index)
        {
            var mnemonic = new Mnemonic(seed);

            var privateKey = mnemonic.DeriveExtKey(passphrase);
            var rootKey = privateKey.Derive(index);

            return rootKey;
        }

        public string AssetPathToAddress(string path)
        {
            return string.Empty;
        }

        public string AddressToAssetPath(string address)
        {
            //return $"/asset/p2pkh/{address}/";
            return $"/p2pkh/{address}/";
        }

        public string AccountPathToAddress(string path)
        {
            return string.Empty;
        }

        public string AddressToAccountPath(string address)
        {
            return $"/p2pkh/{address}/";
        }

        public async Task<AccountModel> CreateAccountAsync(string passphrase, string assetPath)
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            var privateKey = mnemonic.DeriveExtKey(passphrase);

            var rootKey = privateKey.Derive(0);

            var address = rootKey.PrivateKey.PubKey.GetAddress(_network);

            var account = new AccountModel
            {
                Seed = mnemonic.ToString(),
                Path = AddressToAccountPath(address.ToString()),
                Base58Address = address.ToString(),
                Chain = "0"
            };

            var dataRecord = await _sdk.Client.GetDataRecord(account.Path, "acl");

            if (dataRecord.Data == null)
            {
                var aclRecords = new List<AclRecord>();

                var receiveAclRecord = new AclRecord
                {
                    Subjects = new List<AclSubject>
                    {
                        new AclSubject
                        {
                            Addresses = new List<string>(),
                            Required = 0
                        }
                    }
                };

                receiveAclRecord
                    .AllowCreate()
                    .AllowModify();

                aclRecords.Add(receiveAclRecord);

                var sendAclRecord = new AclRecord
                {
                    Subjects = new List<AclSubject>
                    {
                        new AclSubject
                        {
                            Addresses = new List<string>
                            {
                                address.ToString()
                            },
                            Required = 1
                        }
                    }
                };

                sendAclRecord
                    .AllowSpend();

                aclRecords.Add(sendAclRecord);

                var aclData = JsonConvert.SerializeObject(aclRecords);

                var builder = _sdk.CreateTransactionBuilder();

                builder.AddRecord(dataRecord.Key, aclRecords, dataRecord.Version);

                builder.AddSigningKey(new MutationSigner(GetAdminKey()));

                await builder.Submit();
            }

            var record = await _sdk.Client.GetAccountRecord(account.Path, assetPath);

            account.Balance = record.Balance;

            return account;
        }

        public async Task<AddressModel> CreateAccountAddressAsync(AccountCredentialsModel account, uint index)
        {
            var rootKey = GetAccountKey(account.Seed, account.Passphrase, 0);
            var rootAddress = rootKey.PrivateKey.PubKey.GetAddress(_network).ToString();

            var key = rootKey.Derive(index);

            var address = key.PrivateKey.PubKey.GetAddress(_network);

            var addressModel = new AddressModel
            {
                Base58Address = address.ToString(),
                Chain = $"/0/{index.ToString()}/",
                Index = index
            };

            var dataRecord = await _sdk.Client.GetDataRecord(AddressToAccountPath(address.ToString()), "acl");

            if (dataRecord.Data == null)
            {
                var aclRecords = new List<AclRecord>();

                var receiveAclRecord = new AclRecord
                {
                    Subjects = new List<AclSubject>
                    {
                        new AclSubject
                        {
                            Addresses = new List<string>(),
                            Required = 0
                        }
                    }
                };

                receiveAclRecord
                    .AllowCreate()
                    .AllowModify();

                aclRecords.Add(receiveAclRecord);

                var sendAclRecord = new AclRecord
                {
                    Subjects = new List<AclSubject>
                    {
                        new AclSubject
                        {
                            Addresses = new List<string>
                            {
                                address.ToString(),
                                rootAddress

                            },
                            Required = 1
                        }
                    }
                };

                sendAclRecord
                    .AllowSpend();

                aclRecords.Add(sendAclRecord);

                var aclData = JsonConvert.SerializeObject(aclRecords);

                var builder = _sdk.CreateTransactionBuilder();

                builder.AddRecord(dataRecord.Key, aclRecords, dataRecord.Version);

                builder.AddSigningKey(new MutationSigner(GetAdminKey()));

                await builder.Submit();
            }

            var gotoRecord = await _sdk.Client.GetDataRecord(AddressToAccountPath(address.ToString()), "goto");

            if (gotoRecord == null)
            {
                var builder = _sdk.CreateTransactionBuilder();

                builder.AddRecord(gotoRecord.Key, AddressToAccountPath(address.ToString()), gotoRecord.Version);

                builder.AddSigningKey(new MutationSigner(GetAdminKey()));

                await builder.Submit();
            }

            return addressModel;
        }

        public async Task<long> GetAccountBalanceAsync(string accountPath, string assetPath)
        {
            var record = await _sdk.Client.GetAccountRecord(accountPath, assetPath);

            return record.Balance;
        }

        public async Task<AssetModel> GetAssetAsync(uint index)
        {
            var rootKey = GetIssuanceKey(index);


            var address = rootKey.PrivateKey.PubKey.GetAddress(_network);

            var path = AddressToAssetPath(address.ToString());

            var dataRecord = await _sdk.Client.GetDataRecord(path, "asdef");

            if (dataRecord.Data == null)
            {
                return null;
            }

            var accountRecord = await _sdk.Client.GetAccountRecord(path, path);

            return new AssetModel
            {
                Path = path,
                Balance = accountRecord.Balance,
                Index = index,
                Chain = "0",
                AccountAddress = address.ToString()
            };
        }

        public async Task<AssetModel> CreateAssetAsync(uint index, AssetDefinition definition)
        {
            var rootKey = GetIssuanceKey(index);

            var address = rootKey.PrivateKey.PubKey.GetAddress(_network);

            var path = AddressToAssetPath(address.ToString());

            var dataRecord = await _sdk.Client.GetDataRecord(path, "asdef");

            if (dataRecord.Data != null)
            {
                throw new Exception($"The asset at index {index} has already been defined.");
            }

            var builder = _sdk.CreateTransactionBuilder();

            builder.AddRecord(dataRecord.Key, definition, dataRecord.Version);
            builder.AddSigningKey(new MutationSigner(rootKey));

            await builder.Submit();

            var accountRecord = await _sdk.Client.GetAccountRecord(path, path);

            return new AssetModel
            {
                Path = path,
                Balance = accountRecord.Balance,
                Index = index,
                Chain = "0",
                AccountAddress = address.ToString()
            };
        }

        public async Task<TransactionModel> IssueAssetAsync(string accountPath, AssetModel asset, long amount)
        {
            var rootKey = GetIssuanceKey(asset.Index);

            var builder = _sdk.CreateTransactionBuilder();

            builder.AddSigningKey(new MutationSigner(rootKey));

            await builder.UpdateAccountRecord(asset.Path, asset.Path, -amount);
            await builder.UpdateAccountRecord(accountPath, asset.Path, amount);

            var transactionData = await builder.Submit();

            return new TransactionModel
            {
                Id = transactionData.TransactionHash,

                Entries = new List<TransactionEntryModel> {
                   new TransactionEntryModel { Path = asset.Path, Value = -amount },
                   new TransactionEntryModel { Path = accountPath, Value = amount }
                }
            };
        }

        public async Task<TransactionModel> RetireAssetAsync(string accountPath, AssetModel asset, long amount)
        {
            var rootKey = GetIssuanceKey(asset.Index);

            var builder = _sdk.CreateTransactionBuilder();

            builder.AddSigningKey(new MutationSigner(rootKey));

            await builder.UpdateAccountRecord(asset.Path, asset.Path, amount);
            await builder.UpdateAccountRecord(accountPath, asset.Path, -amount);

            var transactionData = await builder.Submit();

            return new TransactionModel
            {
                Id = transactionData.TransactionHash,

                Entries = new List<TransactionEntryModel>
               {
                   new TransactionEntryModel { Path = asset.Path, Value = -amount },
                   new TransactionEntryModel { Path = accountPath, Value = amount }
               }
            };
        }

        public async Task<TransactionModel> TransferAssetAsync(AccountCredentialsModel account, List<TransactionEntryModel> recipients, string assetPath)
        {
            var rootKey = GetAccountKey(account.Seed, account.Passphrase, 0);

            var address = rootKey.PrivateKey.PubKey.GetAddress(_network);

            var accountPath = AddressToAccountPath(address.ToString());

            var builder = _sdk.CreateTransactionBuilder();

            builder.AddSigningKey(new MutationSigner(rootKey));

            long sum = 0;

            // Ensure recipients are unique.

            var uniqueRecipients = new List<TransactionEntryModel>();

            foreach (var recipient in recipients)
            {
                var uniqueRecipient = uniqueRecipients.FirstOrDefault(u => u.Path.Equals(recipient.Path));

                if (uniqueRecipient == null)
                {
                    uniqueRecipients.Add(new TransactionEntryModel
                    {
                        Path = recipient.Path,
                        Value = recipient.Value
                    });
                }
                else
                {
                    uniqueRecipient.Value += recipient.Value;
                }
            }

            foreach (var recipient in uniqueRecipients)
            {
                await builder.UpdateAccountRecord(recipient.Path, assetPath, recipient.Value);

                sum += recipient.Value;
            }

            await builder.UpdateAccountRecord(accountPath, assetPath, -sum);

            var transactionData = await builder.Submit();

            var model = new TransactionModel
            {
                Id = transactionData.TransactionHash,

                Entries = recipients
            };

            model.Entries.Add(new TransactionEntryModel
            {
                Path = accountPath,
                Value = -sum
            });

            return model;
        }
    }
}
