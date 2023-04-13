using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Domain.Persistence.Sequences;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Transversals.Business.Core.Domain.Exceptions;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.Core.Domain.Configuration.Counters
{
    [ExcludeFromCodeCoverage]
    public class CounterService : ICounterService
    {
        private readonly ICounterRepository _counterRepository;
        private readonly ISequenceManager _sequenceManager;
        private readonly ISequence _sequence;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _sp;
        private static string ComputeSequenceName(Counter counter) => $"Counter_{counter.Name}";

        public CounterService(ICounterRepository counterRepository,
            ISequenceManager sequenceManager,
            ISequence sequence,
            IUnitOfWork unitOfWork,
            IServiceProvider sp)
        {
            _counterRepository = counterRepository;
            _sequenceManager = sequenceManager;
            _sequence = sequence;
            _unitOfWork = unitOfWork;
            _sp = sp;

        }
        public static Counter CreateCounterEntity(string name, string? prefix, string? suffix, int initialValue, int maxValue = int.MaxValue - 1)
        {
            return new Counter
            {
                Name = name,
                Prefixe = prefix,
                Suffixe = suffix,
                Value = initialValue,
                MaxValue = maxValue,
                Locked = true,
            };
        }
        /// <summary>
        /// Creates the new counter asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="maxValue">The maximum value (default int.MaxValue - 1).</param>
        public async Task CreateNewCounterAsync(string name, string? prefix, string? suffix, int initialValue, int maxValue = int.MaxValue - 1)
        {
            Counter couter = CreateCounterEntity(name, prefix, suffix, initialValue, maxValue);
            await EnsureSequenceCreatedAsync(couter);
            _counterRepository.Add(couter);
            await _unitOfWork.SaveAsync();
        }

        private Business.Domain.Entities.Counter? GetCounter(string counterName)
        {
            return _counterRepository.GetQuery().FirstOrDefault(c => c.Name == counterName);
        }

        public bool TryFindCounter(string counterName)
        {
            return GetCounter(counterName) != null;
        }

        public async Task<string> NextFormatedValueAsync(string counterName)
        {
            TransactionOptions topt = new();
            topt.IsolationLevel = IsolationLevel.RepeatableRead;

            using (Locker.Lock())
            {
                using var scope = _sp.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                Counter currentCounter = _counterRepository.GetQuery().Single(c => c.Name == counterName);
                await EnsureSequenceCreatedAsync(currentCounter);
                var seq = await _sequenceManager.GetSequenceAsync(ComputeSequenceName(currentCounter));
                long? value = await _sequence.GetCurrentValueAsync(ComputeSequenceName(currentCounter));
                if (seq.MaxValue == value)
                {
                    throw new MaxValueException(Resources.Core.MaxValueExceptionMessage);
                }
                if (currentCounter.Value == null)
                {
                    throw new NullValueException(Resources.Core.CounterNullValueExceptionMessage);
                }

                currentCounter.Value = await _sequence.GetNextValueAsync(ComputeSequenceName(currentCounter));

                await unitOfWork.SaveAsync();
                return $"{currentCounter.Prefixe}{currentCounter.Value:0000000}{currentCounter.Suffixe}";

            }
        }

        private async Task EnsureSequenceCreatedAsync(Counter counter)
        {
            string name = ComputeSequenceName(counter);
            if (!await _sequenceManager.SequenceExistsAsync(name))
            {
                SequenceInfo seq = new(name);
                seq.StartValue = counter.Value ?? 0;
                seq.MinValue = 0;
                seq.MaxValue = counter.MaxValue ?? long.MaxValue - 1;
                await _sequenceManager.CreateSequenceAsync(seq);
            }
        }
    }
}
