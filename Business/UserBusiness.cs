using Niwahana_backend.Models.Domain;
using Niwahana_backend.Repository.Data;

namespace Niwahana_backend.Business
{
    public class UserBusiness
    {
        private readonly UserRepository _userRepository;

        public UserBusiness(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public  UserMl? GetUserById(int userId)
        {
            return  _userRepository.GetUserById(userId);
        }

        public  bool UpdateUser(UserMl user)
        {
            return  _userRepository.UpdateUser(user);
        }
    }
}