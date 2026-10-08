using Niwahana_backend.Models.Domain;
using Niwahana_backend.Repository.Data;

namespace Niwahana_backend.Business
{
    public class BungalowsBusiness
    {
        private readonly BungalowsRepository _bungalowsRepository;

        public BungalowsBusiness(BungalowsRepository bungalowsRepository)
        {
            _bungalowsRepository = bungalowsRepository;
        }

        public List<BungalowsMl> GetAllBungalows()
        {
            return _bungalowsRepository.GetAllBungalows();
        }

        public BungalowsMl? GetBungalowsById(int bungalowId)
        {
            return _bungalowsRepository.GetBungalowsById(bungalowId);
        }

        public bool AddBungalow(BungalowsMl bungalow)
        {
            return _bungalowsRepository.AddBungalow(bungalow);
        }

        public bool UpdateBungalows(BungalowsMl bungalows)
        {
            return _bungalowsRepository.UpdateBungalows(bungalows);
        }

         public bool DeleteBungalow(int bungalowId)
        {
            return _bungalowsRepository.DeleteBungalow(bungalowId);
        }
    }
}