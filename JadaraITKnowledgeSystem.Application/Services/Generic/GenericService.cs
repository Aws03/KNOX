//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//using JadaraITKnowledgeSystem.Application.Interfaces.Services.Generic;

//namespace JadaraITKnowledgeSystem.Application.Services.Generic
//{
//    public class GenericService<TEntity, TDto> : IGenericService<TDto>
//        where TEntity : class
//        where TDto : class
//    {
//        protected readonly IGenericRepository<TEntity> _repository;
//        private readonly IUnitOfWork _unitOfWork;
//        protected readonly IMapper _mapper;

//        public GenericService(IGenericRepository<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _repository = repository;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }

//        public virtual async Task<TDto> GetByIdAsync(int id)
//        {
//            var entity = await _repository.GetByIdAsync(id);
//            return _mapper.Map<TDto>(entity);
//        }

//        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
//        {
//            var entities = await _repository.GetAllAsync();
//            return _mapper.Map<IEnumerable<TDto>>(entities);
//        }

//        public virtual async Task<TDto> AddAsync(TDto dto)
//        {
//            var entity = _mapper.Map<TEntity>(dto);
//            await _repository.AddAsync(entity);
//            await _unitOfWork.SaveChangesAsync();

//            return _mapper.Map<TDto>(entity);
//        }

//        public virtual async Task UpdateAsync(TDto dto)
//        {
//            var entity = _mapper.Map<TEntity>(dto);
//            await _repository.UpdateAsync(entity);
//            await _unitOfWork.SaveChangesAsync();
//        }

//        public virtual async Task DeleteAsync(int id)
//        {
//            var entity = await _repository.GetByIdAsync(id);
//            if (entity == null)
//                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found.");

//            await _repository.DeleteAsync(entity);
//            await _unitOfWork.SaveChangesAsync();
//        }
//    }
//}
