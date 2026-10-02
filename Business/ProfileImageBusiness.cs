using Niwahana_backend.Models.Domain;
using Niwahana_backend.Repository.Data;

namespace Niwahana_backend.Business
{
    public class ProfileImageBusiness
    {
        private readonly ProfileImageRepository _repository;

        public ProfileImageBusiness(ProfileImageRepository repository)
        {
            _repository = repository;
        }
      
        // GET
       
        public  ProfileImageMl? GetImage(int userId)
        {
            return  _repository.GetImage(userId);
        }

        // ADD / UPDATE
    
        public bool SaveImage(ProfileImageMl image)
        {
            var exists =  _repository.ImageExists(image.UserId);

            if (exists)
            {
                return  _repository.UpdateImage(image);
            }

            return  _repository.AddImage(image);
        }

        // DELETE

        public  bool DeleteImage(int userId)
        {
            return  _repository.DeleteImage(userId);
        }
    }
}