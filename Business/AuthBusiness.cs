using Niwahana_backend.Models.Domain;
using Niwahana_backend.Repository.Data;

namespace Niwahana_backend.Business
{
    public class AuthBusiness
    {
        private readonly AuthRepository _authRepository;

        public AuthBusiness(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public  UserMl? Login(string email,string password)
        {
            return  _authRepository.Login(email,password);
        }
    }
}