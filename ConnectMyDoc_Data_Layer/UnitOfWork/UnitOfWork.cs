/*using ConnectMyDoc_Data_Layer.Context;
using ConnectMyDoc_Domain_Layer.Repository;
using Microsoft.EntityFrameworkCore.Storage;


namespace ConnectMyDoc_Data_Layer.UnitOfWork
{
    public class UnitOfWork 
    {
        private readonly PatientCMDDbContext _context;
        private IDbContextTransaction _transaction;
        public IPatientRepository _patientRepository;
        public IPatientGuardianRepository _patientGuardianRepository;
        public IPatientAddressRepository _patientAddressRepository;
        public UnitOfWork(PatientCMDDbContext context, IPatientAddressRepository patientAddressRepository, IPatientRepository patientRepository, IPatientGuardianRepository patientGuardianRepository)
        {
            _context = context;
            _patientRepository = patientRepository;
            _patientAddressRepository = patientAddressRepository;

        }
*//*
        public IPatientRepository PatientRepository => new PatientRepository(_context);
        public IPatientGuardianRepository PatientGuardianRepository => new PatientGuardianRepository(_context);
        public IPatientAddressRepository PatientAddressRepository => new PatientAddressRepository(_context);*//*

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
            _transaction.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
            _transaction.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }
}
*/